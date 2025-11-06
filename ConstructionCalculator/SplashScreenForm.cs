using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConstructionCalculator
{
    public class SplashScreenForm : Form
    {
        private System.Windows.Forms.Timer closeTimer;

        public SplashScreenForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(500, 300);
            this.BackColor = Color.FromArgb(55, 71, 79);
            this.ShowInTaskbar = false;

            try
            {
                this.Icon = new Icon("Assets/SmallTile.scale-100.ico");
            }
            catch
            {
            }

            Label titleLabel = new Label
            {
                Text = "Construction Calculator",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 80),
                Size = new Size(500, 50),
                BackColor = Color.Transparent
            };
            this.Controls.Add(titleLabel);

            Label versionLabel = new Label
            {
                Text = "Version 1.0",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 140),
                Size = new Size(500, 30),
                BackColor = Color.Transparent
            };
            this.Controls.Add(versionLabel);

            Label subtitleLabel = new Label
            {
                Text = "Feet-Inches Calculator with Professional Tools",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(200, 200, 200),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 180),
                Size = new Size(500, 30),
                BackColor = Color.Transparent
            };
            this.Controls.Add(subtitleLabel);

            Label loadingLabel = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(179, 229, 252),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 240),
                Size = new Size(500, 30),
                BackColor = Color.Transparent
            };
            this.Controls.Add(loadingLabel);

            closeTimer = new System.Windows.Forms.Timer
            {
                Interval = 2000
            };
            closeTimer.Tick += (s, e) =>
            {
                closeTimer.Stop();
                this.Close();
            };
            closeTimer.Start();
        }
    }
}
