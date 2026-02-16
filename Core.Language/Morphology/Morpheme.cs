using System;
using System.Collections.Generic;
using Core.Language.Shared;

namespace Core.Language.Morphology;

public record Morpheme(string Id, string Gloss,
                       IReadOnlyList<string> underlyingPhones,
                       Func<FeatBundle,bool> usageCondition);

public record MorphemeToken(Morpheme Morpheme,
                            IReadOnlyList<string> underlyingPhones);