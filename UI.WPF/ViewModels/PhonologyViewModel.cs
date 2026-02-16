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

    private string _newPhoneme = "";
    public string NewPhoneme { get => _newPhoneme; set { _newPhoneme = value; Raise(nameof(NewPhoneme)); } }

    public ICommand AddConsonantCommand { get; }
    public ICommand AddVowelCommand { get; }
    public ICommand RemovePhonemeCommand { get; }

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
    }

    private void AddConsonant()
    {
        if (!Consonants.Contains(NewPhoneme))
        {
            Consonants.Add(NewPhoneme);
            UpdateEngine();
        }
        NewPhoneme = "";
    }

    private void AddVowel()
    {
        if (!Vowels.Contains(NewPhoneme))
        {
            Vowels.Add(NewPhoneme);
            UpdateEngine();
        }
        NewPhoneme = "";
    }

    private void RemovePhoneme(object? param)
    {
        var symbol = param as string;
        if (symbol is null) return;

        if (Consonants.Contains(symbol)) Consonants.Remove(symbol);
        else if (Vowels.Contains(symbol)) Vowels.Remove(symbol);
        UpdateEngine();
    }

    private void UpdateEngine()
    {
        var inventory = _engine.Phonology.Inventory;

        // Use the constructor for Phoneme, since 'Symbol' is a required parameter.
        var newConsonants = Consonants.Select(s => new Phoneme(s, System.Array.Empty<string>())).ToList();
        var newVowels = Vowels.Select(s => new Phoneme(s, System.Array.Empty<string>())).ToList();

        if (inventory.consonants is List<Phoneme> consonantList)
        {
            consonantList.Clear();
            consonantList.AddRange(newConsonants);
        }
        if (inventory.vowels is List<Phoneme> vowelList)
        {
            vowelList.Clear();
            vowelList.AddRange(newVowels);
        }
    }
}
