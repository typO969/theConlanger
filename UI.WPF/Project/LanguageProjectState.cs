using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Core.Language.Morphology;
using Core.Language.Orthography;
using Core.Language.Phonology;
using Core.Language.Shared;
using Core.Language.Syntax;

namespace UI.WPF.Project;

public sealed class LanguageProjectState
{
    public string Name { get; set; } = "My Conlang";
    public List<string> Consonants { get; set; } = new();
    public List<string> Vowels { get; set; } = new();
    public List<RuleSpec> PhonologyRules { get; set; } = new();
    public List<LexemeState> Lexemes { get; set; } = new();
    public List<AffixState> Affixes { get; set; } = new();
    public WordOrder WordOrder { get; set; } = WordOrder.SVO;
    public ClauseTemplate ClauseTemplate { get; set; } = ClauseTemplate.Transitive;
    public List<OrthRuleState> OrthographyRules { get; set; } = new();
    public List<OrthExceptionState> OrthographyExceptions { get; set; } = new();
    public CapitalizationMode Capitalization { get; set; } = CapitalizationMode.SentenceInitial;

    public static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static LanguageProjectState Capture(string name, LangEngine engine, ConfigurableSyntaxGenerator syntax, IEnumerable<RuleSpec>? phonologyRuleSpecs = null)
    {
        return new LanguageProjectState
        {
            Name = name,
            Consonants = engine.Phonology.Inventory.consonants.Select(p => p.Symbol).ToList(),
            Vowels = engine.Phonology.Inventory.vowels.Select(p => p.Symbol).ToList(),
            Lexemes = engine.Morphology.Lexicon.GetAll().Select(l => new LexemeState(l.Lemma, l.Pos, l.rootPhones.GetValueOrDefault("UR", ""))).ToList(),
            Affixes = engine.Morphology.AffixRules.Select(a => new AffixState(a.Name, a.Pos, a.FeatureName, a.FeatureValue, a.Affix, a.IsPrefix, a.Enabled)).ToList(),
            WordOrder = syntax.Settings.WordOrder,
            ClauseTemplate = syntax.Settings.ClauseTemplate,
            OrthographyRules = engine.Orthography.Rules.Select(r => new OrthRuleState(r.Phone, r.Grapheme, r.PrevContext, r.NextContext, r.Enabled)).ToList(),
            OrthographyExceptions = engine.Orthography.Exceptions.Select(e => new OrthExceptionState(e.Input, e.Output)).ToList(),
            Capitalization = engine.Orthography.Capitalization,
            PhonologyRules = (phonologyRuleSpecs ?? engine.Phonology.Rules
                .OfType<RewriteRule>()
                .Select(r => new RuleSpec { Name = r.Name }))
                .Select(r => new RuleSpec
                {
                    Name = r.Name,
                    Target = r.Target,
                    Replacement = r.Replacement,
                    PrevPhone = r.PrevPhone,
                    NextPhone = r.NextPhone,
                    Enabled = r.Enabled
                })
                .ToList()
        };
    }

    public void Apply(LangEngine engine, ConfigurableSyntaxGenerator syntax)
    {
        engine.Phonology.Inventory.consonants = Consonants.Select(c => new Phoneme(c, [])).ToList();
        engine.Phonology.Inventory.vowels = Vowels.Select(v => new Phoneme(v, [])).ToList();

        engine.Phonology.Rules.Clear();
        foreach (var rule in PhonologyRules)
            engine.Phonology.Rules.Add(rule.ToRule());

        if (engine.Morphology.Lexicon is InMemoryLexicon mem)
            mem.ReplaceAll(Lexemes.Select(l => new Lexeme(l.Lemma, l.Pos, new() { ["UR"] = l.UR })));

        engine.Morphology.AffixRules.Clear();
        foreach (var a in Affixes)
            engine.Morphology.AffixRules.Add(new AffixRule(a.Name, a.Pos, a.FeatureName, a.FeatureValue, a.Affix, a.IsPrefix, a.Enabled));

        syntax.Settings.WordOrder = WordOrder;
        syntax.Settings.ClauseTemplate = ClauseTemplate;

        engine.Orthography.Rules.Clear();
        foreach (var r in OrthographyRules)
            engine.Orthography.Rules.Add(new OrthographyRule(r.Phone, r.Grapheme, r.PrevContext, r.NextContext, r.Enabled));

        engine.Orthography.Exceptions.Clear();
        foreach (var e in OrthographyExceptions)
            engine.Orthography.Exceptions.Add(new OrthographyException(e.Input, e.Output));

        engine.Orthography.Capitalization = Capitalization;
    }

    public bool SemanticallyEquals(LanguageProjectState other)
    {
        var left = JsonSerializer.Serialize(this, JsonOptions);
        var right = JsonSerializer.Serialize(other, JsonOptions);
        return left == right;
    }

    public LanguageProjectState Clone()
    {
        var json = JsonSerializer.Serialize(this, JsonOptions);
        return JsonSerializer.Deserialize<LanguageProjectState>(json, JsonOptions) ?? new LanguageProjectState();
    }
}

public sealed record LexemeState(string Lemma, string Pos, string UR);
public sealed record AffixState(string Name, string Pos, string FeatureName, string FeatureValue, string Affix, bool IsPrefix, bool Enabled);
public sealed record OrthRuleState(string Phone, string Grapheme, string? PrevContext, string? NextContext, bool Enabled);
public sealed record OrthExceptionState(string Input, string Output);
