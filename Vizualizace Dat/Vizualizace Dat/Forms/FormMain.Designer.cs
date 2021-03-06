﻿namespace Vizualizace_Dat
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.gMap = new GMap.NET.WindowsForms.GMapControl();
            this.lDateTimeForecast = new System.Windows.Forms.Label();
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
            this.početKrokůVAnimaciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animCustomMove = new System.Windows.Forms.ToolStripMenuItem();
            this.animCustomMoveText = new System.Windows.Forms.ToolStripTextBox();
            this.animMinMove = new System.Windows.Forms.ToolStripMenuItem();
            this.animMaxMove = new System.Windows.Forms.ToolStripMenuItem();
            this.početHodnotPřiDvojklikuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dblclickCustomData = new System.Windows.Forms.ToolStripMenuItem();
            this.dblclickCustomDataText = new System.Windows.Forms.ToolStripTextBox();
            this.dblclickMinData = new System.Windows.Forms.ToolStripMenuItem();
            this.dblclickMaxData = new System.Windows.Forms.ToolStripMenuItem();
            this.setDefSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.aktualizovatŠkályToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.akceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nahrátCestuZGPXSouboruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.přemazatMapuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rBPrec = new System.Windows.Forms.RadioButton();
            this.rBTemp = new System.Windows.Forms.RadioButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.rBPres = new System.Windows.Forms.RadioButton();
            this.rBHumi = new System.Windows.Forms.RadioButton();
            this.lAnimProgress = new System.Windows.Forms.Label();
            this.bAnim = new System.Windows.Forms.Button();
            this.pBScale = new System.Windows.Forms.PictureBox();
            this.lScaleMin = new System.Windows.Forms.Label();
            this.lScaleMax = new System.Windows.Forms.Label();
            this.tLPTimeMarks = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lT0 = new System.Windows.Forms.Label();
            this.lT1 = new System.Windows.Forms.Label();
            this.lT2 = new System.Windows.Forms.Label();
            this.lT3 = new System.Windows.Forms.Label();
            this.lT4 = new System.Windows.Forms.Label();
            this.lT5 = new System.Windows.Forms.Label();
            this.lT6 = new System.Windows.Forms.Label();
            this.lT7 = new System.Windows.Forms.Label();
            this.lT8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tLPTMChar = new System.Windows.Forms.TableLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBScale)).BeginInit();
            this.tLPTimeMarks.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tLPTMChar.SuspendLayout();
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
            this.gMap.Size = new System.Drawing.Size(1112, 444);
            this.gMap.TabIndex = 15;
            this.gMap.Zoom = 0D;
            this.gMap.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.gMap_OnMarkerClick);
            this.gMap.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gMap_OnMapZoomChanged);
            this.gMap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mouseDoubleClickInMap);
            this.gMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMoveInMap);
            // 
            // lDateTimeForecast
            // 
            this.lDateTimeForecast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lDateTimeForecast.AutoSize = true;
            this.lDateTimeForecast.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lDateTimeForecast.Location = new System.Drawing.Point(12, 641);
            this.lDateTimeForecast.Name = "lDateTimeForecast";
            this.lDateTimeForecast.Size = new System.Drawing.Size(128, 16);
            this.lDateTimeForecast.TabIndex = 18;
            this.lDateTimeForecast.Text = "05. 11. 2020 10:30";
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.Location = new System.Drawing.Point(0, 672);
            this.trackBar1.Maximum = 8640;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1125, 45);
            this.trackBar1.TabIndex = 19;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            this.trackBar1.MouseCaptureChanged += new System.EventHandler(this.trackBar1_MouseCaptureChanged);
            // 
            // pGraph
            // 
            this.pGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pGraph.BackColor = System.Drawing.SystemColors.Window;
            this.pGraph.Location = new System.Drawing.Point(7, 480);
            this.pGraph.Name = "pGraph";
            this.pGraph.Size = new System.Drawing.Size(1112, 150);
            this.pGraph.TabIndex = 22;
            this.pGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.pGraph_Paint);
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
            this.průhlednostToolStripMenuItem,
            this.početKrokůVAnimaciToolStripMenuItem,
            this.početHodnotPřiDvojklikuToolStripMenuItem,
            this.setDefSettings,
            this.aktualizovatŠkályToolStripMenuItem});
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
            this.adresaServeruToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
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
            this.průhlednostToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
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
            // početKrokůVAnimaciToolStripMenuItem
            // 
            this.početKrokůVAnimaciToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animCustomMove,
            this.animMinMove,
            this.animMaxMove});
            this.početKrokůVAnimaciToolStripMenuItem.Name = "početKrokůVAnimaciToolStripMenuItem";
            this.početKrokůVAnimaciToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.početKrokůVAnimaciToolStripMenuItem.Text = "Počet kroků v animaci";
            // 
            // animCustomMove
            // 
            this.animCustomMove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.animCustomMoveText});
            this.animCustomMove.Name = "animCustomMove";
            this.animCustomMove.Size = new System.Drawing.Size(130, 22);
            this.animCustomMove.Text = "Vlastní";
            this.animCustomMove.MouseEnter += new System.EventHandler(this.animCustomMove_MouseEnter);
            // 
            // animCustomMoveText
            // 
            this.animCustomMoveText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.animCustomMoveText.Name = "animCustomMoveText";
            this.animCustomMoveText.Size = new System.Drawing.Size(100, 23);
            this.animCustomMoveText.MouseLeave += new System.EventHandler(this.animCustomMove_MouseLeave);
            // 
            // animMinMove
            // 
            this.animMinMove.Name = "animMinMove";
            this.animMinMove.Size = new System.Drawing.Size(130, 22);
            this.animMinMove.Text = "Minimální";
            this.animMinMove.Click += new System.EventHandler(this.animMinMove_Click);
            // 
            // animMaxMove
            // 
            this.animMaxMove.Name = "animMaxMove";
            this.animMaxMove.Size = new System.Drawing.Size(130, 22);
            this.animMaxMove.Text = "Maximální";
            this.animMaxMove.Click += new System.EventHandler(this.animMaxMove_Click);
            // 
            // početHodnotPřiDvojklikuToolStripMenuItem
            // 
            this.početHodnotPřiDvojklikuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dblclickCustomData,
            this.dblclickMinData,
            this.dblclickMaxData});
            this.početHodnotPřiDvojklikuToolStripMenuItem.Name = "početHodnotPřiDvojklikuToolStripMenuItem";
            this.početHodnotPřiDvojklikuToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.početHodnotPřiDvojklikuToolStripMenuItem.Text = "Počet hodnot při dvojkliku";
            // 
            // dblclickCustomData
            // 
            this.dblclickCustomData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dblclickCustomDataText});
            this.dblclickCustomData.Name = "dblclickCustomData";
            this.dblclickCustomData.Size = new System.Drawing.Size(130, 22);
            this.dblclickCustomData.Text = "Vlastní";
            this.dblclickCustomData.MouseEnter += new System.EventHandler(this.dblclickCustomData_MouseEnter);
            // 
            // dblclickCustomDataText
            // 
            this.dblclickCustomDataText.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dblclickCustomDataText.Name = "dblclickCustomDataText";
            this.dblclickCustomDataText.Size = new System.Drawing.Size(100, 23);
            this.dblclickCustomDataText.MouseLeave += new System.EventHandler(this.dblclickCustomData_MouseLeave);
            // 
            // dblclickMinData
            // 
            this.dblclickMinData.Name = "dblclickMinData";
            this.dblclickMinData.Size = new System.Drawing.Size(130, 22);
            this.dblclickMinData.Text = "Minimální";
            this.dblclickMinData.Click += new System.EventHandler(this.dblclickMinData_Click);
            // 
            // dblclickMaxData
            // 
            this.dblclickMaxData.Name = "dblclickMaxData";
            this.dblclickMaxData.Size = new System.Drawing.Size(130, 22);
            this.dblclickMaxData.Text = "Maximální";
            this.dblclickMaxData.Click += new System.EventHandler(this.dblclickMaxData_Click);
            // 
            // setDefSettings
            // 
            this.setDefSettings.Name = "setDefSettings";
            this.setDefSettings.Size = new System.Drawing.Size(214, 22);
            this.setDefSettings.Text = "Nastavit výchozí nastavení";
            this.setDefSettings.Click += new System.EventHandler(this.setDefSettings_Click);
            // 
            // aktualizovatŠkályToolStripMenuItem
            // 
            this.aktualizovatŠkályToolStripMenuItem.Name = "aktualizovatŠkályToolStripMenuItem";
            this.aktualizovatŠkályToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.aktualizovatŠkályToolStripMenuItem.Text = "Aktualizovat škály";
            this.aktualizovatŠkályToolStripMenuItem.Click += new System.EventHandler(this.aktualizovatŠkályToolStripMenuItem_Click);
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
            // rBPrec
            // 
            this.rBPrec.AutoSize = true;
            this.rBPrec.BackColor = System.Drawing.SystemColors.Control;
            this.rBPrec.Location = new System.Drawing.Point(12, 35);
            this.rBPrec.Name = "rBPrec";
            this.rBPrec.Size = new System.Drawing.Size(82, 17);
            this.rBPrec.TabIndex = 25;
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
            this.checkBox1.Location = new System.Drawing.Point(1020, 35);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox1.Size = new System.Drawing.Size(91, 17);
            this.checkBox1.TabIndex = 27;
            this.checkBox1.Text = "Radar.Bourky";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox2.AutoSize = true;
            this.checkBox2.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox2.Location = new System.Drawing.Point(1018, 58);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox2.Size = new System.Drawing.Size(93, 17);
            this.checkBox2.TabIndex = 28;
            this.checkBox2.Text = "Medard-online";
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox3.AutoSize = true;
            this.checkBox3.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox3.Location = new System.Drawing.Point(1060, 81);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox3.Size = new System.Drawing.Size(51, 17);
            this.checkBox3.TabIndex = 29;
            this.checkBox3.Text = "Yr.no";
            this.checkBox3.UseVisualStyleBackColor = false;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox4.AutoSize = true;
            this.checkBox4.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox4.Location = new System.Drawing.Point(997, 104);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox4.Size = new System.Drawing.Size(114, 17);
            this.checkBox4.TabIndex = 30;
            this.checkBox4.Text = "OpenWeatherMap";
            this.checkBox4.UseVisualStyleBackColor = false;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox5.AutoSize = true;
            this.checkBox5.BackColor = System.Drawing.SystemColors.Control;
            this.checkBox5.Location = new System.Drawing.Point(998, 127);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox5.Size = new System.Drawing.Size(113, 17);
            this.checkBox5.TabIndex = 31;
            this.checkBox5.Text = "WeatherUnlocked";
            this.checkBox5.UseVisualStyleBackColor = false;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBoxLoaders_CheckedChanged);
            // 
            // rBPres
            // 
            this.rBPres.AutoSize = true;
            this.rBPres.BackColor = System.Drawing.SystemColors.Control;
            this.rBPres.Location = new System.Drawing.Point(12, 81);
            this.rBPres.Name = "rBPres";
            this.rBPres.Size = new System.Drawing.Size(74, 17);
            this.rBPres.TabIndex = 32;
            this.rBPres.Text = "Tlak [hPa]";
            this.rBPres.UseVisualStyleBackColor = false;
            this.rBPres.CheckedChanged += new System.EventHandler(this.rBTemp_CheckedChanged);
            // 
            // rBHumi
            // 
            this.rBHumi.AutoSize = true;
            this.rBHumi.BackColor = System.Drawing.SystemColors.Control;
            this.rBHumi.Location = new System.Drawing.Point(12, 104);
            this.rBHumi.Name = "rBHumi";
            this.rBHumi.Size = new System.Drawing.Size(77, 17);
            this.rBHumi.TabIndex = 33;
            this.rBHumi.Text = "Vlhkost [%]";
            this.rBHumi.UseVisualStyleBackColor = false;
            this.rBHumi.CheckedChanged += new System.EventHandler(this.rBTemp_CheckedChanged);
            // 
            // lAnimProgress
            // 
            this.lAnimProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lAnimProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lAnimProgress.Location = new System.Drawing.Point(958, 641);
            this.lAnimProgress.Name = "lAnimProgress";
            this.lAnimProgress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lAnimProgress.Size = new System.Drawing.Size(116, 16);
            this.lAnimProgress.TabIndex = 34;
            this.lAnimProgress.Text = "  ";
            // 
            // bAnim
            // 
            this.bAnim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bAnim.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.bAnim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bAnim.ForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.bAnim.Image = global::Vizualizace_Dat.Properties.Resources.play_button_32;
            this.bAnim.Location = new System.Drawing.Point(1080, 630);
            this.bAnim.Name = "bAnim";
            this.bAnim.Size = new System.Drawing.Size(39, 38);
            this.bAnim.TabIndex = 24;
            this.bAnim.UseVisualStyleBackColor = false;
            this.bAnim.Click += new System.EventHandler(this.bAnim_Click);
            // 
            // pBScale
            // 
            this.pBScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pBScale.Location = new System.Drawing.Point(12, 269);
            this.pBScale.Name = "pBScale";
            this.pBScale.Size = new System.Drawing.Size(20, 200);
            this.pBScale.TabIndex = 35;
            this.pBScale.TabStop = false;
            // 
            // lScaleMin
            // 
            this.lScaleMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lScaleMin.AutoSize = true;
            this.lScaleMin.BackColor = System.Drawing.SystemColors.Control;
            this.lScaleMin.Location = new System.Drawing.Point(38, 456);
            this.lScaleMin.Name = "lScaleMin";
            this.lScaleMin.Size = new System.Drawing.Size(13, 13);
            this.lScaleMin.TabIndex = 36;
            this.lScaleMin.Text = "0";
            // 
            // lScaleMax
            // 
            this.lScaleMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lScaleMax.AutoSize = true;
            this.lScaleMax.BackColor = System.Drawing.SystemColors.Control;
            this.lScaleMax.Location = new System.Drawing.Point(38, 269);
            this.lScaleMax.Name = "lScaleMax";
            this.lScaleMax.Size = new System.Drawing.Size(19, 13);
            this.lScaleMax.TabIndex = 37;
            this.lScaleMax.Text = "30";
            // 
            // tLPTimeMarks
            // 
            this.tLPTimeMarks.AutoScroll = true;
            this.tLPTimeMarks.ColumnCount = 11;
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.10779F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11153F));
            this.tLPTimeMarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tLPTimeMarks.Controls.Add(this.label1, 0, 0);
            this.tLPTimeMarks.Controls.Add(this.lT0, 1, 0);
            this.tLPTimeMarks.Controls.Add(this.lT1, 2, 0);
            this.tLPTimeMarks.Controls.Add(this.lT2, 3, 0);
            this.tLPTimeMarks.Controls.Add(this.lT3, 4, 0);
            this.tLPTimeMarks.Controls.Add(this.lT4, 5, 0);
            this.tLPTimeMarks.Controls.Add(this.lT5, 6, 0);
            this.tLPTimeMarks.Controls.Add(this.lT6, 7, 0);
            this.tLPTimeMarks.Controls.Add(this.lT7, 8, 0);
            this.tLPTimeMarks.Controls.Add(this.lT8, 9, 0);
            this.tLPTimeMarks.Controls.Add(this.label2, 10, 0);
            this.tLPTimeMarks.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tLPTimeMarks.Location = new System.Drawing.Point(0, 720);
            this.tLPTimeMarks.Name = "tLPTimeMarks";
            this.tLPTimeMarks.RowCount = 1;
            this.tLPTimeMarks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPTimeMarks.Size = new System.Drawing.Size(1125, 13);
            this.tLPTimeMarks.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 39;
            // 
            // lT0
            // 
            this.lT0.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT0.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT0.Location = new System.Drawing.Point(18, 0);
            this.lT0.Name = "lT0";
            this.lT0.Size = new System.Drawing.Size(116, 13);
            this.lT0.TabIndex = 41;
            this.lT0.Text = "05.11. 10:30";
            this.lT0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT1
            // 
            this.lT1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT1.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT1.Location = new System.Drawing.Point(140, 0);
            this.lT1.Name = "lT1";
            this.lT1.Size = new System.Drawing.Size(116, 13);
            this.lT1.TabIndex = 42;
            this.lT1.Text = "05.11. 10:30";
            this.lT1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT2
            // 
            this.lT2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT2.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT2.Location = new System.Drawing.Point(262, 0);
            this.lT2.Name = "lT2";
            this.lT2.Size = new System.Drawing.Size(116, 13);
            this.lT2.TabIndex = 39;
            this.lT2.Text = "05.11. 10:30";
            this.lT2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT3
            // 
            this.lT3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT3.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT3.Location = new System.Drawing.Point(384, 0);
            this.lT3.Name = "lT3";
            this.lT3.Size = new System.Drawing.Size(116, 13);
            this.lT3.TabIndex = 40;
            this.lT3.Text = "05.11. 10:30";
            this.lT3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT4
            // 
            this.lT4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT4.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT4.Location = new System.Drawing.Point(506, 0);
            this.lT4.Name = "lT4";
            this.lT4.Size = new System.Drawing.Size(116, 13);
            this.lT4.TabIndex = 43;
            this.lT4.Text = "05.11. 10:30";
            this.lT4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT5
            // 
            this.lT5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT5.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT5.Location = new System.Drawing.Point(628, 0);
            this.lT5.Name = "lT5";
            this.lT5.Size = new System.Drawing.Size(116, 13);
            this.lT5.TabIndex = 46;
            this.lT5.Text = "05.11. 10:30";
            this.lT5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT6
            // 
            this.lT6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT6.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT6.Location = new System.Drawing.Point(750, 0);
            this.lT6.Name = "lT6";
            this.lT6.Size = new System.Drawing.Size(116, 13);
            this.lT6.TabIndex = 47;
            this.lT6.Text = "05.11. 10:30";
            this.lT6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT7
            // 
            this.lT7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT7.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT7.Location = new System.Drawing.Point(872, 0);
            this.lT7.Name = "lT7";
            this.lT7.Size = new System.Drawing.Size(116, 13);
            this.lT7.TabIndex = 44;
            this.lT7.Text = "05.11. 10:30";
            this.lT7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lT8
            // 
            this.lT8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lT8.ImageAlign = System.Drawing.ContentAlignment.TopRight;
            this.lT8.Location = new System.Drawing.Point(994, 0);
            this.lT8.Name = "lT8";
            this.lT8.Size = new System.Drawing.Size(116, 13);
            this.lT8.TabIndex = 45;
            this.lT8.Text = "05.11. 10:30";
            this.lT8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1116, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 40;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.tLPTMChar);
            this.panel1.Location = new System.Drawing.Point(15, 704);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1101, 13);
            this.panel1.TabIndex = 38;
            // 
            // tLPTMChar
            // 
            this.tLPTMChar.BackColor = System.Drawing.Color.Transparent;
            this.tLPTMChar.ColumnCount = 9;
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.tLPTMChar.Controls.Add(this.label20, 0, 0);
            this.tLPTMChar.Controls.Add(this.label19, 0, 0);
            this.tLPTMChar.Controls.Add(this.label18, 0, 0);
            this.tLPTMChar.Controls.Add(this.label17, 0, 0);
            this.tLPTMChar.Controls.Add(this.label16, 0, 0);
            this.tLPTMChar.Controls.Add(this.label15, 0, 0);
            this.tLPTMChar.Controls.Add(this.label14, 0, 0);
            this.tLPTMChar.Controls.Add(this.label13, 0, 0);
            this.tLPTMChar.Controls.Add(this.label12, 0, 0);
            this.tLPTMChar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tLPTMChar.Location = new System.Drawing.Point(0, 0);
            this.tLPTMChar.Name = "tLPTMChar";
            this.tLPTMChar.RowCount = 1;
            this.tLPTMChar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tLPTMChar.Size = new System.Drawing.Size(1101, 13);
            this.tLPTMChar.TabIndex = 0;
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.Location = new System.Drawing.Point(735, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(116, 13);
            this.label20.TabIndex = 47;
            this.label20.Text = "|";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.Location = new System.Drawing.Point(613, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(116, 13);
            this.label19.TabIndex = 46;
            this.label19.Text = "|";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.Location = new System.Drawing.Point(979, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(119, 13);
            this.label18.TabIndex = 45;
            this.label18.Text = "|";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.Location = new System.Drawing.Point(857, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(116, 13);
            this.label17.TabIndex = 44;
            this.label17.Text = "|";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.Location = new System.Drawing.Point(491, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(116, 13);
            this.label16.TabIndex = 43;
            this.label16.Text = "|";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.Location = new System.Drawing.Point(125, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(116, 13);
            this.label15.TabIndex = 42;
            this.label15.Text = "|";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(116, 13);
            this.label14.TabIndex = 41;
            this.label14.Text = "|";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.Location = new System.Drawing.Point(369, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(116, 13);
            this.label13.TabIndex = 40;
            this.label13.Text = "|";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.Location = new System.Drawing.Point(247, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "|";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1125, 733);
            this.Controls.Add(this.tLPTimeMarks);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pBScale);
            this.Controls.Add(this.lScaleMax);
            this.Controls.Add(this.lScaleMin);
            this.Controls.Add(this.bAnim);
            this.Controls.Add(this.lAnimProgress);
            this.Controls.Add(this.rBHumi);
            this.Controls.Add(this.rBPres);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.rBTemp);
            this.Controls.Add(this.rBPrec);
            this.Controls.Add(this.pGraph);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.lDateTimeForecast);
            this.Controls.Add(this.gMap);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Bude-Hezky";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Form1_ResizeEnd);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBScale)).EndInit();
            this.tLPTimeMarks.ResumeLayout(false);
            this.tLPTimeMarks.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tLPTMChar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private GMap.NET.WindowsForms.GMapControl gMap;
        private System.Windows.Forms.Label lDateTimeForecast;
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
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.RadioButton rBPres;
        private System.Windows.Forms.RadioButton rBHumi;
        private System.Windows.Forms.Label lAnimProgress;
        private System.Windows.Forms.ToolStripMenuItem početKrokůVAnimaciToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animCustomMove;
        private System.Windows.Forms.ToolStripTextBox animCustomMoveText;
        private System.Windows.Forms.ToolStripMenuItem animMinMove;
        private System.Windows.Forms.ToolStripMenuItem animMaxMove;
        private System.Windows.Forms.ToolStripMenuItem početHodnotPřiDvojklikuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dblclickCustomData;
        private System.Windows.Forms.ToolStripTextBox dblclickCustomDataText;
        private System.Windows.Forms.ToolStripMenuItem dblclickMinData;
        private System.Windows.Forms.ToolStripMenuItem dblclickMaxData;
        private System.Windows.Forms.ToolStripMenuItem setDefSettings;
        private System.Windows.Forms.PictureBox pBScale;
        private System.Windows.Forms.Label lScaleMin;
        private System.Windows.Forms.Label lScaleMax;
        private System.Windows.Forms.TableLayoutPanel tLPTimeMarks;
        private System.Windows.Forms.Label lT6;
        private System.Windows.Forms.Label lT5;
        private System.Windows.Forms.Label lT8;
        private System.Windows.Forms.Label lT7;
        private System.Windows.Forms.Label lT4;
        private System.Windows.Forms.Label lT1;
        private System.Windows.Forms.Label lT0;
        private System.Windows.Forms.Label lT3;
        private System.Windows.Forms.Label lT2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tLPTMChar;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem aktualizovatŠkályToolStripMenuItem;
    }
}

