using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Language.Morphology;

namespace UI.WPF.Project;

public static class LexiconPorter
{
    public static IReadOnlyCollection<Lexeme> Import(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext == ".json")
        {
            var raw = File.ReadAllText(path);
            var parsed = JsonSerializer.Deserialize<List<LexemeCsvRow>>(raw) ?? new List<LexemeCsvRow>();
            return parsed
                .Where(r => !string.IsNullOrWhiteSpace(r.Lemma))
                .Select(r => new Lexeme(r.Lemma.Trim(), string.IsNullOrWhiteSpace(r.Pos) ? "N" : r.Pos.Trim(), new() { ["UR"] = (r.UR ?? "").Trim() }))
                .ToList();
        }

        var lines = File.ReadAllLines(path).Skip(1);
        var rows = lines
            .Select(line => line.Split(','))
            .Where(parts => parts.Length >= 3)
            .Select(parts => new Lexeme(parts[0].Trim(), parts[1].Trim(), new() { ["UR"] = parts[2].Trim() }))
            .ToList();

        return rows;
    }

    public static void Export(string path, IReadOnlyCollection<Lexeme> lexemes)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext == ".json")
        {
            var rows = lexemes.Select(l => new LexemeCsvRow(l.Lemma, l.Pos, l.rootPhones.TryGetValue("UR", out var ur) ? ur : ""));
            var json = JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            return;
        }

        var lines = new List<string> { "Lemma,Pos,UR" };
        lines.AddRange(lexemes.Select(l => $"{l.Lemma},{l.Pos},{l.rootPhones.GetValueOrDefault("UR", "")}"));
        File.WriteAllLines(path, lines);
    }

    private sealed record LexemeCsvRow(string Lemma, string Pos, string UR);
}
