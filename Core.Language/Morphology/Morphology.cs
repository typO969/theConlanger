using System;
using System.Collections.Generic;
using System.Linq;
using Core.Language.Shared;
using Core.Language.Syntax;

namespace Core.Language.Morphology;

public sealed record AffixRule(
    string Name,
    string Pos,
    string FeatureName,
    string FeatureValue,
    string Affix,
    bool IsPrefix = false,
    bool Enabled = true);

public sealed class Morphology
{
    public ILexicon Lexicon { get; }
    public List<AffixRule> AffixRules { get; } = new();

    public Morphology(ILexicon lex)
    {
        Lexicon = lex;
        AffixRules.Add(new("Plural", "N", "Number", "Pl", "-i"));
        AffixRules.Add(new("Past", "V", "Tense", "Past", "-ta"));
    }

    public IReadOnlyList<MorphemeToken> inflect(SyntaxTree tree, Random rng)
    {
        var seq = new List<MorphemeToken>();

        foreach (var node in tree.linearize())
        {
            var lex = Lexicon.Resolve(node.LemmaId);
            var basePhones = SplitPhones(lex.rootPhones.TryGetValue("UR", out var ur) ? ur : lex.Lemma);
            var morphemes = new List<Morpheme>
            {
                new("STEM", lex.Lemma, basePhones, _ => true)
            };

            foreach (var rule in AffixRules.Where(r => r.Enabled && r.Pos == node.Pos && node.features.Has(r.FeatureName, r.FeatureValue)))
            {
                var affixPhones = SplitPhones(rule.Affix.Replace("-", ""));
                var morpheme = new Morpheme($"AFX_{rule.Name}", rule.Name, affixPhones, _ => true);
                if (rule.IsPrefix)
                    morphemes.Insert(0, morpheme);
                else
                    morphemes.Add(morpheme);
            }

            foreach (var morpheme in morphemes)
                seq.Add(new MorphemeToken(morpheme, morpheme.underlyingPhones));
        }

        return seq;
    }

    public string RealizeForPreview(string lemma, string pos, FeatBundle features)
    {
        var lex = Lexicon.Resolve(lemma);
        var surface = lex.rootPhones.TryGetValue("UR", out var ur) ? ur : lemma;

        var applicable = AffixRules.Where(r => r.Enabled && r.Pos == pos && features.Has(r.FeatureName, r.FeatureValue));
        foreach (var rule in applicable)
            surface = rule.IsPrefix
                ? $"{rule.Affix.Replace("-", "")} {surface}"
                : $"{surface} {rule.Affix.Replace("-", "")}";

        return surface;
    }

    private static string[] SplitPhones(string source) => source
        .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
