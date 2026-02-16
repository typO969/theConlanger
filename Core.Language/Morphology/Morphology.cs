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
				seq.Add(new MorphemeToken(stem, stem.underlyingPhones));
			}

			return seq;
		}
	}
}
