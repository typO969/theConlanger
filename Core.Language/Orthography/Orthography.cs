using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Language.Orthography;

public sealed record OrthographyRule(
    string Phone,
    string Grapheme,
    string? PrevContext = null,
    string? NextContext = null,
    bool Enabled = true);

public sealed record OrthographyException(string Input, string Output);

public enum CapitalizationMode
{
    None,
    SentenceInitial,
    AllWords
}

public sealed class Orthography
{
    public List<OrthographyRule> Rules { get; } = new();
    public List<OrthographyException> Exceptions { get; } = new();
    public CapitalizationMode Capitalization { get; set; } = CapitalizationMode.SentenceInitial;

    public Orthography()
    {
        Rules.AddRange([
            new("ʃ", "sh"),
            new("t", "c", NextContext: "i"),
            new("k", "c", NextContext: "e"),
            new("k", "k")
        ]);
    }

    public string Render(string surfacePhones, Random rng)
    {
        if (string.IsNullOrWhiteSpace(surfacePhones))
            return string.Empty;

        var words = surfacePhones
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(MapWord)
            .ToList();

        var rendered = string.Join(' ', words);
        return ApplyCapitalization(rendered);
    }

    private string MapWord(string word)
    {
        var excepted = Exceptions.FirstOrDefault(e => string.Equals(e.Input, word, StringComparison.Ordinal));
        if (excepted is not null)
            return excepted.Output;

        var orderedRules = Rules
            .Where(r => r.Enabled && !string.IsNullOrWhiteSpace(r.Phone))
            .OrderByDescending(r => r.Phone.Length)
            .ToList();

        var sb = new StringBuilder();
        for (var i = 0; i < word.Length;)
        {
            var matched = false;
            foreach (var rule in orderedRules)
            {
                if (i + rule.Phone.Length > word.Length)
                    continue;

                if (!word.AsSpan(i, rule.Phone.Length).SequenceEqual(rule.Phone.AsSpan()))
                    continue;

                if (!MatchesContext(word, i, rule))
                    continue;

                sb.Append(rule.Grapheme);
                i += rule.Phone.Length;
                matched = true;
                break;
            }

            if (!matched)
            {
                sb.Append(word[i]);
                i++;
            }
        }

        return sb.ToString();
    }

    private static bool MatchesContext(string word, int index, OrthographyRule rule)
    {
        if (!string.IsNullOrWhiteSpace(rule.PrevContext))
        {
            var prevIndex = index - rule.PrevContext.Length;
            if (prevIndex < 0 || !word.AsSpan(prevIndex, rule.PrevContext.Length).SequenceEqual(rule.PrevContext.AsSpan()))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(rule.NextContext))
        {
            var nextIndex = index + rule.Phone.Length;
            if (nextIndex + rule.NextContext.Length > word.Length || !word.AsSpan(nextIndex, rule.NextContext.Length).SequenceEqual(rule.NextContext.AsSpan()))
                return false;
        }

        return true;
    }

    private string ApplyCapitalization(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        return Capitalization switch
        {
            CapitalizationMode.AllWords => string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(CapitalizeWord)),
            CapitalizationMode.SentenceInitial => char.ToUpperInvariant(text[0]) + text[1..],
            _ => text
        };
    }

    private static string CapitalizeWord(string word) => string.IsNullOrEmpty(word)
        ? word
        : char.ToUpperInvariant(word[0]) + word[1..];
}
