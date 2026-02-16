using System.IO;
using System.Text;

namespace ConlangBuilder.Core
{
    public static class Exporters
    {
        public static void ToCsvFeatures(ConlangModel m, string path)
        {
            using var w = new StreamWriter(path,false,Encoding.UTF8);
            w.WriteLine("Symbol,Type,Voicing,Manner,Place,Height,Backness,Roundness,Custom");
            foreach (var p in m.Inventory.Phonemes)
            {
                var f = p.Features ?? new PhonologicalFeatures();
                string Q(string? s) => $"\"{(s ?? "").Replace("\"","\"\"")}\"";
                w.WriteLine($"{Q(p.Symbol)},{Q(p.Type)},{Q(f.Voicing)},{Q(f.Manner)},{Q(f.Place)},{Q(f.Height)},{Q(f.Backness)},{Q(f.Roundness)},{Q(f.Custom)}");
            }
        }

        public static string ToGraphvizDot(ConlangModel m)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("digraph Rules { rankdir=LR; node [shape=box];");
            sb.AppendLine("INPUT [shape=oval];");
            string prev="INPUT"; int i=0;
            foreach (var r in m.SoundRules)
            {
                string id=$"R{i++}";
                string lbl=$"{r.Input}→{r.Output}\\n/{r.Environment}\\n{r.Condition}";
                sb.AppendLine($"{id} [label=\"{lbl}\"]; {prev} -> {id};");
                prev=id;
            }
            sb.AppendLine($"{prev} -> OUTPUT; OUTPUT [shape=oval];");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}