using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Shared;

public record Feat(string Name, string Value);
public record FeatBundle(IReadOnlyList<Feat> items)
{
    public bool Has(string name, string val) => items.Any(f => f.Name==name && f.Value==val);
}