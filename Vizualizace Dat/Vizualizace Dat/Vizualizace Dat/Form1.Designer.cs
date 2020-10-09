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
            this.lLonY = new System.Windows.Forms.Label();
            this.lLatX = new System.Windows.Forms.Label();
            this.pBForecast = new System.Windows.Forms.PictureBox();
            this.lPrec = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).BeginInit();
            this.SuspendLayout();
            // 
            // lLonY
            // 
            this.lLonY.AutoSize = true;
            this.lLonY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lLonY.Location = new System.Drawing.Point(768, 150);
            this.lLonY.Name = "lLonY";
            this.lLonY.Size = new System.Drawing.Size(73, 16);
            this.lLonY.TabIndex = 0;
            this.lLonY.Text = "[Y] Lon: 0";
            // 
            // lLatX
            // 
            this.lLatX.AutoSize = true;
            this.lLatX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lLatX.Location = new System.Drawing.Point(768, 134);
            this.lLatX.Name = "lLatX";
            this.lLatX.Size = new System.Drawing.Size(68, 16);
            this.lLatX.TabIndex = 1;
            this.lLatX.Text = "[X] Lat: 0";
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
            this.lPrec.Location = new System.Drawing.Point(768, 180);
            this.lPrec.Name = "lPrec";
            this.lPrec.Size = new System.Drawing.Size(109, 16);
            this.lPrec.TabIndex = 3;
            this.lPrec.Text = "Srážky: 0 [mm]";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ClientSize = new System.Drawing.Size(903, 557);
            this.Controls.Add(this.lPrec);
            this.Controls.Add(this.pBForecast);
            this.Controls.Add(this.lLatX);
            this.Controls.Add(this.lLonY);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pBForecast)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lLonY;
        private System.Windows.Forms.Label lLatX;
        private System.Windows.Forms.PictureBox pBForecast;
        private System.Windows.Forms.Label lPrec;
    }
}

