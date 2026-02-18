using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UI.WPF.Project;

namespace UI.WPF.ViewModels;

public sealed class ExtensionsViewModel : ViewModelBase
{
    private readonly PluginCatalog _catalog;
    private readonly string _pluginDirectory;
    private readonly Func<LanguageProjectState> _stateProvider;
    private readonly List<string> _logs = new();
    private readonly PluginLabContext _context;
    private readonly HashSet<string> _allowedCapabilities = new(StringComparer.OrdinalIgnoreCase)
    {
        "OutputFiles"
    };
    private readonly string _pluginHostPath;

    public ObservableCollection<PluginDescriptor> Plugins { get; } = new();
    public ObservableCollection<PluginActionViewModel> Actions { get; } = new();

    private PluginDescriptor? _selectedPlugin;
    public PluginDescriptor? SelectedPlugin { get => _selectedPlugin; set { _selectedPlugin = value; Raise(nameof(SelectedPlugin)); } }

    private PluginActionViewModel? _selectedAction;
    public PluginActionViewModel? SelectedAction { get => _selectedAction; set { _selectedAction = value; Raise(nameof(SelectedAction)); } }

    public ICommand RefreshCommand { get; }

    private string _status = "No plugins discovered yet.";
    public string Status { get => _status; set { _status = value; Raise(nameof(Status)); } }

    private string _logText = "";
    public string LogText { get => _logText; set { _logText = value; Raise(nameof(LogText)); } }

    public ExtensionsViewModel(PluginCatalog catalog, string pluginDirectory, Func<LanguageProjectState> stateProvider)
    {
        _catalog = catalog;
        _pluginDirectory = pluginDirectory;
        _stateProvider = stateProvider;
        var appData = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ConlangBuilder");
        _context = new PluginLabContext(appData, AppendLog, _allowedCapabilities);
        _pluginHostPath = System.IO.Path.Combine(AppContext.BaseDirectory, "PluginHost.exe");
        RefreshCommand = new RelayCommand(_ => Refresh());
        Refresh();
    }

    public void Refresh()
    {
        Plugins.Clear();
        Actions.Clear();

        _catalog.Refresh(_pluginDirectory, _context, _allowedCapabilities);

        foreach (var plugin in _catalog.Plugins)
            Plugins.Add(plugin);

        SelectedPlugin = Plugins.FirstOrDefault();

        foreach (var result in _catalog.LoadResults)
        {
            var plugin = result.Plugin;
            var descriptor = _catalog.Plugins.FirstOrDefault(p => p.Id == plugin.Id);
            var isBlocked = descriptor?.IsBlocked ?? false;
            var isolation = descriptor?.IsolationMode ?? "InProcess";
            foreach (var action in plugin.Actions)
            {
                Actions.Add(new PluginActionViewModel(
                    plugin,
                    action,
                    BuildModel,
                    AppendLog,
                    pluginAssemblyPath: result.Path,
                    pluginHostPath: _pluginHostPath,
                    runOutOfProcess: string.Equals(isolation, "OutOfProcess", StringComparison.OrdinalIgnoreCase),
                    isBlocked: isBlocked));
            }
        }

        SelectedAction = Actions.FirstOrDefault();

        if (_catalog.DiscoveredPaths.Count == 0)
        {
            Status = "No plugin binaries found in /plugins.";
            return;
        }

        if (_catalog.LoadedPlugins.Count == 0)
        {
            Status = "Plugin binaries found, but no plugins were loaded.";
            return;
        }

        Status = $"{Plugins.Count} plugin(s) loaded, {Actions.Count} action(s) available.";
    }

    private ConlangBuilder.Core.ConlangModel BuildModel()
    {
        var state = _stateProvider();
        return PluginModelBuilder.Build(state);
    }

    private void AppendLog(string message)
    {
        void Update()
        {
            _logs.Add($"{DateTime.Now:HH:mm:ss} {message}");
            if (_logs.Count > 200)
                _logs.RemoveAt(0);
            LogText = string.Join(Environment.NewLine, _logs);
        }

        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null || dispatcher.CheckAccess())
            Update();
        else
            dispatcher.Invoke(Update);
    }
}
