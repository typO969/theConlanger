using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Language.Phonology;
using Core.Language.Shared;

namespace UI.WPF.ViewModels;

public sealed class PhonologyViewModel : ViewModelBase
{
    private readonly LangEngine _engine;

    public ObservableCollection<string> Consonants { get; } = new();
    public ObservableCollection<string> Vowels { get; } = new();
    public ObservableCollection<RuleSpec> Rules { get; } = new();

    private string _newPhoneme = "";
    public string NewPhoneme { get => _newPhoneme; set { _newPhoneme = value; Raise(nameof(NewPhoneme)); } }

    public string NewRuleName { get; set; } = "Voicing assimilation";
    public string NewRuleTarget { get; set; } = "s";
    public string NewRuleReplacement { get; set; } = "z";
    public string NewRulePrev { get; set; } = "";
    public string NewRuleNext { get; set; } = "";

    private string _derivationPreview = "";
    public string DerivationPreview { get => _derivationPreview; set { _derivationPreview = value; Raise(nameof(DerivationPreview)); } }

    public ICommand AddConsonantCommand { get; }
    public ICommand AddVowelCommand { get; }
    public ICommand RemovePhonemeCommand { get; }
    public ICommand AddRuleCommand { get; }
    public ICommand RemoveRuleCommand { get; }
    public ICommand PreviewDerivationCommand { get; }

    public PhonologyViewModel(LangEngine engine)
    {
        _engine = engine;

        foreach (var c in _engine.Phonology.Inventory.consonants)
            Consonants.Add(c.Symbol);
        foreach (var v in _engine.Phonology.Inventory.vowels)
            Vowels.Add(v.Symbol);

        AddConsonantCommand = new RelayCommand(_ => AddConsonant(), _ => !string.IsNullOrWhiteSpace(NewPhoneme));
        AddVowelCommand = new RelayCommand(_ => AddVowel(), _ => !string.IsNullOrWhiteSpace(NewPhoneme));
        RemovePhonemeCommand = new RelayCommand(RemovePhoneme);
        AddRuleCommand = new RelayCommand(_ => AddRule(), _ => !string.IsNullOrWhiteSpace(NewRuleName));
        RemoveRuleCommand = new RelayCommand(RemoveRule);
        PreviewDerivationCommand = new RelayCommand(_ => UpdateDerivationPreview());

        SeedRules();
        UpdateDerivationPreview();
    }

    private void AddConsonant()
    {
        if (!Consonants.Contains(NewPhoneme))
        {
            Consonants.Add(NewPhoneme);
            UpdateInventory();
        }

        NewPhoneme = "";
    }

    private void AddVowel()
    {
        if (!Vowels.Contains(NewPhoneme))
        {
            Vowels.Add(NewPhoneme);
            UpdateInventory();
        }

        NewPhoneme = "";
    }

    private void RemovePhoneme(object? param)
    {
        var symbol = param as string;
        if (symbol is null)
            return;

        if (Consonants.Contains(symbol))
            Consonants.Remove(symbol);
        else if (Vowels.Contains(symbol))
            Vowels.Remove(symbol);

        UpdateInventory();
    }

    private void AddRule()
    {
        Rules.Add(new RuleSpec
        {
            Name = NewRuleName,
            Target = NewRuleTarget,
            Replacement = NewRuleReplacement,
            PrevPhone = string.IsNullOrWhiteSpace(NewRulePrev) ? null : NewRulePrev,
            NextPhone = string.IsNullOrWhiteSpace(NewRuleNext) ? null : NewRuleNext
        });

        SyncRules();
        UpdateDerivationPreview();
    }

    private void RemoveRule(object? param)
    {
        if (param is RuleSpec rule)
        {
            Rules.Remove(rule);
            SyncRules();
            UpdateDerivationPreview();
        }
    }

    private void UpdateInventory()
    {
        var inventory = _engine.Phonology.Inventory;

        var newConsonants = Consonants.Select(s => new Phoneme(s, System.Array.Empty<string>())).ToList();
        var newVowels = Vowels.Select(s => new Phoneme(s, System.Array.Empty<string>())).ToList();

        inventory.consonants = newConsonants;
        inventory.vowels = newVowels;
    }

    private void SeedRules()
    {
        if (Rules.Count > 0)
            return;

        Rules.Add(new RuleSpec { Name = "Intervocalic voicing", Target = "s", Replacement = "z", PrevPhone = "a", NextPhone = "a" });
        SyncRules();
    }

    private void SyncRules()
    {
        _engine.Phonology.Rules.Clear();
        foreach (var spec in Rules)
            _engine.Phonology.Rules.Add(spec.ToRule());
    }

    private void UpdateDerivationPreview()
    {
        var mock = new[]
        {
            new Core.Language.Morphology.MorphemeToken(
                new Core.Language.Morphology.Morpheme("DEMO", "demo", new[] { "a", "s", "a" }, _ => true),
                new[] { "a", "s", "a" })
        };
        var derived = _engine.Phonology.Derive(mock);
        DerivationPreview = string.Join("\n", derived.Steps.Select(s => $"{s.Label}: {s.Value}"));
    }
}
