using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ConlangBuilder.Core
{
    public static class PluginLoader
    {
        public static System.Collections.Generic.List<IPlugin> LoadAll(string pluginDir, ILabContext ctx)
        {
            var plugins = new System.Collections.Generic.List<IPlugin>();
            if (!Directory.Exists(pluginDir)) return plugins;
            foreach (var dll in Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.AllDirectories))
            {
                try {
                    var alc = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(dll), isCollectible: true);
                    using var fs = new FileStream(dll, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var asm = alc.LoadFromStream(fs);
                    foreach (var t in asm.GetExportedTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
                        {
                            if (System.Activator.CreateInstance(t) is IPlugin p) { p.Initialize(ctx); plugins.Add(p); }
                        }
                    }
                } catch (System.Exception ex) { ctx.Log($"Failed to load {dll}: {ex.Message}"); }
            }
            return plugins;
        }
    }
}