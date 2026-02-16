using System.Collections.Generic;

namespace Core.Language.Phonology;

public record Phoneme(string Symbol, string[] Features);

public sealed class PhonemeInventory
{
    public IReadOnlyList<Phoneme> consonants { get; init; } = [];
    public IReadOnlyList<Phoneme> vowels { get; init; } = [];
}
