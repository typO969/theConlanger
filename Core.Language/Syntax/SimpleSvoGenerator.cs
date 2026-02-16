using System;
using System.Linq;
using Core.Language.Morphology;
using Core.Language.Shared;

namespace Core.Language.Syntax;

public sealed class SimpleSvoGenerator : ISyntaxGenerator
{
    private readonly ILexicon _lex;
    public SimpleSvoGenerator(ILexicon lex) => _lex = lex;

    public SyntaxTree realize(SentenceSpec spec, Random rng)
    {
        var lexemes = _lex.GetAll();
        var nounLemmas = lexemes.Where(l => l.pos == "N").Select(l => l.lemma).ToList();
        var verbLemmas = lexemes.Where(l => l.pos == "V").Select(l => l.lemma).ToList();

        var subjLemma = nounLemmas.FirstOrDefault() ?? "person";
        var objLemma = nounLemmas.Skip(1).FirstOrDefault() ?? subjLemma;
        var verbLemma = verbLemmas.FirstOrDefault() ?? "see";

        var subj = new SyntaxNode {
            Pos = "N", LemmaId = subjLemma,
            features = new FeatBundle(new[] { new Feat("Role", "Subj") })
        };
        var obj = new SyntaxNode {
            Pos = "N", LemmaId = objLemma,
            features = new FeatBundle(new[] { new Feat("Role", "Obj") })
        };
        var verb = new SyntaxNode {
            Pos = "V", LemmaId = verbLemma,
            features = new FeatBundle(new[] { new Feat("Tense", "Pres") })
        };
        verb.Children.Add(subj);
        verb.Children.Add(obj);
        return new SyntaxTree { Root = verb };
    }
}
