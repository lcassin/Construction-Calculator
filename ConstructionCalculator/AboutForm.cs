using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace ConstructionCalculator
{
    public class AboutForm : MaterialForm
    {
        public AboutForm()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            try
            {
                this.Icon = new Icon("Assets/SmallTile.scale-100.ico");
            }
            catch
            {
            }
        }

        private void InitializeComponent()
        {
            this.Text = "About Construction Calculator";
            this.ClientSize = new Size(450, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label titleLabel = new Label
            {
                Text = "Construction Calculator",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 80),
                Size = new Size(410, 40),
                ForeColor = Color.FromArgb(55, 71, 79)
            };
            this.Controls.Add(titleLabel);

            Label versionLabel = new Label
            {
                Text = "Version 1.0",
                Font = new Font("Segoe UI", 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 130),
                Size = new Size(410, 25),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            this.Controls.Add(versionLabel);

            Label descriptionLabel = new Label
            {
                Text = "A professional calculator for construction measurements\n" +
                       "with feet-inches support and fractions to 1/16\" precision.\n\n" +
                       "Features include:\n" +
                       "• Basic calculations with chainable operations\n" +
                       "• Angle Calculator with trigonometry solver\n" +
                       "• Stair Calculator with code compliance\n" +
                       "• Survey Calculator for bearing/azimuth\n" +
                       "• Seating Layout Calculator for auditoriums",
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.TopLeft,
                Location = new Point(40, 170),
                Size = new Size(370, 160),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            this.Controls.Add(descriptionLabel);

            Label authorLabel = new Label
            {
                Text = "Developed for construction professionals",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 340),
                Size = new Size(410, 20),
                ForeColor = Color.FromArgb(120, 120, 120)
            };
            this.Controls.Add(authorLabel);

            Button closeButton = new Button
            {
                Text = "OK",
                Location = new Point(175, 365),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(96, 125, 139),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            closeButton.Click += (s, e) => this.Close();
            this.Controls.Add(closeButton);
        }
    }
}
