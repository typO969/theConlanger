using System.Collections.Generic;
using Core.Language.Shared;

namespace Core.Language.Syntax;

public sealed class SyntaxNode
{
    public string Pos { get; set; } = "N";
    public string LemmaId { get; set; } = "";
    public FeatBundle features { get; set; } = new([]);
    public List<SyntaxNode> Children { get; } = new();

    public IEnumerable<SyntaxNode> InOrder()
    {
        if (Pos == "CLAUSE")
        {
            foreach (var child in Children)
            {
                foreach (var node in child.InOrder())
                    yield return node;
            }

            yield break;
        }

        yield return this;
    }
}
