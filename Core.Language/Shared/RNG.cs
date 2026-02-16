using System;

namespace Core.Language.Shared;
public sealed class RNG : Random
{
    public RNG(int seed) : base(seed) { }
}
