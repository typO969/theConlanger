using System;
using System.Linq;
using System.Windows.Forms;
using ConlangBuilder.Core;

namespace ConlangBuilder.Forms
{
    public partial class FeatureEditor : Form
    {
        private readonly ConlangModel _model;

        public FeatureEditor(ConlangModel model)
        {
            _model = model;
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            grid.Rows.Clear();
            foreach (var p in _model.Inventory.Phonemes)
            {
                var f = p.Features ?? new PhonologicalFeatures();
                grid.Rows.Add(p.Symbol, p.Type, f.Voicing, f.Manner, f.Place, f.Height, f.Backness, f.Roundness, f.Custom);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                var row = grid.Rows[i];
                if (row.IsNewRow) continue;
                var sym = row.Cells["colSymbol"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(sym)) continue;
                var p = _model.Inventory.Phonemes.FirstOrDefault(x => x.Symbol == sym);
                if (p == null) continue;
                p.Features = new PhonologicalFeatures
                {
                    Voicing = row.Cells["colVoicing"].Value?.ToString(),
                    Manner = row.Cells["colManner"].Value?.ToString(),
                    Place = row.Cells["colPlace"].Value?.ToString(),
                    Height = row.Cells["colHeight"].Value?.ToString(),
                    Backness = row.Cells["colBackness"].Value?.ToString(),
                    Roundness = row.Cells["colRoundness"].Value?.ToString(),
                    Custom = row.Cells["colCustom"].Value?.ToString()
                };
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}