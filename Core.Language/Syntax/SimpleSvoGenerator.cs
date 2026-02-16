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
        var nounLemmas = lexemes.Where(l => l.Pos == "N").Select(l => l.Lemma).ToList();
        var verbLemmas = lexemes.Where(l => l.Pos == "V").Select(l => l.Lemma).ToList();

        var subjLemma = nounLemmas.FirstOrDefault() ?? "person";
        var objLemma = nounLemmas.Skip(1).FirstOrDefault() ?? subjLemma;
        var verbLemma = verbLemmas.FirstOrDefault() ?? "see";

        return new SyntaxTree
        {
            Root = new SyntaxNode
            {
                Pos = "CLAUSE",
                Children =
                {
                    new SyntaxNode { Pos = "N", LemmaId = subjLemma, features = new(new[] { new Feat("Role", "Subj") }) },
                    new SyntaxNode { Pos = "V", LemmaId = verbLemma, features = new(new[] { new Feat("Role", "Verb"), new Feat("Tense", "Pres") }) },
                    new SyntaxNode { Pos = "N", LemmaId = objLemma, features = new(new[] { new Feat("Role", "Obj") }) }
                }
            }
        };
    }
}
