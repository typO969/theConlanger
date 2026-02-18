using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Language.Morphology;
using Core.Language.Shared;
using Core.Language.Syntax;

namespace UI.WPF.ViewModels;

public sealed class InspectorsViewModel : ViewModelBase
{
    private readonly LangEngine _engine;
    private readonly ConfigurableSyntaxGenerator _syntax;

    public ObservableCollection<string> LexemeIds { get; } = new();

    private string _selectedLexeme = "person";
    public string SelectedLexeme { get => _selectedLexeme; set { _selectedLexeme = value; Raise(nameof(SelectedLexeme)); } }

    private string _lexemeInspector = "";
    public string LexemeInspector { get => _lexemeInspector; set { _lexemeInspector = value; Raise(nameof(LexemeInspector)); } }

    private string _syntaxTreeInspector = "";
    public string SyntaxTreeInspector { get => _syntaxTreeInspector; set { _syntaxTreeInspector = value; Raise(nameof(SyntaxTreeInspector)); } }

    private string _phonologyTraceInspector = "";
    public string PhonologyTraceInspector { get => _phonologyTraceInspector; set { _phonologyTraceInspector = value; Raise(nameof(PhonologyTraceInspector)); } }

    public ICommand RefreshCommand { get; }

    public InspectorsViewModel(LangEngine engine, ConfigurableSyntaxGenerator syntax)
    {
        _engine = engine;
        _syntax = syntax;
        RefreshCommand = new RelayCommand(_ => Refresh());
        Refresh();
    }

    public void Refresh()
    {
        LexemeIds.Clear();
        foreach (var l in _engine.Morphology.Lexicon.GetAll().OrderBy(l => l.Lemma).Select(l => l.Lemma))
            LexemeIds.Add(l);

        if (!LexemeIds.Contains(SelectedLexeme) && LexemeIds.Count > 0)
            SelectedLexeme = LexemeIds.First();

        BuildLexemeInspector();
        BuildSyntaxInspector();
        BuildPhonologyTrace();
    }

    private void BuildLexemeInspector()
    {
        var lex = _engine.Morphology.Lexicon.Resolve(SelectedLexeme);
        var ur = lex.rootPhones.TryGetValue("UR", out var u) ? u : lex.Lemma;

        var pres = _engine.Morphology.RealizeForPreview(lex.Lemma, lex.Pos, new FeatBundle(Array.Empty<Feat>()));
        var past = _engine.Morphology.RealizeForPreview(lex.Lemma, lex.Pos, new FeatBundle([new Feat("Tense", "Past")]));
        var plural = _engine.Morphology.RealizeForPreview(lex.Lemma, lex.Pos, new FeatBundle([new Feat("Number", "Pl")]));

        LexemeInspector = $"Lemma: {lex.Lemma}\nPOS: {lex.Pos}\nUR: {ur}\nSR(Pres): {pres}\nSR(Past): {past}\nSR(Plural): {plural}\nOrth(Pres): {_engine.Orthography.Render(pres.Replace(" ", ""), new Random(1))}";
    }

    private void BuildSyntaxInspector()
    {
        var tree = _engine.Syntax.realize(new SentenceSpec("decl", 5, new System.Collections.Generic.Dictionary<string, string> { ["Tense"] = "Past" }), new Random(3));
        SyntaxTreeInspector = FormatNode(tree.Root, 0);
    }

    private void BuildPhonologyTrace()
    {
        var lex = _engine.Morphology.Lexicon.Resolve(SelectedLexeme);
        var tokens = new[]
        {
            new MorphemeToken(new Morpheme("STEM", lex.Lemma, (lex.rootPhones.TryGetValue("UR", out var ur) ? ur : lex.Lemma).Split(' ', StringSplitOptions.RemoveEmptyEntries), _ => true),
                (lex.rootPhones.TryGetValue("UR", out var ur2) ? ur2 : lex.Lemma).Split(' ', StringSplitOptions.RemoveEmptyEntries))
        };

        var derived = _engine.Phonology.Derive(tokens);
        PhonologyTraceInspector = string.Join(Environment.NewLine, derived.Steps.Select(s => $"{s.Label}: {s.Value}"));
    }

    private static string FormatNode(SyntaxNode n, int depth)
    {
        var indent = new string(' ', depth * 2);
        var role = n.features.items.FirstOrDefault(f => f.Name == "Role")?.Value;
        var header = string.IsNullOrWhiteSpace(role) ? $"{n.Pos}:{n.LemmaId}" : $"{n.Pos}:{n.LemmaId} ({role})";
        var lines = n.Children.Select(c => FormatNode(c, depth + 1));
        return string.Join(Environment.NewLine, new[] { indent + header }.Concat(lines));
    }
}
