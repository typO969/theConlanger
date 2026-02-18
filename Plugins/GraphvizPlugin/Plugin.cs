using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConlangBuilder.Core;

namespace GraphvizPlugin;

public sealed class GraphvizPlugin : IPlugin
{
    private ILabContext? _context;

    public string Id => "graphviz";
    public string Name => "Graphviz Export";
    public string Version => "1.0.0";
    public IEnumerable<IPluginAction> Actions => new IPluginAction[] { new ExportDotAction(() => _context) };

    public void Initialize(ILabContext ctx) => _context = ctx;

    private sealed class ExportDotAction : IPluginAction
    {
        private readonly System.Func<ILabContext?> _context;

        public ExportDotAction(System.Func<ILabContext?> context) => _context = context;

        public string ActionId => "export-dot";
        public string Label => "Export sound rules as Graphviz";
        public string Category => "Export";
        public IReadOnlyList<PluginArgumentDefinition> Arguments => new[]
        {
            new PluginArgumentDefinition("fileName", "File name", "string", "sound-rules.dot", true, "Output file name for the DOT graph.")
        };

        public Task<PluginResult> RunAsync(ConlangModel model, IDictionary<string, object?> args, CancellationToken ct)
        {
            var ctx = _context();
            if (ctx is null)
                return Task.FromResult(new PluginResult(false, "Plugin not initialized."));

            var fileName = args.TryGetValue("fileName", out var value) ? value?.ToString() : "sound-rules.dot";
            using var stream = ctx.CreateOutput(fileName ?? "sound-rules.dot");
            using var writer = new StreamWriter(stream);
            var dot = Exporters.ToGraphvizDot(model);
            writer.Write(dot);
            writer.Flush();
            return Task.FromResult(new PluginResult(true, $"Exported {fileName}"));
        }
    }
}
