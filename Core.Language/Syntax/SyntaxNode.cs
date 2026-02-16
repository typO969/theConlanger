using System.Collections.Generic; // Fixes CS0246 for List<>
using System.Linq; // Fixes CS1061 for FirstOrDefault
using Core.Language.Shared;

namespace Core.Language.Syntax;

public sealed class SyntaxNode
{
    public string Pos { get; set; } = "N";
    public string LemmaId { get; set; } = "";
    public FeatBundle features { get; set; } = new(new List<Feat>());
    public List<SyntaxNode> Children { get; } = new();

    public IEnumerable<SyntaxNode> InOrder()
    {
        if (Pos == "V")
        {
            var subj = Children.FirstOrDefault(c => c.features.Has("Role","Subj"));
            var obj  = Children.FirstOrDefault(c => c.features.Has("Role","Obj"));
            if (subj!=null) foreach (var n in subj.InOrder()) yield return n;
            yield return this;
            if (obj!=null) foreach (var n in obj.InOrder()) yield return n;
        }
        else yield return this;
    }
}
