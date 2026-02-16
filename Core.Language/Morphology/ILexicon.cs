using System.Collections.Generic;
using System.Linq; // Add this for LINQ extension methods

namespace Core.Language.Morphology;

public interface ILexicon
{
    Lexeme Resolve(string lemmaId);
}

public sealed class InMemoryLexicon : ILexicon
{
    private readonly Dictionary<string,Lexeme> _dict;

    public InMemoryLexicon()
    {
        _dict = new()
        {
            ["person"] = new("person","N", new(){{"UR","p e r s o n"}}),
            ["fish"]   = new("fish","N", new(){{"UR","f i ʃ"}}),
            ["see"]    = new("see","V",  new(){{"UR","s i"}})
        };
    }

    public Lexeme Resolve(string lemmaId)
    {
        if (_dict.TryGetValue(lemmaId, out var lexeme))
            return lexeme;
        return _dict.Values.First();
    }
}
