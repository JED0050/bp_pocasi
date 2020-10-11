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
            this.pBForecast.Image = global::Vizualizace_Dat.Properties.Resources.JSBitmap2020101012;
            this.pBForecast.InitialImage = null;
            this.pBForecast.Location = new System.Drawing.Point(12, 12);
            this.pBForecast.Name = "pBForecast";
            this.pBForecast.Size = new System.Drawing.Size(728, 528);
            this.pBForecast.TabIndex = 2;
            this.pBForecast.TabStop = false;
            this.pBForecast.Click += new System.EventHandler(this.clickInMap);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(1073, 557);
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
    }
}

