using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Core.Language.Shared;
using Core.Language.Phonology;
using Core.Language.Morphology;
using Core.Language.Syntax;
using Core.Language.Orthography;
using Core.Language.Evaluation;

namespace UI.WPF.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    public ObservableCollection<TabItemViewModel> Tabs { get; }
    public TabItemViewModel? SelectedTab { get; set; }
    public LangEngine Engine { get; }

    public MainViewModel()
    {
        var inv = new PhonemeInventory
        {
            consonants = [ new("p",[]), new("t",[]), new("k",[]), new("ʃ",[]) ],
            vowels = [ new("a",[]), new("e",[]), new("i",[]), new("o",[]), new("u",[]) ]
        };
        var tact = new Phonotactics { allowedOnsets=["p","t","k","ʃ"], allowedCodas=["m","n","s","t","k"] };
        var phon = new Phonology(inv, tact);

        var lexicon = new InMemoryLexicon();
        var morph = new Morphology(lexicon);
        var syn = new Syntax(new SimpleSvoGenerator(lexicon));
        var orth = new Orthography();
        var eval = new Evaluation();
        Engine = new LangEngine(phon, morph, syn, orth, eval);

        Tabs = new ObservableCollection<TabItemViewModel>
        {
            new("Phonology", new UI.WPF.Views.PhonologyTab { DataContext = new PhonologyViewModel(Engine) }),
            new("Morphology", new UI.WPF.Views.MorphologyTab { DataContext = new MorphologyViewModel(Engine) }),
            new("Syntax", new UI.WPF.Views.SyntaxTab()),
            new("Orthography", new UI.WPF.Views.OrthographyTab()),
            new("Output", new UI.WPF.Views.OutputTab { DataContext = new OutputViewModel(Engine) })
        };
        SelectedTab = Tabs.Last();
    }
}

public record TabItemViewModel(string Title, FrameworkElement ContentView);
