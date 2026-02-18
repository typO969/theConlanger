using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConlangBuilder.Core;

namespace EvolutionSimPlugin;

public sealed class EvolutionSimPlugin : IPlugin
{
    private ILabContext? _context;

    public string Id => "evolution-sim";
    public string Name => "Evolution Simulator";
    public string Version => "1.0.0";
    public IEnumerable<IPluginAction> Actions => new IPluginAction[] { new GenerateSamplesAction(() => _context) };

    public void Initialize(ILabContext ctx) => _context = ctx;

    private sealed class GenerateSamplesAction : IPluginAction
    {
        private readonly System.Func<ILabContext?> _context;

        public GenerateSamplesAction(System.Func<ILabContext?> context) => _context = context;

        public string ActionId => "generate-samples";
        public string Label => "Generate sample words";
        public string Category => "Generate";
        public IReadOnlyList<PluginArgumentDefinition> Arguments => new[]
        {
            new PluginArgumentDefinition("count", "Count", "int", "10", true, "Number of samples to generate."),
            new PluginArgumentDefinition("syllables", "Syllables", "int", "2", true, "Syllables per word."),
            new PluginArgumentDefinition("fileName", "File name", "string", "samples.txt", true, "Output file name for the samples."),
            new PluginArgumentDefinition("format", "Format", "string", "txt", false, "Output format.", new[] { "txt", "csv" })
        };

        public Task<PluginResult> RunAsync(ConlangModel model, IDictionary<string, object?> args, CancellationToken ct)
        {
            var ctx = _context();
            if (ctx is null)
                return Task.FromResult(new PluginResult(false, "Plugin not initialized."));

            var count = args.TryGetValue("count", out var countVal) && countVal is int c ? c : 10;
            var syllables = args.TryGetValue("syllables", out var sylVal) && sylVal is int s ? s : 2;
            var fileName = args.TryGetValue("fileName", out var fileVal) ? fileVal?.ToString() : "samples.txt";
            var format = args.TryGetValue("format", out var fmtVal) ? fmtVal?.ToString() : "txt";

            var generator = new WordGenerator(model);
            var words = generator.GenerateWords(count, syllables).ToList();

            using var stream = ctx.CreateOutput(fileName ?? "samples.txt");
            using var writer = new StreamWriter(stream);
            if (string.Equals(format, "csv", System.StringComparison.OrdinalIgnoreCase))
                writer.WriteLine(string.Join(",", words));
            else
            {
                foreach (var word in words)
                    writer.WriteLine(word);
            }

            writer.Flush();
            return Task.FromResult(new PluginResult(true, $"Generated {words.Count} samples to {fileName}"));
        }
    }
}
