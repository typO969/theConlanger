using System.Collections.Generic;

namespace Core.Language.Phonology;

public sealed class Phonotactics
{
    public HashSet<string> allowedOnsets { get; init; } = [];
    public HashSet<string> allowedCodas { get; init; } = [];

    public bool IsValidOnset(string cluster) => allowedOnsets.Contains(cluster);
    public bool IsValidCoda(string cluster)  => allowedCodas.Contains(cluster);
}
