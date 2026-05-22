using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class LeaderboardForm : BaseAppForm
    {
        private Label titleLabel = null!;
        private DataGridView grid = null!;
        private RoundedButton refreshButton = null!;
        private Label statusLabel = null!;

        public LeaderboardForm()
        {
            this.Text = "Leaderboard";
            this.Size = new Size(820, 620);
            this.MinimumSize = new Size(700, 500);

            BuildContent();
            this.Shown += async (s, e) => await LoadLeaderboardAsync();
        }

        private void BuildContent()
        {

            titleLabel = new Label
            {
                Text = "🏆  Leaderboard",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 50),
                Location = new Point(0, 15),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(titleLabel);

            grid = new DataGridView
            {
                Location = new Point(20, 80),
                Size = new Size(ContentPanel.ClientSize.Width - 40, ContentPanel.ClientSize.Height - 170),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 32 }
            };
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.Columns.Add("Rank", "#");
            grid.Columns.Add("Name", "Spielername");
            grid.Columns.Add("Points", "Punkte");
            grid.Columns.Add("Wins", "Siege");
            grid.Columns.Add("Losses", "Niederlagen");
            grid.Columns.Add("Played", "Spiele");
            grid.Columns.Add("Avg", "Ø Versuche");

            grid.Columns["Rank"]!.FillWeight = 40;
            grid.Columns["Name"]!.FillWeight = 130;
            grid.Columns["Points"]!.FillWeight = 80;
            grid.Columns["Name"]!.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.Columns["Name"]!.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);

            ContentPanel.Controls.Add(grid);

            statusLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9F),
                AutoSize = false,
                Size = new Size(300, 24),
                Location = new Point(20, ContentPanel.ClientSize.Height - 80),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            ContentPanel.Controls.Add(statusLabel);

            refreshButton = new RoundedButton
            {
                Text = "🔄  Aktualisieren",
                Size = new Size(180, 42),
                Location = new Point(ContentPanel.ClientSize.Width - 200, ContentPanel.ClientSize.Height - 60),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                CornerRadius = 12
            };
            refreshButton.Click += async (s, e) => await LoadLeaderboardAsync();
            ContentPanel.Controls.Add(refreshButton);
        }

        private async Task LoadLeaderboardAsync()
        {
            statusLabel.Text = "Lade Daten...";
            grid.Rows.Clear();

            try
            {
                var entries = await SupabaseService.GetLeaderboardAsync();

                if (entries.Count == 0)
                {
                    statusLabel.Text = "Noch keine Spieler im Leaderboard.";
                    return;
                }

                int rank = 1;
                foreach (var e in entries)
                {
                    grid.Rows.Add(
                        rank,
                        e.PlayerName,
                        e.Points,
                        e.Wins,
                        e.Losses,
                        e.GamesPlayed,
                        e.AverageAttempts.ToString("0.00")
                    );
                    rank++;
                }

                statusLabel.Text = $"{entries.Count} Spieler geladen.";
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Fehler beim Laden.";
                MessageBox.Show(
                    "Leaderboard konnte nicht geladen werden.\n\n" + ex.Message,
                    "Verbindungsfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        protected override void ApplyContentTheme()
        {
            titleLabel.ForeColor = ThemeManager.Accent;
            statusLabel.ForeColor = ThemeManager.SubtleForeground;

            grid.BackgroundColor = ThemeManager.Background;
            grid.GridColor = ThemeManager.BoxBorder;

            grid.DefaultCellStyle.BackColor = ThemeManager.BoxBackground;
            grid.DefaultCellStyle.ForeColor = ThemeManager.Foreground;
            grid.DefaultCellStyle.SelectionBackColor = ThemeManager.Accent;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;

            grid.AlternatingRowsDefaultCellStyle.BackColor = ThemeManager.Surface;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = ThemeManager.Foreground;

            grid.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.Accent;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            refreshButton.BackColor = ThemeManager.Accent;
            refreshButton.ForeColor = Color.White;
        }
    }
}
