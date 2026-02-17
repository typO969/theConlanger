using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Core.Language.Shared;
using Microsoft.Win32;

namespace UI.WPF.ViewModels;

public sealed class OutputViewModel : ViewModelBase
{
    private readonly LangEngine _engine;

    private string _outputText = "";
    public string OutputText { get => _outputText; set { _outputText = value; Raise(nameof(OutputText)); } }

    public ObservableCollection<string> Samples { get; } = new();

    private int _sampleCount = 5;
    public int SampleCount { get => _sampleCount; set { _sampleCount = Math.Max(1, value); Raise(nameof(SampleCount)); } }

    private int _approxWords = 6;
    public int ApproxWords { get => _approxWords; set { _approxWords = Math.Max(1, value); Raise(nameof(ApproxWords)); } }

    private string _seed = "";
    public string Seed { get => _seed; set { _seed = value; Raise(nameof(Seed)); } }

    private string _featureName = "Tense";
    public string FeatureName { get => _featureName; set { _featureName = value; Raise(nameof(FeatureName)); } }

    private string _featureValue = "Past";
    public string FeatureValue { get => _featureValue; set { _featureValue = value; Raise(nameof(FeatureValue)); } }

    public ICommand GenerateCommand { get; }
    public ICommand ExportTxtCommand { get; }
    public ICommand ExportCsvCommand { get; }

    public OutputViewModel(LangEngine engine)
    {
        _engine = engine;
        GenerateCommand = new RelayCommand(_ => Generate());
        ExportTxtCommand = new RelayCommand(_ => Export("txt"));
        ExportCsvCommand = new RelayCommand(_ => Export("csv"));
    }

    private void Generate()
    {
        Samples.Clear();

        var constraints = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(FeatureName) && !string.IsNullOrWhiteSpace(FeatureValue))
            constraints[FeatureName.Trim()] = FeatureValue.Trim();

        var baseSeed = int.TryParse(Seed, out var parsedSeed) ? parsedSeed : Environment.TickCount;

        for (var i = 0; i < SampleCount; i++)
        {
            var rng = new RNG(baseSeed + i);
            var sentence = _engine.GenerateSentence(rng, new SentenceSpec("decl", ApproxWords, constraints));
            Samples.Add(sentence);
        }

        OutputText = string.Join(Environment.NewLine, Samples.Select((s, i) => $"{i + 1}. {s}"));
    }

    private void Export(string kind)
    {
        if (Samples.Count == 0)
            return;

        var dlg = new SaveFileDialog
        {
            Filter = kind == "csv" ? "CSV (*.csv)|*.csv" : "Text (*.txt)|*.txt",
            FileName = kind == "csv" ? "samples.csv" : "samples.txt"
        };

        if (dlg.ShowDialog() != true)
            return;

        if (kind == "csv")
        {
            File.WriteAllLines(dlg.FileName,
            [
                "Index,Sentence",
                .. Samples.Select((s, i) => $"{i + 1},\"{s.Replace("\"", "\"\"")}\"")
            ]);
        }
        else
        {
            File.WriteAllLines(dlg.FileName, Samples);
        }
    }
}
