using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UI.WPF.Project;

public sealed class PluginCatalog
{
    public IReadOnlyList<PluginDescriptor> Plugins { get; private set; } = Array.Empty<PluginDescriptor>();

    public void Refresh(string pluginDirectory)
    {
        if (!Directory.Exists(pluginDirectory))
        {
            Plugins = Array.Empty<PluginDescriptor>();
            return;
        }

        Plugins = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Select(path => new PluginDescriptor(Path.GetFileNameWithoutExtension(path), path, "Discovered (execution integration deferred to Phase 4)."))
            .OrderBy(p => p.Name)
            .ToList();
    }
}

public sealed record PluginDescriptor(string Name, string Path, string Status);
