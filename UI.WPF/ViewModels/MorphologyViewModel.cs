using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Language.Morphology;
using Core.Language.Shared;

namespace UI.WPF.ViewModels;

public sealed class MorphologyViewModel : ViewModelBase
{
    private readonly LangEngine _engine;

    public sealed record LexemeItem(string Lemma, string Pos, string UR);
    public sealed record AffixItem(string Name, string Pos, string FeatureName, string FeatureValue, string Affix, bool IsPrefix);

    public ObservableCollection<LexemeItem> Lexemes { get; } = new();
    public ObservableCollection<AffixItem> Affixes { get; } = new();

    public string NewLemma { get; set; } = "";
    public string NewPOS { get; set; } = "";
    public string NewPhonemes { get; set; } = "";

    public string NewAffixName { get; set; } = "";
    public string NewAffixPos { get; set; } = "N";
    public string NewFeatureName { get; set; } = "Number";
    public string NewFeatureValue { get; set; } = "Pl";
    public string NewAffixValue { get; set; } = "-i";

    public string PreviewLemma { get; set; } = "person";
    public string PreviewPos { get; set; } = "N";
    public string PreviewFeatureName { get; set; } = "Number";
    public string PreviewFeatureValue { get; set; } = "Pl";

    private string _previewResult = "";
    public string PreviewResult { get => _previewResult; set { _previewResult = value; Raise(nameof(PreviewResult)); } }

    public ICommand AddLexemeCommand { get; }
    public ICommand RemoveLexemeCommand { get; }
    public ICommand AddAffixCommand { get; }
    public ICommand RemoveAffixCommand { get; }
    public ICommand PreviewCommand { get; }

    public event Action? LexiconChanged;
    public event Action? AffixesChanged;

    public MorphologyViewModel(LangEngine engine)
    {
        _engine = engine;

        ReloadFromEngine();

        AddLexemeCommand = new RelayCommand(_ => AddLexeme(), _ => !string.IsNullOrWhiteSpace(NewLemma));
        RemoveLexemeCommand = new RelayCommand(RemoveLexeme);
        AddAffixCommand = new RelayCommand(_ => AddAffix(), _ => !string.IsNullOrWhiteSpace(NewAffixName));
        RemoveAffixCommand = new RelayCommand(RemoveAffix);
        PreviewCommand = new RelayCommand(_ => RunPreview());
        RunPreview();
    }

    private void AddLexeme()
    {
        var lemma = new LexemeItem(
            NewLemma,
            string.IsNullOrWhiteSpace(NewPOS) ? "N" : NewPOS,
            string.Join(" ", NewPhonemes.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
        );
        Lexemes.Add(lemma);
        UpdateLexicon();
        LexiconChanged?.Invoke();

        NewLemma = NewPOS = NewPhonemes = "";
        Raise(nameof(NewLemma));
        Raise(nameof(NewPOS));
        Raise(nameof(NewPhonemes));
    }

    private void RemoveLexeme(object? p)
    {
        if (p is LexemeItem item)
        {
            Lexemes.Remove(item);
            UpdateLexicon();
            LexiconChanged?.Invoke();
        }
    }

    private void AddAffix()
    {
        Affixes.Add(new AffixItem(NewAffixName, NewAffixPos, NewFeatureName, NewFeatureValue, NewAffixValue, false));
        UpdateAffixes();
        AffixesChanged?.Invoke();

        NewAffixName = "";
        Raise(nameof(NewAffixName));
    }

    private void RemoveAffix(object? p)
    {
        if (p is AffixItem item)
        {
            Affixes.Remove(item);
            UpdateAffixes();
            AffixesChanged?.Invoke();
        }
    }


    public void ReloadFromEngine()
    {
        Lexemes.Clear();
        if (_engine.Morphology.Lexicon is InMemoryLexicon mem)
        {
            foreach (var lexeme in mem.GetAll().OrderBy(l => l.Lemma))
            {
                var ur = lexeme.rootPhones.TryGetValue("UR", out var val) ? val : "";
                Lexemes.Add(new LexemeItem(lexeme.Lemma, lexeme.Pos, ur));
            }
        }

        Affixes.Clear();
        foreach (var rule in _engine.Morphology.AffixRules)
            Affixes.Add(new AffixItem(rule.Name, rule.Pos, rule.FeatureName, rule.FeatureValue, rule.Affix, rule.IsPrefix));

        RunPreview();
    }

    private void UpdateLexicon()
    {
        if (_engine.Morphology.Lexicon is InMemoryLexicon mem)
        {
            mem.ReplaceAll(Lexemes.Select(li =>
                new Lexeme(li.Lemma, li.Pos, new() { ["UR"] = li.UR })));
        }
    }

    private void UpdateAffixes()
    {
        _engine.Morphology.AffixRules.Clear();
        foreach (var a in Affixes)
            _engine.Morphology.AffixRules.Add(new AffixRule(a.Name, a.Pos, a.FeatureName, a.FeatureValue, a.Affix, a.IsPrefix));

        RunPreview();
    }

    private void RunPreview()
    {
        var features = new FeatBundle([new Feat(PreviewFeatureName, PreviewFeatureValue)]);
        PreviewResult = _engine.Morphology.RealizeForPreview(PreviewLemma, PreviewPos, features);
    }
}
