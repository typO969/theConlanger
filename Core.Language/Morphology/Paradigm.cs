using System;
using System.Collections.Generic;
using Core.Language.Morphology; // Adjust this if FeatBundle is in a different namespace
using Core.Language.Shared; // <-- Add this line if FeatBundle is defined there

namespace Core.Language.Morphology;

public record Paradigm(string Pos,
                       Func<FeatBundle,bool> slotPredicate,
                       Func<Lexeme,FeatBundle,IEnumerable<Morpheme>> Realizer);