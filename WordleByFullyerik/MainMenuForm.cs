using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class MainMenuForm : BaseAppForm
    {

        private Label titleLabel = null!;
        private Label subtitleLabel = null!;
        private RoundedButton playButton = null!;
        private RoundedButton leaderboardButton = null!;
        private RoundedButton wordManagerButton = null!;
        private RoundedButton helpButton = null!;
        private RoundedButton exitButton = null!;

        public MainMenuForm()
        {

            this.IsMainMenu = true;

            this.Text = "Wordle By FULLYERIK";
            this.Size = new Size(720, 820);

            BuildContent();
        }

        private void BuildContent()
        {

            titleLabel = new Label
            {
                Text = "WORDLE",
                Font = new Font("Segoe UI", 64F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 110),
                Location = new Point(0, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(titleLabel);

            subtitleLabel = new Label
            {
                Text = "By FULLYERIK",
                Font = new Font("Segoe UI", 18F, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 40),
                Location = new Point(0, 170),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(subtitleLabel);

            playButton = CreateMenuButton("▶  Spiel starten", 270);
            playButton.Click += (s, e) => OpenPlayerNameForm();
            ContentPanel.Controls.Add(playButton);

            leaderboardButton = CreateMenuButton("🏆  Leaderboard", 345);
            leaderboardButton.Click += (s, e) => OpenSubForm(new LeaderboardForm());
            ContentPanel.Controls.Add(leaderboardButton);

            wordManagerButton = CreateMenuButton("📝  Wort-Verwaltung", 420);
            wordManagerButton.Click += (s, e) => OpenSubForm(new WordManagementForm());
            ContentPanel.Controls.Add(wordManagerButton);

            helpButton = CreateMenuButton("❓  Hilfe", 495);
            helpButton.Click += (s, e) => OpenSubForm(new HelpForm());
            ContentPanel.Controls.Add(helpButton);

            exitButton = CreateMenuButton("✖  Spiel beenden", 570);
            exitButton.Click += (s, e) => this.Close();
            ContentPanel.Controls.Add(exitButton);
        }

        private void OpenSubForm(Form subForm)
        {
            this.Hide();
            subForm.FormClosed += (s, e) => this.Show();
            subForm.Show();
        }

        private RoundedButton CreateMenuButton(string text, int top)
        {
            int width = 420;
            return new RoundedButton
            {
                Text = text,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Size = new Size(width, 60),
                Location = new Point((ContentPanel.ClientSize.Width - width) / 2, top),
                Anchor = AnchorStyles.Top,
                CornerRadius = 14
            };
        }

        private void OpenPlayerNameForm()
        {
            using var nameForm = new PlayerNameForm();
            if (nameForm.ShowDialog(this) == DialogResult.OK)
            {

                this.Hide();
                var game = new GameForm(nameForm.PlayerName);
                game.FormClosed += (s, e) => this.Show();
                game.Show();
            }
        }

        protected override void ApplyContentTheme()
        {
            titleLabel.ForeColor = ThemeManager.Accent;
            subtitleLabel.ForeColor = ThemeManager.SubtleForeground;

            var menuButtons = new[]
            {
                playButton, leaderboardButton, wordManagerButton, helpButton, exitButton
            };
            foreach (var btn in menuButtons)
            {
                btn.BackColor = ThemeManager.Accent;
                btn.ForeColor = Color.White;
                btn.Invalidate();
            }
        }
    }
}
