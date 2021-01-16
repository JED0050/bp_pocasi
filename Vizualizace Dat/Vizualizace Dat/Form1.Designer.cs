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
            this.datovéZdrojeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.akceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nahrátCestuZGPXSouboruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.přemazatMapuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bAnim = new System.Windows.Forms.Button();
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
            this.trackBar1.Maximum = 864;
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
            this.datovéZdrojeToolStripMenuItem});
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
            this.adresaServeruToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.adresaServeruToolStripMenuItem.Text = "Adresa serveru";
            // 
            // nastavitVlastníAdresuToolStripMenuItem
            // 
            this.nastavitVlastníAdresuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vlastníAdresaToolStripMenuItem});
            this.nastavitVlastníAdresuToolStripMenuItem.Name = "nastavitVlastníAdresuToolStripMenuItem";
            this.nastavitVlastníAdresuToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.nastavitVlastníAdresuToolStripMenuItem.Text = "Nastavit vlastní adresu";
            this.nastavitVlastníAdresuToolStripMenuItem.Click += new System.EventHandler(this.nastavitVlastníAdresuToolStripMenuItem_Click);
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
            // datovéZdrojeToolStripMenuItem
            // 
            this.datovéZdrojeToolStripMenuItem.Name = "datovéZdrojeToolStripMenuItem";
            this.datovéZdrojeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.datovéZdrojeToolStripMenuItem.Text = "Datové zdroje";
            this.datovéZdrojeToolStripMenuItem.Click += new System.EventHandler(this.datovéZdrojeToolStripMenuItem_Click);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1125, 706);
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
        private System.Windows.Forms.ToolStripMenuItem datovéZdrojeToolStripMenuItem;
        private System.Windows.Forms.Button bAnim;
    }
}

