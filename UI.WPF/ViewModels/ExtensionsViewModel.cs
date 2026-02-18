using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.WPF.Project;

namespace UI.WPF.ViewModels;

public sealed class ExtensionsViewModel : ViewModelBase
{
    private readonly PluginCatalog _catalog;

    public ObservableCollection<PluginDescriptor> Plugins { get; } = new();

    public ICommand RefreshCommand { get; }

    private string _status = "No plugins discovered yet.";
    public string Status { get => _status; set { _status = value; Raise(nameof(Status)); } }

    public ExtensionsViewModel(PluginCatalog catalog)
    {
        _catalog = catalog;
        RefreshCommand = new RelayCommand(_ => Refresh());
        Refresh();
    }

    public void Refresh()
    {
        Plugins.Clear();
        foreach (var plugin in _catalog.Plugins)
            Plugins.Add(plugin);

        Status = Plugins.Count == 0
            ? "No plugin binaries found in /plugins. Discovery is in place for Phase 4 execution wiring."
            : $"{Plugins.Count} plugin candidate(s) discovered.";
    }
}
