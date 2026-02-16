using System;
using Core.Language.Phonology;
using Core.Language.Morphology;
using Core.Language.Syntax;
using Core.Language.Orthography;
using Core.Language.Evaluation;

namespace Core.Language.Shared;

public sealed class LangEngine
{
	public Core.Language.Phonology.Phonology Phonology { get; }
	public Core.Language.Morphology.Morphology Morphology { get; }
	public Core.Language.Syntax.Syntax Syntax { get; }
	public Core.Language.Orthography.Orthography Orthography { get; }
	public Core.Language.Evaluation.Evaluation Evaluation { get; }

	public LangEngine(Core.Language.Phonology.Phonology p, Core.Language.Morphology.Morphology m, Core.Language.Syntax.Syntax s, Core.Language.Orthography.Orthography o, Core.Language.Evaluation.Evaluation e)
	{
		Phonology = p;
		Morphology = m;
		Syntax = s;
		Orthography = o;
		Evaluation = e;
	}

	public string GenerateSentence(Random rng, SentenceSpec spec)
	{
		var sTree = Syntax.realize(spec, rng);
		var morphemes = Morphology.inflect(sTree, rng);
		var surface = Phonology.Surface(morphemes, rng);
		var written = Orthography.Render(surface, rng);
		Evaluation.CheckAll(surface);
		return written;
	}
}

