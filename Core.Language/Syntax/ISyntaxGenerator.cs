using System; // Fixes CS0246 for Random
using Core.Language.Shared;

namespace Core.Language.Syntax;

public interface ISyntaxGenerator
{
    SyntaxTree realize(SentenceSpec spec, Random rng); // Fixes IDE1006: method name should start with lower case
}
