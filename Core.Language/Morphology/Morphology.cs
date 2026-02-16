using System.Collections.Generic;
using System;
using Core.Language.Shared;
using Core.Language.Syntax;

namespace Core.Language.Morphology
{
	public record Lexeme(string lemma, string pos, Dictionary<string, string> rootPhones);

	public sealed class Morphology
	{
		public List<Paradigm> paradigms { get; } = new(); // Fixes IDE1006
		public ILexicon Lexicon { get; }
		public Morphology(ILexicon lex) => Lexicon = lex;

		public IReadOnlyList<MorphemeToken> inflect(SyntaxTree tree, Random rng)
		{
			var seq = new List<MorphemeToken>();
			foreach (var node in tree.linearize())
			{
				var lex = Lexicon.Resolve(node.LemmaId);
				var stem = new Morpheme("STEM", lex.lemma,
					 lex.rootPhones["UR"].Split(' '),
					 _ => true);
				// Replace 'stem.UnderlyingPhones' with 'stem.underlyingPhones' to match the property name in Morpheme
				seq.Add(new MorphemeToken(stem, stem.underlyingPhones));
			}

			var lexp = Lexicon.Resolve("person");
			var stemp = new Morpheme(
				 "STEM",
				 lexp.lemma,
				 lexp.rootPhones["UR"].Split(' '),
				 _ => true
			);
			return new[] { new MorphemeToken(stemp, stemp.underlyingPhones) };
		}
	}
}

//public sealed class Morphology
//{
//    public List<Paradigm> paradigms { get; } = new(); // Fixes IDE1006
//    public ILexicon Lexicon { get; }

//    public Morphology(ILexicon lex) => Lexicon = lex;

//    // Fixes IDE1006: method name should start with lower case
//    public IReadOnlyList<MorphemeToken> inflect(SyntaxTree tree, Random rng)
//    {
//        var seq = new List<MorphemeToken>();

//        // Change 'RootPhones' to 'rootPhones' to match the property name in Lexeme
//        foreach (var node in tree.linearize())
//        {
//            var lex = Lexicon.Resolve(node.LemmaId);
//            var stem = new Morpheme("STEM", lex.Lemma,
//                lex.rootPhones["UR"].Split(' '),
//                _ => true);
//            // Replace 'stem.UnderlyingPhones' with 'stem.underlyingPhones' to match the property name in Morpheme
//            seq.Add(new MorphemeToken(stem, stem.underlyingPhones));
//        }

//        return seq;
//    }
//}
