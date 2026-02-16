using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ConlangBuilder.Core;
using ConlangBuilder.Forms;

namespace ConlangBuilder
{
    public partial class MainForm : Form
    {
        private ConlangModel model = new();

        public MainForm()
        {
            InitializeComponent();
            HookEvents();
            txtName.Text = model.Name;
            RefreshAll();
        }

        private void HookEvents()
        {
            txtName.TextChanged += (_, __) => model.Name = txtName.Text;
            btnSave.Click += btnSave_Click;
            btnLoad.Click += btnLoad_Click;

            btnAddPhon.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtPhon.Text)) return;
                var p = new Phoneme(
                    txtPhon.Text.Trim(),
                    (string)cboType.SelectedItem!,
                    (int)numPhonWeight.Value,
                    chkAllowOnset.Checked,
                    chkAllowCoda.Checked
                );
                var existing = model.Inventory.Phonemes.FirstOrDefault(x => x.Symbol == p.Symbol && x.Type == p.Type);
                if (existing != null) p.Features = existing.Features;
                model.Inventory.Phonemes.RemoveAll(x => x.Symbol == p.Symbol && x.Type == p.Type);
                model.Inventory.Phonemes.Add(p);
                txtPhon.Clear();
                RefreshPhonemes();
            };
            btnUpdatePhon.Click += (_, __) =>
            {
                var sel = lstC.SelectedItem as Phoneme ?? lstV.SelectedItem as Phoneme;
                if (sel is null) return;
                model.Inventory.Phonemes.RemoveAll(x => x.Symbol == sel.Symbol && x.Type == sel.Type);
                var p = new Phoneme(
                    txtPhon.Text.Trim(),
                    (string)cboType.SelectedItem!,
                    (int)numPhonWeight.Value,
                    chkAllowOnset.Checked,
                    chkAllowCoda.Checked
                );
                p.Features = sel.Features;
                model.Inventory.Phonemes.Add(p);
                RefreshPhonemes();
            };
            btnDelPhon.Click += (_, __) =>
            {
                var sel = lstC.SelectedItem as Phoneme ?? lstV.SelectedItem as Phoneme;
                if (sel is null) return;
                model.Inventory.Phonemes.RemoveAll(p => p.Symbol == sel.Symbol && p.Type == sel.Type);
                RefreshPhonemes();
            };
            lstC.SelectedIndexChanged += (_, __) => LoadPhonFromSelection(lstC.SelectedItem as Phoneme);
            lstV.SelectedIndexChanged += (_, __) => LoadPhonFromSelection(lstV.SelectedItem as Phoneme);

            btnFeatures.Click += (_, __) =>
            {
                using var dlg = new FeatureEditor(model);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    RefreshPhonemes();
            };

            btnAddOrth.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtPh.Text) || string.IsNullOrWhiteSpace(txtGr.Text)) return;
                model.Orthography.RemoveAll(o => o.Phoneme == txtPh.Text.Trim());
                model.Orthography.Add(new OrthographyMap(txtPh.Text.Trim(), txtGr.Text.Trim()));
                txtPh.Clear(); txtGr.Clear();
                RefreshOrth();
            };
            btnDelOrth.Click += (_, __) =>
            {
                if (lstOrth.SelectedIndex >= 0 && lstOrth.SelectedIndex < model.Orthography.Count)
                {
                    model.Orthography.RemoveAt(lstOrth.SelectedIndex);
                    RefreshOrth();
                }
            };

            btnAddTemplate.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtTemplate.Text)) return;
                model.Templates.Add(new SyllableTemplate { Pattern = txtTemplate.Text.Trim(), Weight = (int)numTemplateWeight.Value });
                txtTemplate.Clear();
                RefreshTemplates();
            };
            btnDelTemplate.Click += (_, __) =>
            {
                if (lstTemplates.SelectedIndex >= 0 && lstTemplates.SelectedIndex < model.Templates.Count)
                {
                    model.Templates.RemoveAt(lstTemplates.SelectedIndex);
                    RefreshTemplates();
                }
            };

            btnAddMorph.Click += (_, __) =>
            {
                if (string.IsNullOrWhiteSpace(txtAffixForm.Text)) return;
                var type = (AffixType)Enum.Parse(typeof(AffixType), (string)cboAffixType.SelectedItem!);
                model.Morphology.Add(new MorphRule { Type = type, Form = txtAffixForm.Text.Trim(), Meaning = txtAffixMeaning.Text.Trim() });
                txtAffixForm.Clear(); txtAffixMeaning.Clear();
                RefreshMorph();
            };
            btnDelMorph.Click += (_, __) =>
            {
                if (lstMorph.SelectedIndex >= 0 && lstMorph.SelectedIndex < model.Morphology.Count)
                {
                    model.Morphology.RemoveAt(lstMorph.SelectedIndex);
                    RefreshMorph();
                }
            };

            btnSavePhono.Click += (_, __) =>
            {
                model.Phonotactics.AllowedOnsetClusters = txtAllowedOnsets.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                model.Phonotactics.AllowedCodaClusters = txtAllowedCodas.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                model.Phonotactics.MaxOnsetCluster = (int)numMaxOnset.Value;
                model.Phonotactics.MaxCodaCluster = (int)numMaxCoda.Value;
                model.Phonotactics.ForbiddenPatterns = txtForbidden.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            };

            btnAddRule.Click += (_, __) =>
            {
                model.SoundRules.Add(new SoundRule { Enabled = true, Input = "p", Output = "f", Environment = "_V" });
                RefreshRules();
            };
            btnDelRule.Click += (_, __) =>
            {
                if (gridRules.CurrentRow != null && gridRules.CurrentRow.Index >= 0 && gridRules.CurrentRow.Index < model.SoundRules.Count)
                {
                    model.SoundRules.RemoveAt(gridRules.CurrentRow.Index);
                    RefreshRules();
                }
            };
            btnUpRule.Click += (_, __) =>
            {
                var i = gridRules.CurrentRow?.Index ?? -1;
                if (i > 0)
                {
                    var r = model.SoundRules[i];
                    model.SoundRules.RemoveAt(i);
                    model.SoundRules.Insert(i - 1, r);
                    RefreshRules();
                    gridRules.CurrentCell = gridRules.Rows[i - 1].Cells[0];
                }
            };
            btnDownRule.Click += (_, __) =>
            {
                var i = gridRules.CurrentRow?.Index ?? -1;
                if (i >= 0 && i < model.SoundRules.Count - 1)
                {
                    var r = model.SoundRules[i];
                    model.SoundRules.RemoveAt(i);
                    model.SoundRules.Insert(i + 1, r);
                    RefreshRules();
                    gridRules.CurrentCell = gridRules.Rows[i + 1].Cells[0];
                }
            };
            btnApplyRuleTest.Click += (_, __) =>
            {
                var tmp = txtRuleTest.Text;
                foreach (var r in model.SoundRules.Where(r => r.Enabled))
                    tmp = r.Apply(tmp, model);
                txtRuleResult.Text = tmp;
            };
            gridRules.CellValueChanged += (_, __) => SaveRulesFromGrid();
            gridRules.CurrentCellDirtyStateChanged += (_, __) =>
            {
                if (gridRules.IsCurrentCellDirty)
                    gridRules.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            btnGenerate.Click += (_, __) =>
            {
                model.Options.HyphenateSyllables = chkHyphenate.Checked;
                model.Options.AllowDuplicates = chkAllowDup.Checked;
                var gen = new WordGenerator(model);
                txtOutput.Clear();
                foreach (var w in gen.GenerateWords((int)numCount.Value, (int)numSyll.Value))
                    txtOutput.AppendText(w + Environment.NewLine);
            };
        }

        private void SaveRulesFromGrid()
        {
            model.SoundRules.Clear();
            foreach (DataGridViewRow row in gridRules.Rows)
            {
                if (row.IsNewRow) continue;
                var rule = new SoundRule
                {
                    Enabled = row.Cells["colEnabled"].Value is bool b && b,
                    Input = row.Cells["colInput"].Value?.ToString() ?? "",
                    Output = row.Cells["colOutput"].Value?.ToString() ?? "",
                    Environment = row.Cells["colEnv"].Value?.ToString() ?? "",
                    Condition = row.Cells["colCond"].Value?.ToString() ?? ""
                };
                model.SoundRules.Add(rule);
            }
        }

        private void LoadPhonFromSelection(Phoneme? sel)
        {
            if (sel is null) return;
            txtPhon.Text = sel.Symbol;
            cboType.SelectedItem = sel.Type;
            numPhonWeight.Value = Math.Max(1, sel.Weight);
            chkAllowOnset.Checked = sel.AllowOnset;
            chkAllowCoda.Checked = sel.AllowCoda;
        }

        private void RefreshAll()
        {
            cboType.Items.Clear();
            cboType.Items.AddRange(new object[] { "C", "V" });
            cboType.SelectedIndex = 0;

            cboAffixType.Items.Clear();
            cboAffixType.Items.AddRange(Enum.GetNames(typeof(AffixType)));
            cboAffixType.SelectedIndex = 0;

            numPhonWeight.Value = 1;
            numTemplateWeight.Value = 1;
            numMaxOnset.Value = model.Phonotactics.MaxOnsetCluster;
            numMaxCoda.Value = model.Phonotactics.MaxCodaCluster;
            chkHyphenate.Checked = model.Options.HyphenateSyllables;
            chkAllowDup.Checked = model.Options.AllowDuplicates;

            txtAllowedOnsets.Text = string.Join(", ", model.Phonotactics.AllowedOnsetClusters);
            txtAllowedCodas.Text = string.Join(", ", model.Phonotactics.AllowedCodaClusters);
            txtForbidden.Text = string.Join(Environment.NewLine, model.Phonotactics.ForbiddenPatterns);

            RefreshPhonemes();
            RefreshOrth();
            RefreshTemplates();
            RefreshMorph();
            RefreshRules();
        }

        private void RefreshPhonemes()
        {
            lstC.DataSource = null; lstV.DataSource = null;
            lstC.DisplayMember = "Symbol";
            lstV.DisplayMember = "Symbol";
            lstC.DataSource = model.Inventory.Consonants.ToList();
            lstV.DataSource = model.Inventory.Vowels.ToList();
        }
        private void RefreshOrth()
        {
            lstOrth.DataSource = null;
            lstOrth.DataSource = model.Orthography.Select(o => $"{o.Phoneme} → {o.Grapheme}").ToList();
        }
        private void RefreshTemplates()
        {
            lstTemplates.DataSource = null;
            lstTemplates.DataSource = model.Templates.Select(t => $"{t.Pattern} (w={t.Weight})").ToList();
        }
        private void RefreshMorph()
        {
            lstMorph.DataSource = null;
            lstMorph.DataSource = model.Morphology.Select(m => m.ToString()).ToList();
        }
        private void RefreshRules()
        {
            gridRules.Rows.Clear();
            foreach (var r in model.SoundRules)
                gridRules.Rows.Add(r.Enabled, r.Input, r.Output, r.Environment, r.Condition);
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog { Filter = "Conlang JSON|*.conlang.json", FileName = $"{model.Name}.conlang.json" };
            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, model.ToJson());
        }

        private void btnLoad_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Conlang JSON|*.conlang.json;*.json" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var json = File.ReadAllText(ofd.FileName);
                model = ConlangModel.FromJson(json);
                txtName.Text = model.Name;
                RefreshAll();
            }
        }
    }
}