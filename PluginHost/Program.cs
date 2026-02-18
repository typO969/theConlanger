using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConlangBuilder.Core;

static string? GetArg(string[] args, string name)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            return args[i + 1];
    }
    return null;
}

static Dictionary<string, object?> ReadArgs(string path)
{
    var json = File.ReadAllText(path);
    var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? new Dictionary<string, JsonElement>();
    var result = new Dictionary<string, object?>();
    foreach (var pair in raw)
    {
        result[pair.Key] = pair.Value.ValueKind switch
        {
            JsonValueKind.Number when pair.Value.TryGetInt32(out var i) => i,
            JsonValueKind.Number when pair.Value.TryGetDouble(out var d) => d,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.String => pair.Value.GetString(),
            JsonValueKind.Null => null,
            _ => pair.Value.ToString()
        };
    }
    return result;
}

static PluginResult Result(bool success, string message) => new(success, message);

var pluginPath = GetArg(args, "--plugin");
var actionId = GetArg(args, "--action");
var modelPath = GetArg(args, "--model");
var argsPath = GetArg(args, "--args");
var resultPath = GetArg(args, "--result");
var outputDir = GetArg(args, "--outputDir");
var timeoutMsRaw = GetArg(args, "--timeoutMs");

if (string.IsNullOrWhiteSpace(pluginPath) || string.IsNullOrWhiteSpace(actionId) || string.IsNullOrWhiteSpace(modelPath) || string.IsNullOrWhiteSpace(argsPath) || string.IsNullOrWhiteSpace(resultPath))
{
    Console.Error.WriteLine("Missing required arguments.");
    return;
}

var timeout = 10000;
if (int.TryParse(timeoutMsRaw, out var parsedTimeout))
    timeout = parsedTimeout;

var outputRoot = string.IsNullOrWhiteSpace(outputDir)
    ? Path.Combine(Path.GetTempPath(), "ConlangBuilder", "plugin-host")
    : outputDir;
Directory.CreateDirectory(outputRoot);

PluginResult result;
try
{
    var modelJson = File.ReadAllText(modelPath);
    var model = ConlangModel.FromJson(modelJson);
    var argValues = ReadArgs(argsPath);

    var ctx = new HostLabContext(outputRoot, Console.WriteLine);
    var plugin = LoadPlugin(pluginPath, ctx);
    if (plugin is null)
        result = Result(false, "No plugin found in assembly.");
    else
    {
        var action = plugin.Actions.FirstOrDefault(a => string.Equals(a.ActionId, actionId, StringComparison.OrdinalIgnoreCase));
        if (action is null)
            result = Result(false, "Action not found.");
        else
        {
            using var cts = new CancellationTokenSource(timeout);
            result = await action.RunAsync(model, argValues, cts.Token);
        }
    }
}
catch (Exception ex)
{
    result = Result(false, ex.Message);
}

var resultJson = JsonSerializer.Serialize(result);
File.WriteAllText(resultPath, resultJson);

static IPlugin? LoadPlugin(string pluginPath, ILabContext context)
{
    var alc = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(pluginPath), isCollectible: true);
    using var fs = new FileStream(pluginPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    var asm = alc.LoadFromStream(fs);
    foreach (var t in asm.GetExportedTypes())
    {
        if (typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)
        {
            if (Activator.CreateInstance(t) is IPlugin plugin)
            {
                plugin.Initialize(context);
                return plugin;
            }
        }
    }
    return null;
}

sealed class HostLabContext : ILabContext
{
    private readonly Action<string> _log;

    public HostLabContext(string appDataDir, Action<string> log)
    {
        AppDataDir = appDataDir;
        _log = log;
    }

    public string AppDataDir { get; }

    public void Log(string message) => _log(message);

    public Stream CreateOutput(string suggestedFileName)
    {
        Directory.CreateDirectory(AppDataDir);
        var safeName = string.IsNullOrWhiteSpace(suggestedFileName) ? "plugin-output.txt" : Path.GetFileName(suggestedFileName);
        var path = Path.Combine(AppDataDir, safeName);
        _log($"Output: {path}");
        return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
    }
}
