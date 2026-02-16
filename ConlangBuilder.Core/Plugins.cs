using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConlangBuilder.Core
{
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        IEnumerable<IPluginAction> Actions { get; }
        void Initialize(ILabContext ctx);
    }

    public interface IPluginAction
    {
        string ActionId { get; }
        string Label { get; }
        string Category { get; }
        Task<PluginResult> RunAsync(ConlangModel model, IDictionary<string,object?> args, CancellationToken ct);
    }

    public record PluginResult(bool Success, string Message, System.Collections.Generic.Dictionary<string, object?>? Payload = null);

    public interface ILabContext
    {
        string AppDataDir { get; }
        void Log(string message);
        System.IO.Stream CreateOutput(string suggestedFileName);
    }
}