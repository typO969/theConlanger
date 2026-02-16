using Core.Language.Shared;
using System; // Add this using directive

namespace Core.Language.Syntax;

public sealed class Syntax
{
    private readonly ISyntaxGenerator _gen;
    public Syntax(ISyntaxGenerator gen) => _gen = gen;
    public SyntaxTree realize(SentenceSpec spec, System.Random rng) => _gen.realize(spec, rng); // Change to lowercase 'realize'
}
