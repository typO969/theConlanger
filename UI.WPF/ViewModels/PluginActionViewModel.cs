using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ConlangBuilder.Core;

namespace UI.WPF.ViewModels;

public sealed class PluginActionViewModel : ViewModelBase
{
    private readonly IPlugin _plugin;
    private readonly IPluginAction _action;
    private readonly Func<ConlangModel> _modelFactory;
    private readonly Action<string> _log;
    private readonly TimeSpan _timeout;
    private readonly bool _runOutOfProcess;
    private readonly bool _isBlocked;
    private readonly string _pluginAssemblyPath;
    private readonly string _pluginHostPath;
    private readonly string _sandboxRoot;

    public string PluginName => _plugin.Name;
    public string ActionLabel => _action.Label;
    public string Category => _action.Category;

    public ObservableCollection<PluginArgumentViewModel> Arguments { get; } = new();

    private string _lastResult = "";
    public string LastResult { get => _lastResult; set { _lastResult = value; Raise(nameof(LastResult)); } }

    public ICommand RunCommand { get; }

    public PluginActionViewModel(
        IPlugin plugin,
        IPluginAction action,
        Func<ConlangModel> modelFactory,
        Action<string> log,
        string pluginAssemblyPath,
        string pluginHostPath,
        bool runOutOfProcess,
        bool isBlocked,
        TimeSpan? timeout = null)
    {
        _plugin = plugin;
        _action = action;
        _modelFactory = modelFactory;
        _log = log;
        _timeout = timeout ?? TimeSpan.FromSeconds(10);
        _runOutOfProcess = runOutOfProcess;
        _isBlocked = isBlocked;
        _pluginAssemblyPath = pluginAssemblyPath;
        _pluginHostPath = pluginHostPath;
        _sandboxRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConlangBuilder", "plugin-sandbox");
        RunCommand = new RelayCommand(async _ => await RunAsync(), _ => !_isBlocked);
        foreach (var arg in action.Arguments)
            Arguments.Add(new PluginArgumentViewModel(arg));
    }

    private async Task RunAsync()
    {
        try
        {
            var args = BuildArgumentDictionary();
            if (args is null)
                return;

            LastResult = "Running...";
            var model = _modelFactory();

            if (_runOutOfProcess && File.Exists(_pluginHostPath))
            {
                var result = await RunOutOfProcessAsync(model, args);
                LastResult = result.Success ? $"Success: {result.Message}" : $"Failed: {result.Message}";
                _log($"[{PluginName}] {ActionLabel}: {LastResult}");
                return;
            }

            using var cts = new CancellationTokenSource(_timeout);
            var inProcess = await Task.Run(() => _action.RunAsync(model, args, cts.Token));
            LastResult = inProcess.Success ? $"Success: {inProcess.Message}" : $"Failed: {inProcess.Message}";
            _log($"[{PluginName}] {ActionLabel}: {LastResult}");
        }
        catch (Exception ex)
        {
            LastResult = $"Error: {ex.Message}";
            _log($"[{PluginName}] {ActionLabel}: {LastResult}");
        }
    }

    private async Task<PluginResult> RunOutOfProcessAsync(ConlangModel model, Dictionary<string, object?> args)
    {
        Directory.CreateDirectory(_sandboxRoot);
        var sandboxDir = Path.Combine(_sandboxRoot, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(sandboxDir);

        var modelPath = Path.Combine(sandboxDir, "model.json");
        var argsPath = Path.Combine(sandboxDir, "args.json");
        var resultPath = Path.Combine(sandboxDir, "result.json");

        File.WriteAllText(modelPath, model.ToJson());
        var argJson = JsonSerializer.Serialize(args);
        File.WriteAllText(argsPath, argJson);

        var startInfo = new ProcessStartInfo
        {
            FileName = _pluginHostPath,
            Arguments = $"--plugin \"{_pluginAssemblyPath}\" --action \"{_action.ActionId}\" --model \"{modelPath}\" --args \"{argsPath}\" --result \"{resultPath}\" --outputDir \"{sandboxDir}\" --timeoutMs {(int)_timeout.TotalMilliseconds}",
            WorkingDirectory = sandboxDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = Process.Start(startInfo);
        if (process is null)
            return new PluginResult(false, "Failed to launch plugin host.");

        using var cts = new CancellationTokenSource(_timeout);
        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            try { process.Kill(true); } catch { }
            return new PluginResult(false, "Plugin timed out.");
        }

        var stdOut = await process.StandardOutput.ReadToEndAsync();
        var stdErr = await process.StandardError.ReadToEndAsync();
        if (!string.IsNullOrWhiteSpace(stdOut))
            _log($"[{PluginName}] Host: {stdOut.Trim()}");
        if (!string.IsNullOrWhiteSpace(stdErr))
            _log($"[{PluginName}] Host error: {stdErr.Trim()}");

        if (!File.Exists(resultPath))
            return new PluginResult(false, "Plugin host did not produce a result.");

        var resultJson = await File.ReadAllTextAsync(resultPath);
        return JsonSerializer.Deserialize<PluginResult>(resultJson) ?? new PluginResult(false, "Invalid plugin result.");
    }

    private Dictionary<string, object?>? BuildArgumentDictionary()
    {
        var args = new Dictionary<string, object?>();
        foreach (var arg in Arguments)
        {
            if (arg.Required && string.IsNullOrWhiteSpace(arg.Value))
            {
                LastResult = $"Missing required argument: {arg.Label}";
                return null;
            }

            if (string.IsNullOrWhiteSpace(arg.Value))
            {
                args[arg.Name] = null;
                continue;
            }

            args[arg.Name] = ParseValue(arg.Type, arg.Value);
        }

        return args;
    }

    private static object ParseValue(string type, string value)
    {
        var normalized = type?.Trim().ToLowerInvariant() ?? "string";
        return normalized switch
        {
            "int" or "integer" => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : value,
            "double" or "float" or "number" => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d) ? d : value,
            "bool" or "boolean" => bool.TryParse(value, out var b) ? b : value,
            _ => value
        };
    }
}
