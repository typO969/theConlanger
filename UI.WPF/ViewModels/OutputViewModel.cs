using System;
using System.Windows.Input;
using Core.Language.Shared;

namespace UI.WPF.ViewModels;

public sealed class OutputViewModel : ViewModelBase
{
    private readonly LangEngine _engine;
    private string _outputText = "";
    public string OutputText { get => _outputText; set { _outputText = value; Raise(nameof(OutputText)); } }
    public ICommand GenerateCommand { get; }

    public OutputViewModel(LangEngine engine)
    {
        _engine = engine;
        GenerateCommand = new RelayCommand(_ => Generate());
    }

    private void Generate()
    {
        var rng = new RNG(Environment.TickCount);
        OutputText = _engine.GenerateSentence(rng, new SentenceSpec("decl", 6, null));
    }
}
