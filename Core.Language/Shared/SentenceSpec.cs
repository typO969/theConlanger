using System.Collections.Generic;

namespace Core.Language.Shared;
public record SentenceSpec(string ClauseType, int ApproxWords, IReadOnlyDictionary<string,string>? constraints);
