using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConlangBuilder.Core;

namespace UI.WPF.Project;

public sealed class PluginCatalog
{
    public IReadOnlyList<PluginDescriptor> Plugins { get; private set; } = Array.Empty<PluginDescriptor>();
    public IReadOnlyList<IPlugin> LoadedPlugins { get; private set; } = Array.Empty<IPlugin>();
    public IReadOnlyList<PluginLoadResult> LoadResults { get; private set; } = Array.Empty<PluginLoadResult>();
    public IReadOnlyList<string> DiscoveredPaths { get; private set; } = Array.Empty<string>();

    public void Refresh(string pluginDirectory, ILabContext context, IReadOnlyCollection<string> allowedCapabilities)
    {
        DiscoveredPaths = Directory.Exists(pluginDirectory)
            ? Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories)
            : Array.Empty<string>();

        LoadResults = PluginLoader.LoadAll(pluginDirectory, context);
        LoadedPlugins = LoadResults.Select(r => r.Plugin).ToList();
        Plugins = LoadResults
            .Select(r => CreateDescriptor(r.Path, r.Plugin, allowedCapabilities))
            .OrderBy(p => p.Name)
            .ToList();
    }

    private static PluginDescriptor CreateDescriptor(string path, IPlugin plugin, IReadOnlyCollection<string> allowedCapabilities)
    {
        var manifest = PluginManifest.LoadForAssemblyPath(path);
        var id = manifest?.Id ?? plugin.Id;
        var name = manifest?.Name ?? plugin.Name;
        var version = manifest?.Version ?? plugin.Version;
        var author = manifest?.Author;
        var description = manifest?.Description;
        var website = manifest?.Website;
        var isolation = string.IsNullOrWhiteSpace(manifest?.Isolation) ? "InProcess" : manifest!.Isolation!;
        var capabilities = manifest?.Capabilities?.ToList() ?? new List<string>();
        var missing = capabilities
            .Where(c => !allowedCapabilities.Contains(c, StringComparer.OrdinalIgnoreCase))
            .ToList();
        var isBlocked = missing.Count > 0;
        var status = isBlocked
            ? $"Blocked: {string.Join(", ", missing)}"
            : "Loaded";
        return new PluginDescriptor(id, name, version, path, status, author, description, website, capabilities, isolation, isBlocked);
    }
}

public sealed record PluginDescriptor(
    string Id,
    string Name,
    string Version,
    string Path,
    string Status,
    string? Author,
    string? Description,
    string? Website,
    IReadOnlyList<string> Capabilities,
    string IsolationMode,
    bool IsBlocked)
{
    public string CapabilitiesDisplay => Capabilities.Count == 0 ? "(none)" : string.Join(", ", Capabilities);
}
