using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Language.Shared;
using Core.Language.Syntax;

namespace UI.WPF.ViewModels;

public sealed class SyntaxViewModel : ViewModelBase
{
    private readonly LangEngine _engine;
    private readonly ConfigurableSyntaxGenerator _generator;

    public ObservableCollection<WordOrder> WordOrders { get; } = new(Enum.GetValues<WordOrder>());
    public ObservableCollection<ClauseTemplate> ClauseTemplates { get; } = new(Enum.GetValues<ClauseTemplate>());

    public WordOrder SelectedWordOrder
    {
        get => _generator.Settings.WordOrder;
        set
        {
            _generator.Settings.WordOrder = value;
            Raise(nameof(SelectedWordOrder));
            RefreshPreview();
            SettingsChanged?.Invoke();
        }
    }

    public ClauseTemplate SelectedClauseTemplate
    {
        get => _generator.Settings.ClauseTemplate;
        set
        {
            _generator.Settings.ClauseTemplate = value;
            Raise(nameof(SelectedClauseTemplate));
            RefreshPreview();
            SettingsChanged?.Invoke();
        }
    }

    private string _linearPreview = string.Empty;
    public string LinearPreview { get => _linearPreview; set { _linearPreview = value; Raise(nameof(LinearPreview)); } }

    private string _treePreview = string.Empty;
    public string TreePreview { get => _treePreview; set { _treePreview = value; Raise(nameof(TreePreview)); } }

    public ICommand RefreshPreviewCommand { get; }

    public event Action? SettingsChanged;

    public SyntaxViewModel(LangEngine engine, ConfigurableSyntaxGenerator generator)
    {
        _engine = engine;
        _generator = generator;
        RefreshPreviewCommand = new RelayCommand(_ => RefreshPreview());
        RefreshPreview();
    }

    public void Reload()
    {
        Raise(nameof(SelectedWordOrder));
        Raise(nameof(SelectedClauseTemplate));
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        var tree = _engine.Syntax.realize(new SentenceSpec("decl", 5, new Dictionary<string, string> { ["Tense"] = "Past" }), new Random(1));
        LinearPreview = string.Join(" ", tree.linearize().Select(n => $"{n.LemmaId}/{n.Pos}"));
        TreePreview = FormatTree(tree.Root, 0);
    }

    private static string FormatTree(SyntaxNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        var role = node.features.items.FirstOrDefault(f => f.Name == "Role")?.Value;
        var head = string.IsNullOrWhiteSpace(role) ? $"{node.Pos}:{node.LemmaId}" : $"{node.Pos}:{node.LemmaId} [{role}]";
        var lines = new List<string> { indent + head };
        lines.AddRange(node.Children.Select(c => FormatTree(c, depth + 1)));
        return string.Join(Environment.NewLine, lines);
    }
}
