namespace Vizualizace_Dat
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
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBScale)).BeginInit();
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
            // lDateTimeForecast
            // 
            this.lDateTimeForecast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lDateTimeForecast.AutoSize = true;
            this.lDateTimeForecast.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lDateTimeForecast.Location = new System.Drawing.Point(12, 634);
            this.lDateTimeForecast.Name = "lDateTimeForecast";
            this.lDateTimeForecast.Size = new System.Drawing.Size(128, 16);
            this.lDateTimeForecast.TabIndex = 18;
            this.lDateTimeForecast.Text = "05. 11. 2020 10:30";
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
            this.setDefSettings});
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
            this.lAnimProgress.Location = new System.Drawing.Point(959, 634);
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
            this.bAnim.Location = new System.Drawing.Point(1081, 623);
            this.bAnim.Name = "bAnim";
            this.bAnim.Size = new System.Drawing.Size(39, 38);
            this.bAnim.TabIndex = 24;
            this.bAnim.UseVisualStyleBackColor = false;
            this.bAnim.Click += new System.EventHandler(this.bAnim_Click);
            // 
            // pBScale
            // 
            this.pBScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pBScale.Location = new System.Drawing.Point(12, 257);
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
            this.lScaleMin.Location = new System.Drawing.Point(38, 444);
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
            this.lScaleMax.Location = new System.Drawing.Point(38, 257);
            this.lScaleMax.Name = "lScaleMax";
            this.lScaleMax.Size = new System.Drawing.Size(19, 13);
            this.lScaleMax.TabIndex = 37;
            this.lScaleMax.Text = "30";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1125, 706);
            this.Controls.Add(this.lScaleMax);
            this.Controls.Add(this.lScaleMin);
            this.Controls.Add(this.pBScale);
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
    }
}

