using System;
using System.IO;
using ConlangBuilder.Core;

namespace UI.WPF.Project;

public sealed class PluginLabContext : ILabContext
{
    private readonly Action<string> _log;
    private readonly HashSet<string> _allowedCapabilities;

    public PluginLabContext(string appDataDir, Action<string> log, IEnumerable<string> allowedCapabilities)
    {
        AppDataDir = appDataDir;
        _log = log;
        _allowedCapabilities = new HashSet<string>(allowedCapabilities, StringComparer.OrdinalIgnoreCase);
    }

    public string AppDataDir { get; }

    public void Log(string message) => _log(message);

    public Stream CreateOutput(string suggestedFileName)
    {
        if (!_allowedCapabilities.Contains("OutputFiles"))
            throw new InvalidOperationException("Plugin lacks OutputFiles capability.");

        var outputsDir = Path.Combine(AppDataDir, "plugin-outputs");
        Directory.CreateDirectory(outputsDir);
        var safeName = string.IsNullOrWhiteSpace(suggestedFileName) ? "plugin-output.txt" : Path.GetFileName(suggestedFileName);
        var path = Path.Combine(outputsDir, safeName);
        _log($"Plugin output created: {path}");
        return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    }
}
