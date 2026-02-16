using Core.Language.Morphology;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Phonology;

public sealed record DerivationStep(string Label, string Value);

public sealed class Phonology
{
    public PhonemeInventory Inventory { get; }
    public Phonotactics Tactics { get; }
    public Syllabifier Syllabifier { get; }
    public List<IPhonoRule> Rules { get; } = new();

    public Phonology(PhonemeInventory inv, Phonotactics tact)
    {
        Inventory = inv;
        Tactics = tact;
        Syllabifier = new Syllabifier(tact);
    }

    public string Surface(IEnumerable<MorphemeToken> morphemes, Random rng)
    {
        var result = Derive(morphemes);
        return result.FinalSurface;
    }

    public (string FinalSurface, IReadOnlyList<DerivationStep> Steps, IReadOnlyList<string> PhonotacticIssues) Derive(IEnumerable<MorphemeToken> morphemes)
    {
        var underlying = morphemes.SelectMany(m => m.underlyingPhones).ToList();
        var steps = new List<DerivationStep>
        {
            new("UR", string.Join(" ", underlying))
        };

        IReadOnlyList<string> current = underlying;
        foreach (var rule in Rules)
        {
            current = rule.apply(current);
            steps.Add(new(rule.Name, string.Join(" ", current)));
        }

        var issues = CheckPhonotactics(current);
        if (issues.Count > 0)
            steps.Add(new("Phonotactics", string.Join("; ", issues)));

        return (string.Join("", current), steps, issues);
    }

    private IReadOnlyList<string> CheckPhonotactics(IReadOnlyList<string> phones)
    {
        var issues = new List<string>();
        for (var i = 0; i < phones.Count - 1; i++)
        {
            var cluster = phones[i] + phones[i + 1];
            var legal = Tactics.allowedOnsets.Count == 0 || Tactics.IsValidOnset(cluster) || Tactics.IsValidCoda(cluster);
            if (!legal)
                issues.Add($"Illegal cluster: {cluster} @ {i}");
        }

        return issues;
    }
}
