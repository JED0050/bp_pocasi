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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lLonX = new System.Windows.Forms.Label();
            this.lLatY = new System.Windows.Forms.Label();
            this.pBForecast = new System.Windows.Forms.PictureBox();
            this.lPrec = new System.Windows.Forms.Label();
            this.lCity = new System.Windows.Forms.Label();
            this.axMap1 = new AxMapWinGIS.AxMap();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).BeginInit();
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
            // axMap1
            // 
            this.axMap1.Enabled = true;
            this.axMap1.Location = new System.Drawing.Point(12, 12);
            this.axMap1.Name = "axMap1";
            this.axMap1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMap1.OcxState")));
            this.axMap1.Size = new System.Drawing.Size(728, 528);
            this.axMap1.TabIndex = 5;
            this.axMap1.DblClick += new System.EventHandler(this.doubleclickInMap);
            this.axMap1.MouseMoveEvent += new AxMapWinGIS._DMapEvents_MouseMoveEventHandler(this.mouseMoveInMap);
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
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(852, 488);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(87, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "next bitmap";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1012, 556);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.axMap1);
            this.Controls.Add(this.lCity);
            this.Controls.Add(this.lPrec);
            this.Controls.Add(this.pBForecast);
            this.Controls.Add(this.lLatY);
            this.Controls.Add(this.lLonX);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMap1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lLonX;
        private System.Windows.Forms.Label lLatY;
        private System.Windows.Forms.PictureBox pBForecast;
        private System.Windows.Forms.Label lPrec;
        private System.Windows.Forms.Label lCity;
        private AxMapWinGIS.AxMap axMap1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}

