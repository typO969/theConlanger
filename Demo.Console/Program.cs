using System;
using System.Collections.Generic;

using Core.Language.Shared;
using Core.Language.Phonology;
using Core.Language.Morphology;
using Core.Language.Syntax;
using Core.Language.Orthography;
using Core.Language.Evaluation;

internal class Program
{
    static void Main()
    {
        var inv = new PhonemeInventory
        {
            consonants = [ new("p",[]), new("t",[]), new("k",[]), new("ʃ",[]) ],
            vowels = [ new("a",[]), new("e",[]), new("i",[]), new("o",[]), new("u",[]) ]
        };

        var tact = new Phonotactics
        {
            allowedOnsets = new HashSet<string> { "p", "t", "k", "ʃ" },
            allowedCodas = new HashSet<string> { "m", "n", "s", "t", "k" }
        };

        // Fix: Cast to interface types for LangEngine constructor arguments
        Phonology phon = new Core.Language.Phonology.Phonology(inv, tact);
        Morphology morph = new Core.Language.Morphology.Morphology(new InMemoryLexicon());
        Syntax syn = new Core.Language.Syntax.Syntax(new SimpleSvoGenerator(new InMemoryLexicon()));
        Orthography orth = new Core.Language.Orthography.Orthography();
        Evaluation eval = new Core.Language.Evaluation.Evaluation();

        var engine = new LangEngine(
            (Phonology)phon,
            (Morphology)morph,
            (Syntax)syn,
            (Orthography)orth,
            (Evaluation)eval
        );
        var rng = new RNG(969);

        for (int i = 0; i < 5; i++)
        {
            var result = engine.GenerateSentence(rng, new SentenceSpec("declarative", 6, null));
            Console.WriteLine(result);
        }
    }
}
