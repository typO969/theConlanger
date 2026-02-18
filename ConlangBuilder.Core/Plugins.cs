using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
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
        IReadOnlyList<PluginArgumentDefinition> Arguments => Array.Empty<PluginArgumentDefinition>();
        Task<PluginResult> RunAsync(ConlangModel model, IDictionary<string,object?> args, CancellationToken ct);
    }

    public record PluginResult(bool Success, string Message, System.Collections.Generic.Dictionary<string, object?>? Payload = null);

    public interface ILabContext
    {
        string AppDataDir { get; }
        void Log(string message);
        System.IO.Stream CreateOutput(string suggestedFileName);
    }

    public sealed record PluginArgumentDefinition(
        string Name,
        string Label,
        string Type,
        string? DefaultValue = null,
        bool Required = false,
        string? Description = null,
        IReadOnlyList<string>? Options = null);

    public sealed record PluginManifest(
        string Id,
        string Name,
        string Version,
        string? Author = null,
        string? Description = null,
        string? Website = null,
        string? Isolation = null,
        string[]? Capabilities = null)
    {
        public static PluginManifest? LoadForAssemblyPath(string assemblyPath)
        {
            var manifestPath = Path.ChangeExtension(assemblyPath, ".plugin.json");
            if (!File.Exists(manifestPath))
                return null;

            var json = File.ReadAllText(manifestPath);
            return JsonSerializer.Deserialize<PluginManifest>(json);
        }
    }
}