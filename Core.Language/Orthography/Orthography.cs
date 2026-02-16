using System;

namespace Core.Language.Orthography;

public sealed class Orthography
{
    public string Render(string surfacePhones, Random rng)
    {
        return surfacePhones.Replace(" ", "");
    }
}
