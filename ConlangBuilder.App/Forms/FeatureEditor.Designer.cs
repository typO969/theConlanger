namespace ConlangBuilder.Forms
{
    partial class FeatureEditor
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grid = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // grid
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.Dock = System.Windows.Forms.DockStyle.Top;
            this.grid.Height = 420;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            var colSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colSymbol", HeaderText="Symbol", ReadOnly=true };
            var colType = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colType", HeaderText="Type", ReadOnly=true };
            var colVoicing = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colVoicing", HeaderText="Voicing", DataSource=new string[]{ "", "voiced", "voiceless" } };
            var colManner = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colManner", HeaderText="Manner", DataSource=new string[]{ "", "stop", "fricative", "nasal", "approximant", "vowel", "affricate", "trill", "tap" } };
            var colPlace = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colPlace", HeaderText="Place", DataSource=new string[]{ "", "bilabial", "labiodental", "dental", "alveolar", "postalveolar", "palatal", "velar", "uvular", "pharyngeal", "glottal" } };
            var colHeight = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colHeight", HeaderText="Height", DataSource=new string[]{ "", "high", "mid", "low" } };
            var colBackness = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colBackness", HeaderText="Backness", DataSource=new string[]{ "", "front", "central", "back" } };
            var colRound = new System.Windows.Forms.DataGridViewComboBoxColumn(){ Name="colRoundness", HeaderText="Roundness", DataSource=new string[]{ "", "rounded", "unrounded" } };
            var colCustom = new System.Windows.Forms.DataGridViewTextBoxColumn(){ Name="colCustom", HeaderText="Custom tags" };

            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]{
                colSymbol, colType, colVoicing, colManner, colPlace, colHeight, colBackness, colRound, colCustom
            });

            // Buttons
            this.btnSave.Text = "Save & Close";
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.Location = new System.Drawing.Point(540, 440);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(420, 440);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // Form
            this.ClientSize = new System.Drawing.Size(680, 490);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Text = "Phoneme Features";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
        }
    }
}