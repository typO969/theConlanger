namespace ConlangBuilder
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabPhonemes;
        private System.Windows.Forms.TabPage tabOrth;
        private System.Windows.Forms.TabPage tabSyll;
        private System.Windows.Forms.TabPage tabMorph;
        private System.Windows.Forms.TabPage tabPhono;
        private System.Windows.Forms.TabPage tabRules;
        private System.Windows.Forms.TabPage tabGen;

        private System.Windows.Forms.ListBox lstC;
        private System.Windows.Forms.ListBox lstV;
        private System.Windows.Forms.TextBox txtPhon;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.NumericUpDown numPhonWeight;
        private System.Windows.Forms.CheckBox chkAllowOnset;
        private System.Windows.Forms.CheckBox chkAllowCoda;
        private System.Windows.Forms.Button btnAddPhon;
        private System.Windows.Forms.Button btnUpdatePhon;
        private System.Windows.Forms.Button btnDelPhon;
        private System.Windows.Forms.Button btnFeatures;

        private System.Windows.Forms.ListBox lstOrth;
        private System.Windows.Forms.TextBox txtPh;
        private System.Windows.Forms.TextBox txtGr;
        private System.Windows.Forms.Button btnAddOrth;
        private System.Windows.Forms.Button btnDelOrth;

        private System.Windows.Forms.ListBox lstTemplates;
        private System.Windows.Forms.TextBox txtTemplate;
        private System.Windows.Forms.NumericUpDown numTemplateWeight;
        private System.Windows.Forms.Button btnAddTemplate;
        private System.Windows.Forms.Button btnDelTemplate;

        private System.Windows.Forms.ListBox lstMorph;
        private System.Windows.Forms.ComboBox cboAffixType;
        private System.Windows.Forms.TextBox txtAffixForm;
        private System.Windows.Forms.TextBox txtAffixMeaning;
        private System.Windows.Forms.Button btnAddMorph;
        private System.Windows.Forms.Button btnDelMorph;

        private System.Windows.Forms.TextBox txtAllowedOnsets;
        private System.Windows.Forms.TextBox txtAllowedCodas;
        private System.Windows.Forms.NumericUpDown numMaxOnset;
        private System.Windows.Forms.NumericUpDown numMaxCoda;
        private System.Windows.Forms.TextBox txtForbidden;
        private System.Windows.Forms.Button btnSavePhono;

        private System.Windows.Forms.DataGridView gridRules;
        private System.Windows.Forms.Button btnAddRule;
        private System.Windows.Forms.Button btnDelRule;
        private System.Windows.Forms.Button btnUpRule;
        private System.Windows.Forms.Button btnDownRule;
        private System.Windows.Forms.TextBox txtRuleTest;
        private System.Windows.Forms.TextBox txtRuleResult;
        private System.Windows.Forms.Button btnApplyRuleTest;

        private System.Windows.Forms.NumericUpDown numSyll;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.CheckBox chkHyphenate;
        private System.Windows.Forms.CheckBox chkAllowDup;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TextBox txtOutput;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabPhonemes = new System.Windows.Forms.TabPage();
            this.tabOrth = new System.Windows.Forms.TabPage();
            this.tabSyll = new System.Windows.Forms.TabPage();
            this.tabMorph = new System.Windows.Forms.TabPage();
            this.tabPhono = new System.Windows.Forms.TabPage();
            this.tabRules = new System.Windows.Forms.TabPage();
            this.tabGen = new System.Windows.Forms.TabPage();

            this.panelTop.SuspendLayout();
            var buttonHeight = 33;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 56;
            this.panelTop.Controls.Add(this.txtName);
            this.panelTop.Controls.Add(this.btnSave);
            this.panelTop.Controls.Add(this.btnLoad);

            this.txtName.Location = new System.Drawing.Point(15, 14);
            this.txtName.PlaceholderText = "Conlang name...";
            this.txtName.Width = 260;

            this.btnSave.Location = new System.Drawing.Point(290, 12);
            this.btnSave.Text = "Save JSON";
            this.btnSave.Height = buttonHeight;
            this.btnLoad.Location = new System.Drawing.Point(390, 12);
            this.btnLoad.Text = "Load JSON";
            this.btnLoad.Height = buttonHeight;

            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.TabPages.AddRange(new System.Windows.Forms.TabPage[] { this.tabPhonemes, this.tabOrth, this.tabSyll, this.tabMorph, this.tabPhono, this.tabRules, this.tabGen });

            // Phonemes
            this.tabPhonemes.Text = "Phonemes";
            var phonSplit = new System.Windows.Forms.TableLayoutPanel();
            phonSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            phonSplit.ColumnCount = 3;
            phonSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33));
            phonSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34));
            phonSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33));

            this.lstC = new System.Windows.Forms.ListBox(); this.lstV = new System.Windows.Forms.ListBox();
            var lblC = new System.Windows.Forms.Label(){ Text="Consonants", Dock=System.Windows.Forms.DockStyle.Top};
            var lblV = new System.Windows.Forms.Label(){ Text="Vowels", Dock=System.Windows.Forms.DockStyle.Top};
            var pnlLeft = new System.Windows.Forms.Panel(){ Dock=System.Windows.Forms.DockStyle.Fill};
            var pnlMid = new System.Windows.Forms.Panel(){ Dock=System.Windows.Forms.DockStyle.Fill};
            var pnlRight = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false, AutoScroll=true};

            this.lstC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstV.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlLeft.Controls.Add(this.lstC); pnlLeft.Controls.Add(lblC);
            pnlMid.Controls.Add(this.lstV); pnlMid.Controls.Add(lblV);

            this.txtPhon = new System.Windows.Forms.TextBox(){ Width=180, PlaceholderText="Symbol (e.g., p, t͡s, a)"};
            this.cboType = new System.Windows.Forms.ComboBox(){ DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList, Width=80};
            this.numPhonWeight = new System.Windows.Forms.NumericUpDown(){ Minimum=1, Maximum=99, Value=1, Width=80 };
            this.chkAllowOnset = new System.Windows.Forms.CheckBox(){ Text="Allow Onset", Checked=true };
            this.chkAllowCoda = new System.Windows.Forms.CheckBox(){ Text="Allow Coda", Checked=true };
            this.btnAddPhon = new System.Windows.Forms.Button(){ Text="Add/Replace", Width=120, Height=buttonHeight };
            this.btnUpdatePhon = new System.Windows.Forms.Button(){ Text="Update Selected", Width=140, Height=buttonHeight };
            this.btnDelPhon = new System.Windows.Forms.Button(){ Text="Delete Selected", Width=140, Height=buttonHeight };
            this.btnFeatures = new System.Windows.Forms.Button(){ Text="Features…", Width=120, Height=buttonHeight };
            pnlRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Symbol / Type / Weight"});
            pnlRight.Controls.Add(this.txtPhon);
            pnlRight.Controls.Add(this.cboType);
            pnlRight.Controls.Add(this.numPhonWeight);
            pnlRight.Controls.Add(this.chkAllowOnset);
            pnlRight.Controls.Add(this.chkAllowCoda);
            pnlRight.Controls.Add(this.btnAddPhon);
            pnlRight.Controls.Add(this.btnUpdatePhon);
            pnlRight.Controls.Add(this.btnDelPhon);
            pnlRight.Controls.Add(this.btnFeatures);

            phonSplit.Controls.Add(pnlLeft, 0, 0);
            phonSplit.Controls.Add(pnlMid, 1, 0);
            phonSplit.Controls.Add(pnlRight, 2, 0);
            this.tabPhonemes.Controls.Add(phonSplit);

            // Orthography
            this.tabOrth.Text = "Orthography";
            var orthSplit = new System.Windows.Forms.TableLayoutPanel();
            orthSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            orthSplit.ColumnCount = 2;
            orthSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60));
            orthSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40));

            this.lstOrth = new System.Windows.Forms.ListBox(){ Dock=System.Windows.Forms.DockStyle.Fill };
            orthSplit.Controls.Add(this.lstOrth, 0, 0);

            var orthRight = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false, AutoScroll=true };
            this.txtPh = new System.Windows.Forms.TextBox(){ Width=220, PlaceholderText="Phoneme (e.g., t͡s)"};
            this.txtGr = new System.Windows.Forms.TextBox(){ Width=220, PlaceholderText="Grapheme (e.g., cz)"};
            this.btnAddOrth = new System.Windows.Forms.Button(){ Text="Map", Height=buttonHeight };
            this.btnDelOrth = new System.Windows.Forms.Button(){ Text="Delete", Height=buttonHeight };
            orthRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Phoneme"});
            orthRight.Controls.Add(this.txtPh);
            orthRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Grapheme"});
            orthRight.Controls.Add(this.txtGr);
            orthRight.Controls.Add(this.btnAddOrth);
            orthRight.Controls.Add(this.btnDelOrth);
            orthSplit.Controls.Add(orthRight, 1, 0);
            this.tabOrth.Controls.Add(orthSplit);

            // Syllables
            this.tabSyll.Text = "Syllables";
            var sylSplit = new System.Windows.Forms.TableLayoutPanel();
            sylSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            sylSplit.ColumnCount = 2;
            sylSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60));
            sylSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40));

            this.lstTemplates = new System.Windows.Forms.ListBox(){ Dock=System.Windows.Forms.DockStyle.Fill };
            sylSplit.Controls.Add(this.lstTemplates, 0, 0);

            var sylRight = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false, AutoScroll=true };
            this.txtTemplate = new System.Windows.Forms.TextBox(){ Width=220, PlaceholderText="Pattern (CV, CVC, (C)VC)"};
            this.numTemplateWeight = new System.Windows.Forms.NumericUpDown(){ Minimum=1, Maximum=99, Value=1, Width=80 };
            this.btnAddTemplate = new System.Windows.Forms.Button(){ Text="Add", Height=buttonHeight };
            this.btnDelTemplate = new System.Windows.Forms.Button(){ Text="Delete", Height=buttonHeight };
            sylRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Pattern"});
            sylRight.Controls.Add(this.txtTemplate);
            sylRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Weight"});
            sylRight.Controls.Add(this.numTemplateWeight);
            sylRight.Controls.Add(this.btnAddTemplate);
            sylRight.Controls.Add(this.btnDelTemplate);
            sylSplit.Controls.Add(sylRight, 1, 0);
            this.tabSyll.Controls.Add(sylSplit);

            // Morphology
            this.tabMorph.Text = "Morphology";
            var morphSplit = new System.Windows.Forms.TableLayoutPanel();
            morphSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            morphSplit.ColumnCount = 2;
            morphSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60));
            morphSplit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40));

            this.lstMorph = new System.Windows.Forms.ListBox(){ Dock=System.Windows.Forms.DockStyle.Fill };
            morphSplit.Controls.Add(this.lstMorph, 0, 0);

            var morphRight = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false, AutoScroll=true };
            this.cboAffixType = new System.Windows.Forms.ComboBox(){ DropDownStyle=System.Windows.Forms.ComboBoxStyle.DropDownList, Width=200 };
            this.txtAffixForm = new System.Windows.Forms.TextBox(){ Width=220, PlaceholderText="Affix form (e.g., -ka)"};
            this.txtAffixMeaning = new System.Windows.Forms.TextBox(){ Width=220, PlaceholderText="Meaning (e.g., diminutive)"};
            this.btnAddMorph = new System.Windows.Forms.Button(){ Text="Add", Height=buttonHeight };
            this.btnDelMorph = new System.Windows.Forms.Button(){ Text="Delete", Height=buttonHeight };
            morphRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Affix type"});
            morphRight.Controls.Add(this.cboAffixType);
            morphRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Form"});
            morphRight.Controls.Add(this.txtAffixForm);
            morphRight.Controls.Add(new System.Windows.Forms.Label(){ Text="Meaning"});
            morphRight.Controls.Add(this.txtAffixMeaning);
            morphRight.Controls.Add(this.btnAddMorph);
            morphRight.Controls.Add(this.btnDelMorph);
            morphSplit.Controls.Add(morphRight, 1, 0);
            this.tabMorph.Controls.Add(morphSplit);

            // Phonotactics
            this.tabPhono.Text = "Phonotactics";
            var phonoPanel = new System.Windows.Forms.TableLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, ColumnCount=2 };
            phonoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));
            phonoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50));

            var grpOnset = new System.Windows.Forms.GroupBox(){ Text="Onset rules", Dock=System.Windows.Forms.DockStyle.Fill };
            var grpCoda = new System.Windows.Forms.GroupBox(){ Text="Coda rules", Dock=System.Windows.Forms.DockStyle.Fill };
            var grpForbidden = new System.Windows.Forms.GroupBox(){ Text="Forbidden regex patterns (one per line)", Dock=System.Windows.Forms.DockStyle.Fill };

            this.txtAllowedOnsets = new System.Windows.Forms.TextBox(){ Multiline=true, ScrollBars=System.Windows.Forms.ScrollBars.Vertical, Height=120, Dock=System.Windows.Forms.DockStyle.Top };
            this.numMaxOnset = new System.Windows.Forms.NumericUpDown(){ Minimum=0, Maximum=5, Value=2, Dock=System.Windows.Forms.DockStyle.Top };
            var lblMaxOnset = new System.Windows.Forms.Label(){ Text="Max onset cluster length:", Dock=System.Windows.Forms.DockStyle.Top };

            this.txtAllowedCodas = new System.Windows.Forms.TextBox(){ Multiline=true, ScrollBars=System.Windows.Forms.ScrollBars.Vertical, Height=120, Dock=System.Windows.Forms.DockStyle.Top };
            this.numMaxCoda = new System.Windows.Forms.NumericUpDown(){ Minimum=0, Maximum=5, Value=2, Dock=System.Windows.Forms.DockStyle.Top };
            var lblMaxCoda = new System.Windows.Forms.Label(){ Text="Max coda cluster length:", Dock=System.Windows.Forms.DockStyle.Top };

            this.txtForbidden = new System.Windows.Forms.TextBox(){ Multiline=true, ScrollBars=System.Windows.Forms.ScrollBars.Vertical, Dock=System.Windows.Forms.DockStyle.Fill };
            this.btnSavePhono = new System.Windows.Forms.Button(){ Text="Apply Phonotactic Settings", Dock=System.Windows.Forms.DockStyle.Bottom, Height=buttonHeight };

            var pnlOnset = new System.Windows.Forms.Panel(){ Dock=System.Windows.Forms.DockStyle.Fill };
            pnlOnset.Controls.Add(this.btnSavePhono);
            pnlOnset.Controls.Add(this.numMaxOnset);
            pnlOnset.Controls.Add(lblMaxOnset);
            pnlOnset.Controls.Add(this.txtAllowedOnsets);
            grpOnset.Controls.Add(pnlOnset);

            var pnlCoda = new System.Windows.Forms.Panel(){ Dock=System.Windows.Forms.DockStyle.Fill };
            pnlCoda.Controls.Add(this.numMaxCoda);
            pnlCoda.Controls.Add(lblMaxCoda);
            pnlCoda.Controls.Add(this.txtAllowedCodas);
            grpCoda.Controls.Add(pnlCoda);

            grpForbidden.Controls.Add(this.txtForbidden);

            phonoPanel.Controls.Add(grpOnset, 0, 0);
            phonoPanel.Controls.Add(grpCoda, 1, 0);
            phonoPanel.Controls.Add(grpForbidden, 0, 1);
            phonoPanel.SetColumnSpan(grpForbidden, 2);

            this.tabPhono.Controls.Add(phonoPanel);

            // Rules
            this.tabRules.Text = "Sound Rules";
            var rulesLayout = new System.Windows.Forms.TableLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, RowCount=2 };
            rulesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70));
            rulesLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30));

            var gridRow = new System.Windows.Forms.TableLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, ColumnCount=2 };
            gridRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85));
            gridRow.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15));
            this.gridRules = new System.Windows.Forms.DataGridView(){ Dock=System.Windows.Forms.DockStyle.Fill, AutoSizeColumnsMode=System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows=true, AllowUserToDeleteRows=true };
            var colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn(){ Name="colEnabled", HeaderText="Enabled" };
            var colInput   = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colInput", HeaderText="Input" };
            var colOutput  = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colOutput", HeaderText="Output" };
            var colEnv     = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colEnv", HeaderText="Environment (_ marks target)" };
            var colCond    = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colCond", HeaderText="Condition ([voicing=voiced], [+voice])" };
            this.gridRules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]{ colEnabled, colInput, colOutput, colEnv, colCond });
            var btnCol = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false };
            this.btnAddRule = new System.Windows.Forms.Button(){ Text="Add Rule", Height=buttonHeight };
            this.btnDelRule = new System.Windows.Forms.Button(){ Text="Delete Rule", Height=buttonHeight };
            this.btnUpRule  = new System.Windows.Forms.Button(){ Text="↑ Move Up", Height=buttonHeight };
            this.btnDownRule= new System.Windows.Forms.Button(){ Text="↓ Move Down", Height=buttonHeight };
            btnCol.Controls.Add(this.btnAddRule);
            btnCol.Controls.Add(this.btnDelRule);
            btnCol.Controls.Add(this.btnUpRule);
            btnCol.Controls.Add(this.btnDownRule);
            gridRow.Controls.Add(this.gridRules, 0, 0);
            gridRow.Controls.Add(btnCol, 1, 0);

            var testPanel = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.LeftToRight, WrapContents=false };
            this.txtRuleTest = new System.Windows.Forms.TextBox(){ Width=400, PlaceholderText="Enter phoneme string (pre-orthography), e.g., 'taka'"};
            this.btnApplyRuleTest = new System.Windows.Forms.Button(){ Text="Apply All Rules", Height=buttonHeight };
            this.txtRuleResult = new System.Windows.Forms.TextBox(){ Width=400, ReadOnly=true };
            testPanel.Controls.Add(new System.Windows.Forms.Label(){ Text="Test:" });
            testPanel.Controls.Add(this.txtRuleTest);
            testPanel.Controls.Add(this.btnApplyRuleTest);
            testPanel.Controls.Add(new System.Windows.Forms.Label(){ Text="Result:" });
            testPanel.Controls.Add(this.txtRuleResult);

            rulesLayout.Controls.Add(gridRow, 0, 0);
            rulesLayout.Controls.Add(testPanel, 0, 1);
            this.tabRules.Controls.Add(rulesLayout);

            // Generator
            this.tabGen.Text = "Generator";
            var genPanel = new System.Windows.Forms.FlowLayoutPanel(){ Dock=System.Windows.Forms.DockStyle.Fill, FlowDirection=System.Windows.Forms.FlowDirection.TopDown, WrapContents=false };
            var genRow = new System.Windows.Forms.FlowLayoutPanel(){ AutoSize=true };
            var lblSyl = new System.Windows.Forms.Label(){ Text="Syllables per word:" };
            var lblCnt = new System.Windows.Forms.Label(){ Text="Count:" };
            this.numSyll = new System.Windows.Forms.NumericUpDown(){ Minimum=1, Maximum=8, Value=2, Width=60 };
            this.numCount = new System.Windows.Forms.NumericUpDown(){ Minimum=1, Maximum=1000, Value=10, Width=80 };
            this.chkHyphenate = new System.Windows.Forms.CheckBox(){ Text="Hyphenate syllables" };
            this.chkAllowDup = new System.Windows.Forms.CheckBox(){ Text="Allow duplicates", Checked=true };
            this.btnGenerate = new System.Windows.Forms.Button(){ Text="Generate", Height=buttonHeight };
            genRow.Controls.Add(lblSyl); genRow.Controls.Add(this.numSyll);
            genRow.Controls.Add(lblCnt); genRow.Controls.Add(this.numCount);
            genRow.Controls.Add(this.chkHyphenate); genRow.Controls.Add(this.chkAllowDup);
            this.txtOutput = new System.Windows.Forms.TextBox(){ Multiline=true, ScrollBars=System.Windows.Forms.ScrollBars.Vertical, ReadOnly=true, WordWrap=false, Height=450, Dock=System.Windows.Forms.DockStyle.Fill };
            genPanel.Controls.Add(genRow);
            genPanel.Controls.Add(this.btnGenerate);
            genPanel.Controls.Add(this.txtOutput);
            this.tabGen.Controls.Add(genPanel);

            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Text = "Conlang Builder v4 — Phonology Lab";
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.panelTop);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
        }
    }
}