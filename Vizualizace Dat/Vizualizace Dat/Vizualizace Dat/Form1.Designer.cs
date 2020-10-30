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
            this.lLonX = new System.Windows.Forms.Label();
            this.lLatY = new System.Windows.Forms.Label();
            this.pBForecast = new System.Windows.Forms.PictureBox();
            this.lPrec = new System.Windows.Forms.Label();
            this.lCity = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.gMap = new GMap.NET.WindowsForms.GMapControl();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).BeginInit();
            this.SuspendLayout();
            // 
            // lLonX
            // 
            this.lLonX.AutoSize = true;
            this.lLonX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lLonX.Location = new System.Drawing.Point(768, 127);
            this.lLonX.Name = "lLonX";
            this.lLonX.Size = new System.Drawing.Size(116, 16);
            this.lLonX.TabIndex = 0;
            this.lLonX.Text = "[X: 0] Lon: 10.06";
            // 
            // lLatY
            // 
            this.lLatY.AutoSize = true;
            this.lLatY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lLatY.Location = new System.Drawing.Point(768, 143);
            this.lLatY.Name = "lLatY";
            this.lLatY.Size = new System.Drawing.Size(117, 16);
            this.lLatY.TabIndex = 1;
            this.lLatY.Text = "[Y: 0] Lon: 51.88";
            // 
            // pBForecast
            // 
            this.pBForecast.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pBForecast.Image = global::Vizualizace_Dat.Properties.Resources.RadarBitmap2020100911;
            this.pBForecast.InitialImage = null;
            this.pBForecast.Location = new System.Drawing.Point(12, 12);
            this.pBForecast.Name = "pBForecast";
            this.pBForecast.Size = new System.Drawing.Size(728, 528);
            this.pBForecast.TabIndex = 2;
            this.pBForecast.TabStop = false;
            this.pBForecast.Click += new System.EventHandler(this.clickInBitmap);
            // 
            // lPrec
            // 
            this.lPrec.AutoSize = true;
            this.lPrec.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lPrec.Location = new System.Drawing.Point(768, 185);
            this.lPrec.Name = "lPrec";
            this.lPrec.Size = new System.Drawing.Size(109, 16);
            this.lPrec.TabIndex = 3;
            this.lPrec.Text = "Srážky: 0 [mm]";
            // 
            // lCity
            // 
            this.lCity.AutoSize = true;
            this.lCity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lCity.Location = new System.Drawing.Point(768, 223);
            this.lCity.Name = "lCity";
            this.lCity.Size = new System.Drawing.Size(99, 16);
            this.lCity.TabIndex = 4;
            this.lCity.Text = "Město: Praha";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(771, 517);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "kreslit body";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.canDrawPointChange);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(852, 517);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "vymazat body";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.clearAllPoints);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(771, 488);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "bitmapa";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.showDownloadedBitmap);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(852, 488);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(87, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "next bitmap";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.drawDownloadedBitmap);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(771, 339);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(200, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = "Bitmap from server";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.drawBitmapFromServer);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(771, 369);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 11;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(771, 396);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Bitmap Loader";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(771, 419);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(84, 17);
            this.checkBox2.TabIndex = 13;
            this.checkBox2.Text = "XML Loader";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(771, 442);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(90, 17);
            this.checkBox3.TabIndex = 14;
            this.checkBox3.Text = "JSON Loader";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // gMap
            // 
            this.gMap.Bearing = 0F;
            this.gMap.CanDragMap = true;
            this.gMap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMap.GrayScaleMode = false;
            this.gMap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMap.LevelsKeepInMemmory = 5;
            this.gMap.Location = new System.Drawing.Point(12, 12);
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
            this.gMap.Size = new System.Drawing.Size(728, 528);
            this.gMap.TabIndex = 15;
            this.gMap.Zoom = 0D;
            this.gMap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mouseDoubleClickInMap);
            this.gMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mouseMoveInMap);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(945, 491);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(43, 17);
            this.radioButton1.TabIndex = 16;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "bod";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(945, 514);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(51, 17);
            this.radioButton2.TabIndex = 17;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "cesta";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1006, 552);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.gMap);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lCity);
            this.Controls.Add(this.lPrec);
            this.Controls.Add(this.pBForecast);
            this.Controls.Add(this.lLatY);
            this.Controls.Add(this.lLonX);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lLonX;
        private System.Windows.Forms.Label lLatY;
        private System.Windows.Forms.PictureBox pBForecast;
        private System.Windows.Forms.Label lPrec;
        private System.Windows.Forms.Label lCity;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private GMap.NET.WindowsForms.GMapControl gMap;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}

