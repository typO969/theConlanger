using System;
using System.Collections.Generic;

namespace Core.Language.Phonology;

public interface IPhonoRule
{
    IReadOnlyList<string> apply(IReadOnlyList<string> phones);
}

public sealed class RewriteRule : IPhonoRule
{
    private readonly Func<string,bool> _match;
    private readonly Func<IReadOnlyList<string>,int,bool> _env;
    private readonly Func<string,string> _map;

    public RewriteRule(Func<string,bool> match,
                       Func<IReadOnlyList<string>,int,bool> env,
                       Func<string,string> map)
    { _match = match; _env = env; _map = map; }

    public IReadOnlyList<string> apply(IReadOnlyList<string> phones)
    {
        var outp = new List<string>(phones);
        for (int i=0;i<phones.Count;i++)
            if (_match(phones[i]) && _env(phones,i))
                outp[i] = _map(phones[i]);
        return outp;
    }
}
