using System.Collections.Generic;

namespace Core.Language.Phonology;

public record Syllable(IReadOnlyList<string> onset,
                       IReadOnlyList<string> nucleus,
                       IReadOnlyList<string> coda);

public sealed class Syllabifier
{
    private readonly Phonotactics _t;
    public Syllabifier(Phonotactics t) => _t = t;

    private static bool IsVowel(string p)
        => "aeiouɑɔɛɪʊyøœ".Contains(p[0]);

    public List<Syllable> Split(IReadOnlyList<string> phones)
    {
        var syllables = new List<Syllable>();
        var onset = new List<string>();
        var nucleus = new List<string>();
        var coda = new List<string>();

        foreach (var p in phones)
        {
            if (IsVowel(p))
            {
                if (nucleus.Count > 0)
                {
                    syllables.Add(new Syllable(onset, nucleus, coda));
                    onset = new(); nucleus = new(); coda = new();
                }
                nucleus.Add(p);
            }
            else
            {
                if (nucleus.Count == 0) onset.Add(p);
                else coda.Add(p);
            }
        }
        syllables.Add(new Syllable(onset, nucleus, coda));
        return syllables;
    }
}
