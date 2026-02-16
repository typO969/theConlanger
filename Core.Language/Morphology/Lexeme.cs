using System.Collections.Generic;

namespace Core.Language.Morphology;

public record Lexeme(string Lemma, string Pos, Dictionary<string, string> rootPhones);
