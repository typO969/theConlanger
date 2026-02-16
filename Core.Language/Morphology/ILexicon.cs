using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Morphology;

public interface ILexicon
{
    Lexeme Resolve(string lemmaId);
    IReadOnlyCollection<Lexeme> GetAll();
    void ReplaceAll(IEnumerable<Lexeme> lexemes);
}

public sealed class InMemoryLexicon : ILexicon
{
    private readonly Dictionary<string, Lexeme> _dict;

    public InMemoryLexicon()
    {
        _dict = new()
        {
            ["person"] = new("person", "N", new() { ["UR"] = "p e r s o n" }),
            ["fish"] = new("fish", "N", new() { ["UR"] = "f i ʃ" }),
            ["see"] = new("see", "V", new() { ["UR"] = "s i" }),
            ["small"] = new("small", "ADJ", new() { ["UR"] = "s m a l" })
        };
    }

    public Lexeme Resolve(string lemmaId)
    {
        if (_dict.TryGetValue(lemmaId, out var lexeme))
            return lexeme;

        return _dict.Values.First();
    }

    public IReadOnlyCollection<Lexeme> GetAll() => _dict.Values.ToArray();

    public void ReplaceAll(IEnumerable<Lexeme> lexemes)
    {
        _dict.Clear();
        foreach (var lexeme in lexemes)
            _dict[lexeme.Lemma] = lexeme;
    }
}
