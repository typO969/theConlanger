using System.Collections.Generic;

namespace Core.Language.Syntax;

public sealed class SyntaxTree
{
    public SyntaxNode Root { get; init; } = new();
    public IEnumerable<SyntaxNode> linearize() => Root.InOrder();
}
