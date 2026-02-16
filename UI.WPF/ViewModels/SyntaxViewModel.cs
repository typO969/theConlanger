using System;
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
        }
    }

    private string _linearPreview = string.Empty;
    public string LinearPreview { get => _linearPreview; set { _linearPreview = value; Raise(nameof(LinearPreview)); } }

    private string _treePreview = string.Empty;
    public string TreePreview { get => _treePreview; set { _treePreview = value; Raise(nameof(TreePreview)); } }

    public ICommand RefreshPreviewCommand { get; }

    public SyntaxViewModel(LangEngine engine, ConfigurableSyntaxGenerator generator)
    {
        _engine = engine;
        _generator = generator;
        RefreshPreviewCommand = new RelayCommand(_ => RefreshPreview());
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        var tree = _engine.Syntax.realize(new SentenceSpec("decl", 5, new System.Collections.Generic.Dictionary<string, string> { ["Tense"] = "Past" }), new Random(1));
        LinearPreview = string.Join(" ", tree.linearize().Select(n => $"{n.LemmaId}/{n.Pos}"));
        TreePreview = $"Clause({string.Join(", ", tree.Root.Children.Select(c => c.features.items.FirstOrDefault(f => f.Name == "Role")?.Value ?? c.Pos))})";
    }
}
