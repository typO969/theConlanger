using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Core.Language.Evaluation;
using Core.Language.Morphology;
using Core.Language.Orthography;
using Core.Language.Phonology;
using Core.Language.Shared;
using Core.Language.Syntax;
using Microsoft.Win32;
using UI.WPF.Project;

namespace UI.WPF.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    public ObservableCollection<TabItemViewModel> Tabs { get; }
    public TabItemViewModel? SelectedTab { get; set; }
    public LangEngine Engine { get; }
    public ConfigurableSyntaxGenerator SyntaxGenerator { get; }

    public PhonologyViewModel PhonologyVm { get; }
    public MorphologyViewModel MorphologyVm { get; }
    public SyntaxViewModel SyntaxVm { get; }
    public OrthographyViewModel OrthographyVm { get; }
    public OutputViewModel OutputVm { get; }
    public InspectorsViewModel InspectorsVm { get; }
    public ExtensionsViewModel ExtensionsVm { get; }

    private readonly ProjectHistory _history = new(60);
    private readonly PluginCatalog _pluginCatalog = new();

    public ObservableCollection<string> RecentFiles { get; } = new();

    private string _projectName = "My Conlang";
    public string ProjectName { get => _projectName; set { _projectName = value; Raise(nameof(ProjectName)); } }

    private string? _currentProjectPath;
    public string CurrentProjectPath { get => _currentProjectPath ?? "(unsaved)"; private set { _currentProjectPath = value == "(unsaved)" ? null : value; Raise(nameof(CurrentProjectPath)); } }

    public ICommand SaveProjectCommand { get; }
    public ICommand SaveProjectAsCommand { get; }
    public ICommand LoadProjectCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand ImportLexiconCommand { get; }
    public ICommand ExportLexiconCommand { get; }
    public ICommand OpenRecentCommand { get; }

    public MainViewModel()
    {
        var inv = new PhonemeInventory
        {
            consonants = [new("p", []), new("t", []), new("k", []), new("ʃ", []), new("s", []), new("z", [])],
            vowels = [new("a", []), new("e", []), new("i", []), new("o", []), new("u", [])]
        };
        var tact = new Phonotactics { allowedOnsets = ["pr", "tr", "kr", "sp", "st", "sk"], allowedCodas = ["m", "n", "s", "t", "k"] };
        var phon = new Phonology(inv, tact);

        var lexicon = new InMemoryLexicon();
        var morph = new Morphology(lexicon);
        SyntaxGenerator = new ConfigurableSyntaxGenerator(lexicon);
        var syn = new Syntax(SyntaxGenerator);
        var orth = new Orthography();
        var eval = new Evaluation();
        Engine = new LangEngine(phon, morph, syn, orth, eval);

        PhonologyVm = new PhonologyViewModel(Engine);
        MorphologyVm = new MorphologyViewModel(Engine);
        SyntaxVm = new SyntaxViewModel(Engine, SyntaxGenerator);
        OrthographyVm = new OrthographyViewModel(Engine);
        OutputVm = new OutputViewModel(Engine);
        InspectorsVm = new InspectorsViewModel(Engine, SyntaxGenerator);
        var pluginFolder = Path.Combine(AppContext.BaseDirectory, "plugins");
        ExtensionsVm = new ExtensionsViewModel(_pluginCatalog, pluginFolder, () => LanguageProjectState.Capture(ProjectName, Engine, SyntaxGenerator, PhonologyVm.Rules));

        Tabs = new ObservableCollection<TabItemViewModel>
        {
            new("Phonology", new UI.WPF.Views.PhonologyTab { DataContext = PhonologyVm }),
            new("Morphology", new UI.WPF.Views.MorphologyTab { DataContext = MorphologyVm }),
            new("Syntax", new UI.WPF.Views.SyntaxTab { DataContext = SyntaxVm }),
            new("Orthography", new UI.WPF.Views.OrthographyTab { DataContext = OrthographyVm }),
            new("Output", new UI.WPF.Views.OutputTab { DataContext = OutputVm }),
            new("Inspectors", new UI.WPF.Views.InspectorsTab { DataContext = InspectorsVm }),
            new("Extensions", new UI.WPF.Views.ExtensionsTab { DataContext = ExtensionsVm })
        };
        SelectedTab = Tabs.Last();

        SaveProjectCommand = new RelayCommand(_ => SaveProject(false));
        SaveProjectAsCommand = new RelayCommand(_ => SaveProject(true));
        LoadProjectCommand = new RelayCommand(_ => LoadProjectWithDialog());
        UndoCommand = new RelayCommand(_ => Undo());
        RedoCommand = new RelayCommand(_ => Redo());
        ImportLexiconCommand = new RelayCommand(_ => ImportLexicon());
        ExportLexiconCommand = new RelayCommand(_ => ExportLexicon());
        OpenRecentCommand = new RelayCommand(OpenRecent);

        HookChangeTracking();
        PushCheckpoint("Initial");

    }

    private void HookChangeTracking()
    {
        MorphologyVm.LexiconChanged += () => PushCheckpoint("Lexicon updated");
        MorphologyVm.AffixesChanged += () => PushCheckpoint("Morphology rules updated");
        PhonologyVm.InventoryChanged += () => PushCheckpoint("Phonology inventory updated");
        PhonologyVm.RulesChanged += () => PushCheckpoint("Phonology rules updated");
        SyntaxVm.SettingsChanged += () => PushCheckpoint("Syntax settings updated");
        OrthographyVm.RulesChanged += () => PushCheckpoint("Orthography rules updated");
        OrthographyVm.ExceptionsChanged += () => PushCheckpoint("Orthography exceptions updated");
    }

    private void SaveProject(bool saveAs)
    {
        var path = _currentProjectPath;
        if (saveAs || string.IsNullOrWhiteSpace(path))
        {
            var sfd = new SaveFileDialog
            {
                Filter = "Conlang Project (*.clproj.json)|*.clproj.json",
                FileName = $"{ProjectName}.clproj.json"
            };
            if (sfd.ShowDialog() != true)
                return;

            path = sfd.FileName;
        }

        var state = LanguageProjectState.Capture(ProjectName, Engine, SyntaxGenerator, PhonologyVm.Rules);
        var json = JsonSerializer.Serialize(state, LanguageProjectState.JsonOptions);
        File.WriteAllText(path!, json);

        CreateVersionedBackup(path!, json);
        CurrentProjectPath = path!;
        TrackRecent(path!);
    }

    private void LoadProjectWithDialog()
    {
        var ofd = new OpenFileDialog { Filter = "Conlang Project (*.clproj.json)|*.clproj.json|JSON (*.json)|*.json" };
        if (ofd.ShowDialog() == true)
            LoadProject(ofd.FileName);
    }

    private void LoadProject(string path)
    {
        var json = File.ReadAllText(path);
        var state = JsonSerializer.Deserialize<LanguageProjectState>(json, LanguageProjectState.JsonOptions);
        if (state is null)
            return;

        state.Apply(Engine, SyntaxGenerator);
        PhonologyVm.LoadRuleSpecs(state.PhonologyRules);
        ProjectName = state.Name;
        CurrentProjectPath = path;
        TrackRecent(path);
        RefreshAllEditors(skipPhonologyRuleReset: true);
        PushCheckpoint("Project loaded");
    }

    private void Undo()
    {
        var state = _history.Undo(LanguageProjectState.Capture(ProjectName, Engine, SyntaxGenerator, PhonologyVm.Rules));
        if (state is null)
            return;

        ApplyState(state, "Undo");
    }

    private void Redo()
    {
        var state = _history.Redo(LanguageProjectState.Capture(ProjectName, Engine, SyntaxGenerator, PhonologyVm.Rules));
        if (state is null)
            return;

        ApplyState(state, "Redo");
    }

    private void ApplyState(LanguageProjectState state, string checkpoint)
    {
        state.Apply(Engine, SyntaxGenerator);
        PhonologyVm.LoadRuleSpecs(state.PhonologyRules);
        ProjectName = state.Name;
        RefreshAllEditors(skipPhonologyRuleReset: true);
        PushCheckpoint(checkpoint, suppressDuplicate: true);
    }

    private void PushCheckpoint(string label, bool suppressDuplicate = false)
    {
        var snapshot = LanguageProjectState.Capture(ProjectName, Engine, SyntaxGenerator, PhonologyVm.Rules);
        _history.Push(snapshot, label, suppressDuplicate);
        InspectorsVm.Refresh();
    }

    private void RefreshAllEditors(bool skipPhonologyRuleReset = false)
    {
        if (skipPhonologyRuleReset)
        {
            // Keep explicit RuleSpec details loaded from project snapshots.
            var consonants = Engine.Phonology.Inventory.consonants.Select(c => c.Symbol).ToList();
            var vowels = Engine.Phonology.Inventory.vowels.Select(v => v.Symbol).ToList();
            PhonologyVm.Consonants.Clear();
            foreach (var c in consonants) PhonologyVm.Consonants.Add(c);
            PhonologyVm.Vowels.Clear();
            foreach (var v in vowels) PhonologyVm.Vowels.Add(v);
            PhonologyVm.UpdateDerivationPreviewFromOutside();
        }
        else
        {
            PhonologyVm.ReloadFromEngine();
        }
        MorphologyVm.ReloadFromEngine();
        SyntaxVm.Reload();
        OrthographyVm.ReloadFromEngine();
        InspectorsVm.Refresh();
    }

    private void ImportLexicon()
    {
        var ofd = new OpenFileDialog { Filter = "CSV (*.csv)|*.csv|JSON (*.json)|*.json" };
        if (ofd.ShowDialog() != true)
            return;

        var imported = LexiconPorter.Import(ofd.FileName);
        if (_engineLexicon is not InMemoryLexicon mem)
            return;

        mem.ReplaceAll(imported);
        MorphologyVm.ReloadFromEngine();
        InspectorsVm.Refresh();
        PushCheckpoint("Lexicon imported");
    }

    private ILexicon _engineLexicon => Engine.Morphology.Lexicon;

    private void ExportLexicon()
    {
        var sfd = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv|JSON (*.json)|*.json",
            FileName = "lexicon.csv"
        };
        if (sfd.ShowDialog() != true)
            return;

        var lexemes = _engineLexicon.GetAll();
        LexiconPorter.Export(sfd.FileName, lexemes);
    }

    private void OpenRecent(object? arg)
    {
        var path = arg as string;
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            LoadProject(path);
    }

    private void TrackRecent(string path)
    {
        var normalized = Path.GetFullPath(path);
        var existing = RecentFiles.FirstOrDefault(r => string.Equals(r, normalized, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
            RecentFiles.Remove(existing);

        RecentFiles.Insert(0, normalized);
        while (RecentFiles.Count > 10)
            RecentFiles.RemoveAt(RecentFiles.Count - 1);

        Raise(nameof(RecentFiles));
    }

    private static void CreateVersionedBackup(string projectPath, string json)
    {
        var backupDir = Path.Combine(Path.GetDirectoryName(projectPath)!, ".clproj-backups");
        Directory.CreateDirectory(backupDir);
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var backupFile = Path.Combine(backupDir, $"{Path.GetFileNameWithoutExtension(projectPath)}.{stamp}.bak.json");
        File.WriteAllText(backupFile, json);
    }
}

public record TabItemViewModel(string Title, FrameworkElement ContentView);
