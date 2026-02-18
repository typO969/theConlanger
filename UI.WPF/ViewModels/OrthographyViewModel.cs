using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Language.Orthography;
using Core.Language.Shared;

namespace UI.WPF.ViewModels;

public sealed class OrthographyViewModel : ViewModelBase
{
    private readonly Orthography _orthography;

    public ObservableCollection<OrthographyRuleItem> Rules { get; } = new();
    public ObservableCollection<OrthographyExceptionItem> Exceptions { get; } = new();
    public ObservableCollection<CapitalizationMode> CapitalizationModes { get; } = new(System.Enum.GetValues<CapitalizationMode>());

    public string NewPhone { get; set; } = "";
    public string NewGrapheme { get; set; } = "";
    public string NewPrevContext { get; set; } = "";
    public string NewNextContext { get; set; } = "";

    public string NewExceptionInput { get; set; } = "";
    public string NewExceptionOutput { get; set; } = "";

    public CapitalizationMode SelectedCapitalization
    {
        get => _orthography.Capitalization;
        set
        {
            _orthography.Capitalization = value;
            Raise(nameof(SelectedCapitalization));
            UpdatePreview();
        }
    }

    private string _previewInput = "ʃika taka";
    public string PreviewInput { get => _previewInput; set { _previewInput = value; Raise(nameof(PreviewInput)); } }

    private string _previewOutput = "";
    public string PreviewOutput { get => _previewOutput; set { _previewOutput = value; Raise(nameof(PreviewOutput)); } }

    public ICommand AddRuleCommand { get; }
    public ICommand RemoveRuleCommand { get; }
    public ICommand AddExceptionCommand { get; }
    public ICommand RemoveExceptionCommand { get; }
    public ICommand PreviewCommand { get; }

    public event Action? RulesChanged;
    public event Action? ExceptionsChanged;

    public OrthographyViewModel(LangEngine engine)
    {
        _orthography = engine.Orthography;

        ReloadFromEngine();

        AddRuleCommand = new RelayCommand(_ => AddRule(), _ => !string.IsNullOrWhiteSpace(NewPhone));
        RemoveRuleCommand = new RelayCommand(RemoveRule);
        AddExceptionCommand = new RelayCommand(_ => AddException(), _ => !string.IsNullOrWhiteSpace(NewExceptionInput));
        RemoveExceptionCommand = new RelayCommand(RemoveException);
        PreviewCommand = new RelayCommand(_ => UpdatePreview());

        UpdatePreview();
    }


    public void ReloadFromEngine()
    {
        Rules.Clear();
        foreach (var r in _orthography.Rules)
            Rules.Add(new OrthographyRuleItem(r.Phone, r.Grapheme, r.PrevContext ?? "", r.NextContext ?? "", r.Enabled));

        Exceptions.Clear();
        foreach (var e in _orthography.Exceptions)
            Exceptions.Add(new OrthographyExceptionItem(e.Input, e.Output));

        Raise(nameof(SelectedCapitalization));
        UpdatePreview();
    }

    private void AddRule()
    {
        Rules.Add(new OrthographyRuleItem(NewPhone.Trim(), NewGrapheme, NewPrevContext, NewNextContext, true));
        SyncRules();
        RulesChanged?.Invoke();

        NewPhone = NewGrapheme = NewPrevContext = NewNextContext = "";
        Raise(nameof(NewPhone));
        Raise(nameof(NewGrapheme));
        Raise(nameof(NewPrevContext));
        Raise(nameof(NewNextContext));
        UpdatePreview();
    }

    private void RemoveRule(object? item)
    {
        if (item is OrthographyRuleItem rule)
        {
            Rules.Remove(rule);
            SyncRules();
            UpdatePreview();
            RulesChanged?.Invoke();
        }
    }

    private void AddException()
    {
        Exceptions.Add(new OrthographyExceptionItem(NewExceptionInput.Trim(), NewExceptionOutput.Trim()));
        SyncExceptions();
        ExceptionsChanged?.Invoke();

        NewExceptionInput = NewExceptionOutput = "";
        Raise(nameof(NewExceptionInput));
        Raise(nameof(NewExceptionOutput));
        UpdatePreview();
    }

    private void RemoveException(object? item)
    {
        if (item is OrthographyExceptionItem exc)
        {
            Exceptions.Remove(exc);
            SyncExceptions();
            UpdatePreview();
            ExceptionsChanged?.Invoke();
        }
    }

    private void UpdatePreview() => PreviewOutput = _orthography.Render(PreviewInput, new System.Random(1));

    private void SyncRules()
    {
        _orthography.Rules.Clear();
        foreach (var r in Rules)
        {
            _orthography.Rules.Add(new OrthographyRule(
                r.Phone,
                r.Grapheme,
                string.IsNullOrWhiteSpace(r.PrevContext) ? null : r.PrevContext,
                string.IsNullOrWhiteSpace(r.NextContext) ? null : r.NextContext,
                r.Enabled));
        }
    }

    private void SyncExceptions()
    {
        _orthography.Exceptions.Clear();
        foreach (var e in Exceptions.Where(e => !string.IsNullOrWhiteSpace(e.Input)))
            _orthography.Exceptions.Add(new OrthographyException(e.Input, e.Output));
    }
}

public sealed class OrthographyRuleItem
{
    public string Phone { get; set; }
    public string Grapheme { get; set; }
    public string PrevContext { get; set; }
    public string NextContext { get; set; }
    public bool Enabled { get; set; }

    public OrthographyRuleItem(string phone, string grapheme, string prevContext, string nextContext, bool enabled)
    {
        Phone = phone;
        Grapheme = grapheme;
        PrevContext = prevContext;
        NextContext = nextContext;
        Enabled = enabled;
    }
}

public sealed class OrthographyExceptionItem
{
    public string Input { get; set; }
    public string Output { get; set; }

    public OrthographyExceptionItem(string input, string output)
    {
        Input = input;
        Output = output;
    }
}
