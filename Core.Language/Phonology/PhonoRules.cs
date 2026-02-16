using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Language.Phonology;

public interface IPhonoRule
{
    string Name { get; }
    bool Enabled { get; set; }
    IReadOnlyList<string> apply(IReadOnlyList<string> phones);
}

public sealed class RewriteRule : IPhonoRule
{
    private readonly Func<string, bool> _match;
    private readonly Func<IReadOnlyList<string>, int, bool> _env;
    private readonly Func<string, string> _map;

    public string Name { get; }
    public bool Enabled { get; set; } = true;

    public RewriteRule(
        string name,
        Func<string, bool> match,
        Func<IReadOnlyList<string>, int, bool> env,
        Func<string, string> map)
    {
        Name = name;
        _match = match;
        _env = env;
        _map = map;
    }

    public IReadOnlyList<string> apply(IReadOnlyList<string> phones)
    {
        if (!Enabled)
            return phones;

        var output = new List<string>(phones);
        for (var i = 0; i < phones.Count; i++)
        {
            if (_match(phones[i]) && _env(phones, i))
                output[i] = _map(phones[i]);
        }

        return output;
    }
}

public sealed class RuleSpec
{
    public string Name { get; set; } = "Rule";
    public string Target { get; set; } = "s";
    public string Replacement { get; set; } = "z";
    public string? PrevPhone { get; set; }
    public string? NextPhone { get; set; }
    public bool Enabled { get; set; } = true;

    public RewriteRule ToRule() => new(
        Name,
        phone => phone == Target,
        (phones, i) =>
        {
            var prevOk = string.IsNullOrWhiteSpace(PrevPhone) || (i > 0 && phones[i - 1] == PrevPhone);
            var nextOk = string.IsNullOrWhiteSpace(NextPhone) || (i < phones.Count - 1 && phones[i + 1] == NextPhone);
            return prevOk && nextOk;
        },
        _ => Replacement)
    { Enabled = Enabled };

    public override string ToString()
    {
        var ctx = $"{(string.IsNullOrWhiteSpace(PrevPhone) ? "_" : PrevPhone)} __ {(string.IsNullOrWhiteSpace(NextPhone) ? "_" : NextPhone)}";
        return $"{Name}: {Target}→{Replacement} / {ctx}";
    }
}
