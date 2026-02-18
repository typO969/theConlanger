using System;
using System.Linq;
using ConlangBuilder.Core;

namespace UI.WPF.Project;

public static class PluginModelBuilder
{
    public static ConlangModel Build(LanguageProjectState state)
    {
        var model = new ConlangModel
        {
            Name = state.Name
        };

        foreach (var consonant in state.Consonants)
            model.Inventory.Phonemes.Add(new Phoneme(consonant, "C"));

        foreach (var vowel in state.Vowels)
            model.Inventory.Phonemes.Add(new Phoneme(vowel, "V"));

        foreach (var rule in state.PhonologyRules)
            model.SoundRules.Add(new SoundRule
            {
                Enabled = rule.Enabled,
                Input = rule.Target,
                Output = rule.Replacement,
                Environment = BuildEnvironment(rule.PrevPhone, rule.NextPhone)
            });

        foreach (var orth in state.OrthographyRules.Where(r => r.Enabled))
            model.Orthography.Add(new OrthographyMap(orth.Phone, orth.Grapheme));

        foreach (var affix in state.Affixes.Where(a => !string.IsNullOrWhiteSpace(a.Affix)))
        {
            model.Morphology.Add(new MorphRule
            {
                Type = affix.IsPrefix ? AffixType.Prefix : AffixType.Suffix,
                Form = affix.Affix,
                Meaning = string.IsNullOrWhiteSpace(affix.FeatureName)
                    ? affix.Name
                    : $"{affix.FeatureName}={affix.FeatureValue}"
            });
        }

        return model;
    }

    private static string BuildEnvironment(string? prevPhone, string? nextPhone)
    {
        var prev = string.IsNullOrWhiteSpace(prevPhone) ? "" : prevPhone;
        var next = string.IsNullOrWhiteSpace(nextPhone) ? "" : nextPhone;
        if (string.IsNullOrEmpty(prev) && string.IsNullOrEmpty(next))
            return "";

        return $"{prev}_{next}";
    }
}
