using Core.Language.Morphology;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Phonology;

public sealed class Phonology
{
    public PhonemeInventory Inventory { get; }
    public Phonotactics Tactics { get; }
    public Syllabifier Syllabifier { get; }
    public List<IPhonoRule> Rules { get; } = new();

    public Phonology(PhonemeInventory inv, Phonotactics tact)
    { Inventory = inv; Tactics = tact; Syllabifier = new Syllabifier(tact); }

    public string Surface(IEnumerable<MorphemeToken> morphemes, Random rng)
    {
        var underlying = morphemes.SelectMany<MorphemeToken, string>(m => m.underlyingPhones).ToList();
        IReadOnlyList<string> cur = underlying;
        foreach (var r in Rules) cur = r.apply(cur); // Changed 'Apply' to 'apply' to match interface
        return string.Join("", cur);
    }
}