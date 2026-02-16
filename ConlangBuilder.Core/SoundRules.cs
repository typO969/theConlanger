using System.Linq;
using System.Text.RegularExpressions;

namespace ConlangBuilder.Core
{
    public class SoundRule
    {
        public bool Enabled { get; set; } = true;
        public string Input { get; set; } = "";
        public string Output { get; set; } = "";
        public string Environment { get; set; } = "";
        public string Condition { get; set; } = "";

        public override string ToString() => $"{(Enabled ? "" : "⦸ ")}{Input} → {Output} / {Environment} {Condition}".Trim();

        public string Apply(string word, ConlangModel model)
        {
            if (!Enabled || string.IsNullOrWhiteSpace(Input)) return word;

            string pattern = Regex.Escape(Input);
            string env = Environment ?? "";
            string before = "";
            string after = "";

            if (env.Contains("_"))
            {
                var parts = env.Split('_');
                before = parts.Length > 0 ? parts[0] : "";
                after  = parts.Length > 1 ? parts[1] : "";
            }

            string Cclass = string.Join("|", model.Inventory.Phonemes.Where(p => p.Type == "C").Select(p => Regex.Escape(p.Symbol)).OrderByDescending(s => s.Length));
            if (string.IsNullOrEmpty(Cclass)) Cclass = "[^aeiou]";
            string Vclass = string.Join("|", model.Inventory.Phonemes.Where(p => p.Type == "V").Select(p => Regex.Escape(p.Symbol)).OrderByDescending(s => s.Length));
            if (string.IsNullOrEmpty(Vclass)) Vclass = "[aeiou]";

            string EnvToRegex(string e)
            {
                if (string.IsNullOrEmpty(e)) return "";
                e = Regex.Replace(e, @"\bC\b", $"(?:{Cclass})");
                e = Regex.Replace(e, @"\bV\b", $"(?:{Vclass})");
                return e;
            }

            string left  = EnvToRegex(before);
            string right = EnvToRegex(after);

            string finalPattern = pattern;
            if (!string.IsNullOrEmpty(left))  finalPattern = $"(?<={left})" + finalPattern;
            if (!string.IsNullOrEmpty(right)) finalPattern = finalPattern + $"(?={right})";

            if (!string.IsNullOrWhiteSpace(Condition))
            {
                return Regex.Replace(word, finalPattern, m =>
                {
                    string matched = m.Value;
                    return MatchesCondition(matched, model) ? Output : matched;
                });
            }
            else
            {
                return Regex.Replace(word, finalPattern, Output);
            }
        }

        private bool MatchesCondition(string symbol, ConlangModel model)
        {
            if (string.IsNullOrWhiteSpace(Condition)) return true;
            var p = model.Inventory.Phonemes.FirstOrDefault(x => x.Symbol == symbol);
            if (p == null) return false;
            var feat = p.Features ?? new PhonologicalFeatures();

            string cond = Condition.Trim();
            if (!cond.StartsWith("[") || !cond.EndsWith("]")) return true;
            cond = cond.Trim('[', ']');

            if (cond.StartsWith("+") || cond.StartsWith("-"))
            {
                bool positive = cond.StartsWith("+");
                var name = cond.Substring(1).ToLowerInvariant();
                if (name == "voice" || name == "voicing")
                {
                    var v = (feat.Voicing ?? "").ToLowerInvariant();
                    return positive ? v == "voiced" : v == "voiceless";
                }
                var custom = (feat.Custom ?? "").ToLowerInvariant();
                return positive ? custom.Contains(name) : !custom.Contains(name);
            }
            else if (cond.Contains("="))
            {
                var parts = cond.Split('=', 2);
                var key = parts[0].Trim().ToLowerInvariant();
                var val = parts[1].Trim().ToLowerInvariant();
                string? featVal = key switch
                {
                    "voicing" => feat.Voicing,
                    "manner" => feat.Manner,
                    "place" => feat.Place,
                    "height" => feat.Height,
                    "backness" => feat.Backness,
                    "roundness" => feat.Roundness,
                    _ => feat.Custom
                };
                return (featVal ?? "").ToLowerInvariant().Contains(val);
            }
            return true;
        }
    }
}