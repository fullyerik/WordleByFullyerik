using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class WordManagementForm : BaseAppForm
    {
        private Label titleLabel = null!;
        private Label countLabel = null!;
        private ListBox wordList = null!;
        private TextBox newWordInput = null!;
        private RoundedButton addButton = null!;
        private RoundedButton deleteButton = null!;
        private RoundedButton refreshButton = null!;
        private Label hintLabel = null!;

        public WordManagementForm()
        {
            this.Text = "Wort-Verwaltung";
            this.Size = new Size(620, 720);
            this.MinimumSize = new Size(560, 600);

            BuildContent();
            this.Shown += async (s, e) => await LoadWordsAsync();
        }

        private void BuildContent()
        {

            titleLabel = new Label
            {
                Text = "📝  Wort-Verwaltung",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 50),
                Location = new Point(0, 15),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(titleLabel);

            hintLabel = new Label
            {
                Text = "Wörter müssen genau 5 Buchstaben haben.",
                Font = new Font("Segoe UI", 9F),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 22),
                Location = new Point(0, 70),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(hintLabel);

            wordList = new ListBox
            {
                Location = new Point(20, 110),
                Size = new Size(ContentPanel.ClientSize.Width - 40, ContentPanel.ClientSize.Height - 270),
                Font = new Font("Consolas", 12F, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                IntegralHeight = false
            };
            ContentPanel.Controls.Add(wordList);

            countLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9F),
                AutoSize = false,
                Size = new Size(300, 22),
                Location = new Point(20, ContentPanel.ClientSize.Height - 155),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            ContentPanel.Controls.Add(countLabel);

            int inputY = ContentPanel.ClientSize.Height - 125;

            newWordInput = new TextBox
            {
                Location = new Point(20, inputY),
                Size = new Size(280, 36),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                MaxLength = 5,
                CharacterCasing = CharacterCasing.Upper,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                PlaceholderText = "Neues Wort eingeben..."
            };
            newWordInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await AddWordAsync();
                }
            };
            ContentPanel.Controls.Add(newWordInput);

            addButton = new RoundedButton
            {
                Text = "➕  Hinzufügen",
                Location = new Point(310, inputY),
                Size = new Size(150, 36),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                CornerRadius = 10,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            addButton.Click += async (s, e) => await AddWordAsync();
            ContentPanel.Controls.Add(addButton);

            deleteButton = new RoundedButton
            {
                Text = "🗑  Löschen",
                Location = new Point(470, inputY),
                Size = new Size(120, 36),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                CornerRadius = 10,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            deleteButton.Click += async (s, e) => await DeleteSelectedAsync();
            ContentPanel.Controls.Add(deleteButton);

            refreshButton = new RoundedButton
            {
                Text = "🔄  Aktualisieren",
                Location = new Point(20, ContentPanel.ClientSize.Height - 60),
                Size = new Size(180, 42),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                CornerRadius = 12,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            refreshButton.Click += async (s, e) => await LoadWordsAsync();
            ContentPanel.Controls.Add(refreshButton);
        }

        private async Task LoadWordsAsync()
        {
            countLabel.Text = "Lade...";
            wordList.Items.Clear();

            try
            {
                var words = await SupabaseService.GetAllWordsAsync();
                words.Sort();

                foreach (var w in words)
                {
                    wordList.Items.Add(w);
                }

                countLabel.Text = $"{words.Count} Wörter in der Datenbank.";
            }
            catch (Exception ex)
            {
                countLabel.Text = "Fehler beim Laden.";
                MessageBox.Show(
                    "Wörter konnten nicht geladen werden.\n\n" + ex.Message,
                    "Verbindungsfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task AddWordAsync()
        {
            string word = newWordInput.Text.Trim().ToUpper();

            if (string.IsNullOrEmpty(word))
            {
                MessageBox.Show("Bitte gib ein Wort ein.", "Hinweis",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (word.Length != 5)
            {
                MessageBox.Show("Das Wort muss genau 5 Buchstaben haben.", "Falsche Länge",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (char c in word)
            {
                if (!char.IsLetter(c))
                {
                    MessageBox.Show("Das Wort darf nur Buchstaben enthalten.", "Ungültiges Zeichen",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            try
            {
                addButton.Enabled = false;
                await SupabaseService.AddWordAsync(word);
                newWordInput.Text = "";
                await LoadWordsAsync();
                newWordInput.Focus();
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Contains("23505") || ex.Message.Contains("duplicate")
                    ? $"Das Wort \"{word}\" existiert bereits."
                    : "Wort konnte nicht hinzugefügt werden.\n\n" + ex.Message;

                MessageBox.Show(msg, "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                addButton.Enabled = true;
            }
        }

        private async Task DeleteSelectedAsync()
        {
            if (wordList.SelectedItem == null)
            {
                MessageBox.Show("Bitte wähle ein Wort aus der Liste aus.", "Hinweis",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string word = wordList.SelectedItem.ToString() ?? "";
            var result = MessageBox.Show(
                $"Soll das Wort \"{word}\" wirklich gelöscht werden?",
                "Wort löschen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                deleteButton.Enabled = false;
                await SupabaseService.DeleteWordAsync(word);
                await LoadWordsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Wort konnte nicht gelöscht werden.\n\n" + ex.Message,
                    "Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                deleteButton.Enabled = true;
            }
        }

        protected override void ApplyContentTheme()
        {
            titleLabel.ForeColor = ThemeManager.Accent;
            hintLabel.ForeColor = ThemeManager.SubtleForeground;
            countLabel.ForeColor = ThemeManager.SubtleForeground;

            wordList.BackColor = ThemeManager.BoxBackground;
            wordList.ForeColor = ThemeManager.Foreground;

            newWordInput.BackColor = ThemeManager.BoxBackground;
            newWordInput.ForeColor = ThemeManager.Foreground;

            addButton.BackColor = ThemeManager.Accent;
            addButton.ForeColor = Color.White;

            deleteButton.BackColor = Color.FromArgb(180, 70, 60);
            deleteButton.ForeColor = Color.White;

            refreshButton.BackColor = ThemeManager.Accent;
            refreshButton.ForeColor = Color.White;
        }
    }
}
