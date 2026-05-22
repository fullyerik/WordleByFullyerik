using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class PlayerNameForm : Form
    {
        public string PlayerName { get; private set; } = "";

        private Label titleLabel = null!;
        private Label hintLabel = null!;
        private TextBox nameInput = null!;
        private RoundedButton okButton = null!;
        private RoundedButton cancelButton = null!;

        public PlayerNameForm()
        {
            InitializeUI();
            ApplyTheme();
        }

        private void InitializeUI()
        {
            this.Text = "Spielername";
            this.Size = new Size(460, 290);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Font = new Font("Segoe UI", 10F);

            titleLabel = new Label
            {
                Text = "Spielername eingeben",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(this.ClientSize.Width, 40),
                Location = new Point(0, 25)
            };
            this.Controls.Add(titleLabel);

            hintLabel = new Label
            {
                Text = "Dein Ergebnis wird unter diesem Namen gespeichert.",
                Font = new Font("Segoe UI", 9F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(this.ClientSize.Width, 20),
                Location = new Point(0, 70)
            };
            this.Controls.Add(hintLabel);

            nameInput = new TextBox
            {
                Font = new Font("Segoe UI", 14F),
                Size = new Size(360, 36),
                Location = new Point((this.ClientSize.Width - 360) / 2, 105),
                BorderStyle = BorderStyle.FixedSingle,
                MaxLength = 30,
                TextAlign = HorizontalAlignment.Center
            };
            nameInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    OkClick();
                }
            };
            this.Controls.Add(nameInput);

            okButton = new RoundedButton
            {
                Text = "OK",
                Size = new Size(140, 44),
                Location = new Point(70, 175),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 12
            };
            okButton.Click += (s, e) => OkClick();
            this.Controls.Add(okButton);

            cancelButton = new RoundedButton
            {
                Text = "Abbrechen",
                Size = new Size(140, 44),
                Location = new Point(240, 175),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 12
            };
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkClick()
        {
            string name = nameInput.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Bitte gib einen Namen ein.", "Hinweis",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PlayerName = name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ApplyTheme()
        {
            this.BackColor = ThemeManager.Background;
            titleLabel.ForeColor = ThemeManager.Secondary;
            hintLabel.ForeColor = ThemeManager.SubtleForeground;

            nameInput.BackColor = ThemeManager.BoxBackground;
            nameInput.ForeColor = ThemeManager.Foreground;

            okButton.BackColor = ThemeManager.Accent;
            okButton.ForeColor = Color.White;

            cancelButton.BackColor = ThemeManager.SubtleForeground;
            cancelButton.ForeColor = Color.White;
        }
    }
}
