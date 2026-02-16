using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Core.Language.Morphology;
using Core.Language.Shared;

namespace UI.WPF.ViewModels
{
	public sealed class MorphologyViewModel : ViewModelBase
	{
		private readonly LangEngine _engine;

		public sealed record LexemeItem(string Lemma, string Pos, string UR);

		public ObservableCollection<LexemeItem> Lexemes { get; } = new();
		public string NewLemma { get; set; } = "";
		public string NewPOS { get; set; } = "";
		public string NewPhonemes { get; set; } = "";

		public ICommand AddLexemeCommand { get; }
		public ICommand RemoveLexemeCommand { get; }

		public MorphologyViewModel(LangEngine engine)
		{
			_engine = engine;

			if (_engine.Morphology.Lexicon is InMemoryLexicon mem)
			{
				// Use reflection to access the private _dict field since GetAll() does not exist
				var dictField = typeof(InMemoryLexicon)
					.GetField("_dict", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

				if (dictField != null)
				{
					var dict = dictField.GetValue(mem) as Dictionary<string, Lexeme>;
					if (dict != null)
					{
						foreach (var kv in dict)
						{
							var ur = kv.Value.rootPhones.TryGetValue("UR", out var val) ? val : "";
							Lexemes.Add(new LexemeItem(kv.Value.lemma, kv.Value.pos, ur));
						}
					}
				}
			}

			AddLexemeCommand = new RelayCommand(_ => AddLexeme(), _ => !string.IsNullOrWhiteSpace(NewLemma));
			RemoveLexemeCommand = new RelayCommand(RemoveLexeme);
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
			}
		}

		private void UpdateLexicon()
		{
			if (_engine.Morphology.Lexicon is InMemoryLexicon mem)
			{
				// Clear the internal dictionary and repopulate it
				var dictField = typeof(InMemoryLexicon)
					.GetField("_dict", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

				if (dictField != null)
				{
					var dict = dictField.GetValue(mem) as Dictionary<string, Lexeme>;
					if (dict != null)
					{
						dict.Clear();
						foreach (var li in Lexemes)
						{
							var lexeme = new Lexeme(li.Lemma, li.Pos, new() { { "UR", li.UR } });
							dict[li.Lemma] = lexeme;
						}
					}
				}
			}
		}
	}
}
