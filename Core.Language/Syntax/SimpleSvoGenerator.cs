using System;
using Core.Language.Morphology;
using Core.Language.Shared;

namespace Core.Language.Syntax;

public sealed class SimpleSvoGenerator : ISyntaxGenerator
{
    private readonly ILexicon _lex;
    public SimpleSvoGenerator(ILexicon lex) => _lex = lex;

    // Fix: Ensure method name matches interface (lowercase 'realize')
    public SyntaxTree realize(SentenceSpec spec, Random rng)
    {
        var subj = new SyntaxNode {
            Pos = "N", LemmaId = "person",
            features = new FeatBundle(new[] { new Feat("Role", "Subj") })
        };
        var obj = new SyntaxNode {
            Pos = "N", LemmaId = "fish",
            features = new FeatBundle(new[] { new Feat("Role", "Obj") })
        };
        var verb = new SyntaxNode {
            Pos = "V", LemmaId = "see",
            features = new FeatBundle(new[] { new Feat("Tense", "Pres") })
        };
        verb.Children.Add(subj);
        verb.Children.Add(obj);
        return new SyntaxTree { Root = verb };
    }
}
