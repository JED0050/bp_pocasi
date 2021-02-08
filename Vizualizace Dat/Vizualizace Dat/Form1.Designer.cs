namespace Vizualizace_Dat
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.gMap = new GMap.NET.WindowsForms.GMapControl();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.pGraph = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.nastaveníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adresaServeruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nastavitVlastníAdresuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vlastníAdresaToolStripMenuItem = new System.Windows.Forms.ToolStripTextBox();
            this.nastavitVýchozíAdresuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.průhlednostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vlastníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOwnTransparent = new System.Windows.Forms.ToolStripTextBox();
            this.minimálníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maximálníToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.akceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nahrátCestuZGPXSouboruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.přemazatMapuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bAnim = new System.Windows.Forms.Button();
            this.rBPrec = new System.Windows.Forms.RadioButton();
            this.rBTemp = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gMap
            // 
            this.gMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMap.Bearing = 0F;
            this.gMap.CanDragMap = true;
            this.gMap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMap.GrayScaleMode = false;
            this.gMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMap.LevelsKeepInMemmory = 5;
            this.gMap.Location = new System.Drawing.Point(7, 30);
            this.gMap.MarkersEnabled = true;
            this.gMap.MaxZoom = 2;
            this.gMap.MinZoom = 2;
            this.gMap.MouseWheelZoomEnabled = true;
            this.gMap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gMap.Name = "gMap";
            this.gMap.NegativeMode = false;
            this.gMap.PolygonsEnabled = true;
            this.gMap.RetryLoadTile = 0;
            this.gMap.RoutesEnabled = true;
            this.gMap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMap.ShowTileGridLines = false;
            this.gMap.Size = new System.Drawing.Size(1112, 432);
            this.gMap.TabIndex = 15;
            this.gMap.Zoom = 0D;
            this.gMap.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gMap_OnMapZoomChanged);
            this.gMap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mouseDoubleClickInMap);
            this.gMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMoveInMap);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 634);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "05. 11. 2020 10:30";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(8, 658);
            this.trackBar1.Maximum = 8640;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1112, 45);
            this.trackBar1.TabIndex = 19;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.MouseCaptureChanged += new System.EventHandler(this.trackBar1_MouseCaptureChanged);
            // 
            // pGraph
            // 
            this.pGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pGraph.BackColor = System.Drawing.SystemColors.Window;
            this.pGraph.Location = new System.Drawing.Point(8, 468);
            this.pGraph.Name = "pGraph";
            this.pGraph.Size = new System.Drawing.Size(1112, 150);
            this.pGraph.TabIndex = 22;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nastaveníToolStripMenuItem,
            this.akceToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1125, 24);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // nastaveníToolStripMenuItem
            // 
            this.nastaveníToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.adresaServeruToolStripMenuItem,
            this.průhlednostToolStripMenuItem});
            this.nastaveníToolStripMenuItem.Name = "nastaveníToolStripMenuItem";
            this.nastaveníToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.nastaveníToolStripMenuItem.Text = "Nastavení";
            // 
            // adresaServeruToolStripMenuItem
            // 
            this.adresaServeruToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nastavitVlastníAdresuToolStripMenuItem,
            this.nastavitVýchozíAdresuToolStripMenuItem});
            this.adresaServeruToolStripMenuItem.Name = "adresaServeruToolStripMenuItem";
            this.adresaServeruToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.adresaServeruToolStripMenuItem.Text = "Adresa serveru";
            // 
            // nastavitVlastníAdresuToolStripMenuItem
            // 
            this.nastavitVlastníAdresuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vlastníAdresaToolStripMenuItem});
            this.nastavitVlastníAdresuToolStripMenuItem.Name = "nastavitVlastníAdresuToolStripMenuItem";
            this.nastavitVlastníAdresuToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.nastavitVlastníAdresuToolStripMenuItem.Text = "Nastavit vlastní adresu";
            this.nastavitVlastníAdresuToolStripMenuItem.MouseEnter += new System.EventHandler(this.nastavitVlastníAdresuToolStripMenuItem_MouseEnter);
            // 
            // vlastníAdresaToolStripMenuItem
            // 
            this.vlastníAdresaToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.vlastníAdresaToolStripMenuItem.Name = "vlastníAdresaToolStripMenuItem";
            this.vlastníAdresaToolStripMenuItem.Size = new System.Drawing.Size(180, 23);
            this.vlastníAdresaToolStripMenuItem.TextChanged += new System.EventHandler(this.vlastníAdresaToolStripMenuItem_TextChanged);
            // 
            // nastavitVýchozíAdresuToolStripMenuItem
            // 
            this.nastavitVýchozíAdresuToolStripMenuItem.Name = "nastavitVýchozíAdresuToolStripMenuItem";
            this.nastavitVýchozíAdresuToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.nastavitVýchozíAdresuToolStripMenuItem.Text = "Nastavit výchozí adresu";
            this.nastavitVýchozíAdresuToolStripMenuItem.Click += new System.EventHandler(this.nastavitVýchozíAdresuToolStripMenuItem_Click);
            // 
            // průhlednostToolStripMenuItem
            // 
            this.průhlednostToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vlastníToolStripMenuItem,
            this.minimálníToolStripMenuItem,
            this.maximálníToolStripMenuItem});
            this.průhlednostToolStripMenuItem.Name = "průhlednostToolStripMenuItem";
            this.průhlednostToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.průhlednostToolStripMenuItem.Text = "Průhlednost počasí";
            // 
            // vlastníToolStripMenuItem
            // 
            this.vlastníToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuOwnTransparent});
            this.vlastníToolStripMenuItem.Name = "vlastníToolStripMenuItem";
            this.vlastníToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.vlastníToolStripMenuItem.Text = "Vlastní";
            this.vlastníToolStripMenuItem.MouseEnter += new System.EventHandler(this.vlastníToolStripMenuItem_MouseEnter);
            // 
            // menuOwnTransparent
            // 
            this.menuOwnTransparent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuOwnTransparent.Name = "menuOwnTransparent";
            this.menuOwnTransparent.Size = new System.Drawing.Size(100, 23);
            this.menuOwnTransparent.MouseLeave += new System.EventHandler(this.menuOwnTransparent_MouseLeave);
            // 
            // minimálníToolStripMenuItem
            // 
            this.minimálníToolStripMenuItem.Name = "minimálníToolStripMenuItem";
            this.minimálníToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.minimálníToolStripMenuItem.Text = "Minimální";
            this.minimálníToolStripMenuItem.Click += new System.EventHandler(this.minimálníToolStripMenuItem_Click);
            // 
            // maximálníToolStripMenuItem
            // 
            this.maximálníToolStripMenuItem.Name = "maximálníToolStripMenuItem";
            this.maximálníToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.maximálníToolStripMenuItem.Text = "Maximální";
            this.maximálníToolStripMenuItem.Click += new System.EventHandler(this.maximálníToolStripMenuItem_Click);
            // 
            // akceToolStripMenuItem
            // 
            this.akceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nahrátCestuZGPXSouboruToolStripMenuItem,
            this.přemazatMapuToolStripMenuItem});
            this.akceToolStripMenuItem.Name = "akceToolStripMenuItem";
            this.akceToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.akceToolStripMenuItem.Text = "Akce";
            // 
            // nahrátCestuZGPXSouboruToolStripMenuItem
            // 
            this.nahrátCestuZGPXSouboruToolStripMenuItem.Name = "nahrátCestuZGPXSouboruToolStripMenuItem";
            this.nahrátCestuZGPXSouboruToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.nahrátCestuZGPXSouboruToolStripMenuItem.Text = "Nahrát trasu z GPX souboru";
            this.nahrátCestuZGPXSouboruToolStripMenuItem.Click += new System.EventHandler(this.nahrátCestuZGPXSouboruToolStripMenuItem_Click);
            // 
            // přemazatMapuToolStripMenuItem
            // 
            this.přemazatMapuToolStripMenuItem.Name = "přemazatMapuToolStripMenuItem";
            this.přemazatMapuToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
            this.přemazatMapuToolStripMenuItem.Text = "Přemazat mapu";
            this.přemazatMapuToolStripMenuItem.Click += new System.EventHandler(this.přemazatMapuToolStripMenuItem_Click);
            // 
            // bAnim
            // 
            this.bAnim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bAnim.BackColor = System.Drawing.Color.Lime;
            this.bAnim.Location = new System.Drawing.Point(1087, 629);
            this.bAnim.Name = "bAnim";
            this.bAnim.Size = new System.Drawing.Size(32, 23);
            this.bAnim.TabIndex = 24;
            this.bAnim.Text = "|>";
            this.bAnim.UseVisualStyleBackColor = false;
            this.bAnim.Click += new System.EventHandler(this.bAnim_Click);
            // 
            // rBPrec
            // 
            this.rBPrec.AutoSize = true;
            this.rBPrec.BackColor = System.Drawing.SystemColors.Control;
            this.rBPrec.Checked = true;
            this.rBPrec.Location = new System.Drawing.Point(12, 35);
            this.rBPrec.Name = "rBPrec";
            this.rBPrec.Size = new System.Drawing.Size(82, 17);
            this.rBPrec.TabIndex = 25;
            this.rBPrec.TabStop = true;
            this.rBPrec.Text = "Srážky [mm]";
            this.rBPrec.UseVisualStyleBackColor = false;
            this.rBPrec.CheckedChanged += new System.EventHandler(this.rBTemp_CheckedChanged);
            // 
            // rBTemp
            // 
            this.rBTemp.AutoSize = true;
            this.rBTemp.BackColor = System.Drawing.SystemColors.Control;
            this.rBTemp.Location = new System.Drawing.Point(12, 58);
            this.rBTemp.Name = "rBTemp";
            this.rBTemp.Size = new System.Drawing.Size(81, 17);
            this.rBTemp.TabIndex = 26;
            this.rBTemp.Text = "Teplota [°C]";
            this.rBTemp.UseVisualStyleBackColor = false;
            this.rBTemp.CheckedChanged += new System.EventHandler(this.rBTemp_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox1.Location = new System.Drawing.Point(994, 35);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox1.Size = new System.Drawing.Size(117, 17);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "radar.bourky (BMP)";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox2.Location = new System.Drawing.Point(1017, 58);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox2.Size = new System.Drawing.Size(94, 17);
            this.checkBox2.TabIndex = 28;
            this.checkBox2.Text = "Medard (BMP)";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox3.Location = new System.Drawing.Point(1029, 81);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox3.Size = new System.Drawing.Size(82, 17);
            this.checkBox3.TabIndex = 29;
            this.checkBox3.Text = "Yr.no (XML)";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox4.AutoSize = true;
            this.checkBox4.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox4.Location = new System.Drawing.Point(964, 104);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox4.Size = new System.Drawing.Size(147, 17);
            this.checkBox4.TabIndex = 30;
            this.checkBox4.Text = "Openweathermap (JSON)";
            this.checkBox4.UseVisualStyleBackColor = false;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1125, 706);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.rBTemp);
            this.Controls.Add(this.rBPrec);
            this.Controls.Add(this.bAnim);
            this.Controls.Add(this.pGraph);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gMap);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private GMap.NET.WindowsForms.GMapControl gMap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Panel pGraph;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nastaveníToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adresaServeruToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem akceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nahrátCestuZGPXSouboruToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem přemazatMapuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nastavitVlastníAdresuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nastavitVýchozíAdresuToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox vlastníAdresaToolStripMenuItem;
        private System.Windows.Forms.Button bAnim;
        private System.Windows.Forms.RadioButton rBPrec;
        private System.Windows.Forms.RadioButton rBTemp;
        private System.Windows.Forms.ToolStripMenuItem průhlednostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vlastníToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimálníToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox menuOwnTransparent;
        private System.Windows.Forms.ToolStripMenuItem maximálníToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
    }
}

