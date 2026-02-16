using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConlangBuilder.Core
{
    public record Phoneme(string Symbol, string Type, int Weight = 1, bool AllowOnset = true, bool AllowCoda = true)
    {
        public PhonologicalFeatures? Features { get; set; } = new();
    }

    public class Inventory
    {
        public List<Phoneme> Phonemes { get; set; } = new();
        public IEnumerable<Phoneme> Consonants => Phonemes.Where(p => p.Type == "C");
        public IEnumerable<Phoneme> Vowels => Phonemes.Where(p => p.Type == "V");
    }

    public record OrthographyMap(string Phoneme, string Grapheme);

    public class SyllableTemplate
    {
        public string Pattern { get; set; } = "CV";
        public int Weight { get; set; } = 1;
        public override string ToString() => $"{Pattern} (w={Weight})";
    }

    public enum AffixType { Prefix, Suffix }

    public class MorphRule
    {
        public AffixType Type { get; set; }
        public string Form { get; set; } = "";
        public string Meaning { get; set; } = "";
        public override string ToString() => $"{Type}: {Form} ({Meaning})";
    }

    public class Phonotactics
    {
        public List<string> AllowedOnsetClusters { get; set; } = new();
        public List<string> AllowedCodaClusters { get; set; } = new();
        public int MaxOnsetCluster { get; set; } = 2;
        public int MaxCodaCluster { get; set; } = 2;
        public List<string> ForbiddenPatterns { get; set; } = new();
    }

    public class GeneratorOptions
    {
        public bool HyphenateSyllables { get; set; } = false;
        public bool AllowDuplicates { get; set; } = true;
    }

    public class ConlangModel
    {
        public string ModelVersion { get; set; } = "5.0.0";
        public string Name { get; set; } = "My Conlang";
        public Inventory Inventory { get; set; } = new();
        public List<OrthographyMap> Orthography { get; set; } = new();
        public List<SyllableTemplate> Templates { get; set; } = new() { new SyllableTemplate { Pattern = "CV", Weight = 3 }, new SyllableTemplate { Pattern = "CVC", Weight = 2 } };
        public List<MorphRule> Morphology { get; set; } = new();
        public Phonotactics Phonotactics { get; set; } = new();
        public GeneratorOptions Options { get; set; } = new();
        public List<SoundRule> SoundRules { get; set; } = new();

        public string ToJson(bool indented = true)
        {
            var opts = new JsonSerializerOptions { WriteIndented = indented };
            return JsonSerializer.Serialize(this, opts);
        }

        public static ConlangModel FromJson(string json) =>
            JsonSerializer.Deserialize<ConlangModel>(json) ?? new ConlangModel();
    }

    public class WordGenerator
    {
        private readonly ConlangModel _model;
        private readonly Random _rng;

        public WordGenerator(ConlangModel model, int? seed = null)
        {
            _model = model;
            _rng = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public IEnumerable<string> GenerateWords(int count, int syllables)
        {
            var set = new HashSet<string>();
            int safety = 0;
            while (set.Count < count && safety < count * 300)
            {
                var w = GenerateWord(syllables);
                safety++;

                if (_model.Options.AllowDuplicates || !set.Contains(w))
                    set.Add(w);
            }
            return set;
        }

        public string GenerateWord(int syllables = 1)
        {
            var pieces = new List<string>();
            for (int i = 0; i < syllables; i++)
            {
                var tmpl = WeightedPick(_model.Templates, t => t.Weight);
                pieces.Add(RenderSyllablePhonemes(tmpl.Pattern));
            }
            string underlying = _model.Options.HyphenateSyllables ? string.Join("-", pieces) : string.Join("", pieces);

            if (_model.Morphology.Any() && _rng.NextDouble() < 0.2)
            {
                var affix = _model.Morphology[_rng.Next(_model.Morphology.Count)];
                underlying = affix.Type == AffixType.Prefix ? affix.Form + underlying : underlying + affix.Form;
            }

            foreach (var rule in _model.SoundRules.Where(r => r.Enabled))
                underlying = rule.Apply(underlying, _model);

            string orth = MapOrth(underlying);

            int tries = 0;
            while (!IsLegal(orth) && tries < 25)
            {
                underlying = GenerateWord(syllables);
                orth = MapOrth(underlying);
                tries++;
            }
            return orth;
        }

        private string RenderSyllablePhonemes(string pattern)
        {
            var outSb = new StringBuilder();
            bool optional = false;
            foreach (var ch in pattern)
            {
                if (ch == '(') { optional = true; continue; }
                if (ch == ')') { optional = false; continue; }
                if (optional && _rng.NextDouble() < 0.5) continue;

                switch (ch)
                {
                    case 'C':
                        var cons = PickWeighted(_model.Inventory.Consonants, p => p.Weight);
                        outSb.Append(cons?.Symbol ?? "");
                        break;
                    case 'V':
                        var vow = PickWeighted(_model.Inventory.Vowels, p => p.Weight);
                        outSb.Append(vow?.Symbol ?? "");
                        break;
                    default:
                        outSb.Append(ch);
                        break;
                }
            }
            return outSb.ToString();
        }

        private string MapOrth(string phonemic)
        {
            var maps = _model.Orthography.OrderByDescending(o => o.Phoneme.Length).ToList();
            string result = phonemic;
            foreach (var m in maps) result = result.Replace(m.Phoneme, m.Grapheme);
            return result;
        }

        private T WeightedPick<T>(IEnumerable<T> items, Func<T, int> weightSelector)
        {
            var list = items.ToList();
            if (list.Count == 0) throw new InvalidOperationException("No items to pick from.");
            var total = list.Sum(it => Math.Max(0, weightSelector(it)));
            if (total <= 0) total = list.Count;
            int r = _rng.Next(1, total + 1);
            int acc = 0;
            foreach (var it in list)
            {
                acc += Math.Max(0, weightSelector(it));
                if (r <= acc) return it;
            }
            return list.Last();
        }

        private Phoneme? PickWeighted(IEnumerable<Phoneme> items, Func<Phoneme, int> weightSelector)
        {
            var list = items.ToList();
            if (list.Count == 0) return null;
            return list[_rng.Next(list.Count)];
        }

        private bool IsLegal(string orthWord)
        {
            foreach (var pat in _model.Phonotactics.ForbiddenPatterns)
            {
                try { if (Regex.IsMatch(orthWord, pat)) return false; }
                catch { }
            }

            var vowels = _model.Inventory.Vowels.Select(v => MapOrth(v.Symbol)).OrderByDescending(s => s.Length).ToList();
            string VV = string.Join("|", vowels.Select(Regex.Escape));
            if (string.IsNullOrEmpty(VV)) VV = "[aeiou]";

            var parts = Regex.Split(orthWord, $"({VV})");
            for (int i = 0; i < parts.Length; i++)
            {
                var seg = parts[i];
                if (string.IsNullOrEmpty(seg)) continue;
                bool isConsonantRun = !Regex.IsMatch(seg, $"^{VV}$");
                if (!isConsonantRun) continue;

                bool isOnset = (i == 0 || parts[i-1] == "-" );
                var clusters = AllClusters(seg);
                int maxLen = isOnset ? _model.Phonotactics.MaxOnsetCluster : _model.Phonotactics.MaxCodaCluster;
                if (clusters.Any(c => c.Length > maxLen)) return false;

                var allowedSet = isOnset ? _model.Phonotactics.AllowedOnsetClusters : _model.Phonotactics.AllowedCodaClusters;
                if (allowedSet.Count > 0 && !clusters.All(c => allowedSet.Contains(c)))
                    return false;
            }
            return true;
        }

        private static List<string> AllClusters(string s)
        {
            var res = new List<string>();
            for (int len = 1; len <= s.Length; len++)
                for (int i = 0; i + len <= s.Length; i++)
                    res.Add(s.Substring(i, len));
            return res;
        }
    }
}