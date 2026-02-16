using System;
using System.Collections.Generic;
using System.Linq;
using Core.Language.Morphology;
using Core.Language.Shared;

namespace Core.Language.Syntax;

public enum WordOrder
{
    SVO,
    SOV,
    VSO,
    VOS,
    OVS,
    OSV
}

public enum ClauseTemplate
{
    Intransitive,
    Transitive,
    Copular
}

public sealed class SyntaxSettings
{
    public WordOrder WordOrder { get; set; } = WordOrder.SVO;
    public ClauseTemplate ClauseTemplate { get; set; } = ClauseTemplate.Transitive;
}

public sealed class ConfigurableSyntaxGenerator : ISyntaxGenerator
{
    private readonly ILexicon _lexicon;
    public SyntaxSettings Settings { get; } = new();

    public ConfigurableSyntaxGenerator(ILexicon lexicon) => _lexicon = lexicon;

    public SyntaxTree realize(SentenceSpec spec, Random rng)
    {
        var nodes = BuildClauseNodes(spec);
        var order = ExpandWordOrder(Settings.WordOrder, nodes.Keys);
        var root = new SyntaxNode
        {
            Pos = "CLAUSE",
            LemmaId = Settings.ClauseTemplate.ToString()
        };

        foreach (var role in order)
            if (nodes.TryGetValue(role, out var node))
                root.Children.Add(node);

        return new SyntaxTree { Root = root };
    }

    private Dictionary<string, SyntaxNode> BuildClauseNodes(SentenceSpec spec)
    {
        var lexemes = _lexicon.GetAll();
        var nouns = lexemes.Where(l => l.Pos == "N").ToList();
        var verbs = lexemes.Where(l => l.Pos == "V").ToList();
        var adjs = lexemes.Where(l => l.Pos == "ADJ").ToList();

        var subjLemma = nouns.FirstOrDefault()?.Lemma ?? "person";
        var objLemma = nouns.Skip(1).FirstOrDefault()?.Lemma ?? subjLemma;
        var verbLemma = verbs.FirstOrDefault()?.Lemma ?? "see";
        var predicateLemma = adjs.FirstOrDefault()?.Lemma ?? nouns.LastOrDefault()?.Lemma ?? "being";

        var tense = spec.constraints is not null && spec.constraints.TryGetValue("Tense", out var t)
            ? t
            : "Pres";

        var subj = NewRoleNode("N", subjLemma, "Subj", [new Feat("Number", "Sing")]);
        var obj = NewRoleNode("N", objLemma, "Obj", [new Feat("Number", "Sing")]);
        var verb = NewRoleNode("V", verbLemma, "Verb", [new Feat("Tense", tense)]);
        var pred = NewRoleNode("ADJ", predicateLemma, "Pred", []);

        return Settings.ClauseTemplate switch
        {
            ClauseTemplate.Intransitive => new Dictionary<string, SyntaxNode>
            {
                ["S"] = subj,
                ["V"] = verb
            },
            ClauseTemplate.Copular => new Dictionary<string, SyntaxNode>
            {
                ["S"] = subj,
                ["V"] = NewRoleNode("V", "be", "Verb", [new Feat("Tense", tense)]),
                ["O"] = pred
            },
            _ => new Dictionary<string, SyntaxNode>
            {
                ["S"] = subj,
                ["V"] = verb,
                ["O"] = obj
            }
        };
    }

    private static IReadOnlyList<string> ExpandWordOrder(WordOrder order, IEnumerable<string> available)
    {
        var canonical = order switch
        {
            WordOrder.SOV => new[] { "S", "O", "V" },
            WordOrder.VSO => new[] { "V", "S", "O" },
            WordOrder.VOS => new[] { "V", "O", "S" },
            WordOrder.OVS => new[] { "O", "V", "S" },
            WordOrder.OSV => new[] { "O", "S", "V" },
            _ => new[] { "S", "V", "O" }
        };

        var present = new HashSet<string>(available);
        return canonical.Where(present.Contains).ToList();
    }

    private static SyntaxNode NewRoleNode(string pos, string lemma, string role, IReadOnlyList<Feat> extra)
    {
        var features = new List<Feat> { new("Role", role) };
        features.AddRange(extra);

        return new SyntaxNode
        {
            Pos = pos,
            LemmaId = lemma,
            features = new FeatBundle(features)
        };
    }
}
