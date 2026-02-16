using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Evaluation;

public interface ICheck
{
    string Name { get; }
    IEnumerable<string> run(IReadOnlyList<string> phones);
}

public sealed class IllegalClusterCheck : ICheck
{
    private readonly Phonology.Phonotactics _t;
    public IllegalClusterCheck(Phonology.Phonotactics t) => _t = t;
    public string Name => "IllegalClusters";

    public IEnumerable<string> run(IReadOnlyList<string> phones)
    {
        for (int i = 0; i < phones.Count - 1; i++)
        {
            var pair = phones[i] + phones[i + 1];
            if (!_t.IsValidOnset(pair) && !_t.IsValidCoda(pair))
                yield return $"Illegal cluster {pair} at {i}";
        }
    }
}

public sealed class Evaluation
{
    private readonly List<ICheck> _checks = new();
    public void Add(ICheck c) => _checks.Add(c);

    public void CheckAll(string surface)
    {
        var phones = surface.Select(ch => ch.ToString()).ToList();
        foreach (var c in _checks)
            foreach (var issue in c.run(phones))
                Console.WriteLine($"[Eval:{c.Name}] {issue}");
    }
}
