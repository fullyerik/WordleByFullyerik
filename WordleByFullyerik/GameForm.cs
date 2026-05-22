using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class GameForm : BaseAppForm
    {

        private const int WordLength = 5;
        private const int MaxAttempts = 6;
        private const int BoxSize = 60;
        private const int BoxGap = 8;

        private string secretWord = "";
        private int currentRow = 0;
        private int currentCol = 0;
        private bool gameOver = false;
        private bool wordLoaded = false;
        private readonly string playerName;

        private readonly Label[,] letterBoxes = new Label[MaxAttempts, WordLength];
        private Label statusLabel = null!;
        private Panel gridPanel = null!;
        private Panel keyboardPanel = null!;
        private RoundedButton playAgainButton = null!;

        private readonly System.Collections.Generic.Dictionary<char, RoundedButton> keyButtons
            = new System.Collections.Generic.Dictionary<char, RoundedButton>();

        public GameForm(string playerName)
        {
            this.playerName = playerName;
            this.Text = $"Wordle - {playerName}";
            this.Size = new Size(620, 850);
            this.MinimumSize = new Size(580, 800);

            this.KeyPreview = true;
            this.KeyDown += GameForm_KeyDown;

            BuildContent();

            this.Shown += async (s, e) => await StartNewGameAsync();
        }

        private void BuildContent()
        {

            statusLabel = new Label
            {
                Text = "Wort wird geladen...",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 32),
                Location = new Point(0, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(statusLabel);

            int gridWidth = WordLength * BoxSize + (WordLength - 1) * BoxGap;
            int gridHeight = MaxAttempts * BoxSize + (MaxAttempts - 1) * BoxGap;

            gridPanel = new Panel
            {
                Size = new Size(gridWidth, gridHeight),
                Location = new Point((ContentPanel.ClientSize.Width - gridWidth) / 2, 55),
                Anchor = AnchorStyles.Top
            };
            ContentPanel.Controls.Add(gridPanel);

            for (int row = 0; row < MaxAttempts; row++)
            {
                for (int col = 0; col < WordLength; col++)
                {
                    var box = new Label
                    {
                        Size = new Size(BoxSize, BoxSize),
                        Location = new Point(col * (BoxSize + BoxGap), row * (BoxSize + BoxGap)),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle,
                        Text = ""
                    };
                    letterBoxes[row, col] = box;
                    gridPanel.Controls.Add(box);
                }
            }

            int keyboardY = 55 + gridHeight + 20;
            keyboardPanel = new Panel
            {
                Location = new Point(0, keyboardY),
                Size = new Size(ContentPanel.ClientSize.Width, 180),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(keyboardPanel);

            BuildKeyboard();

            playAgainButton = new RoundedButton
            {
                Text = "🔄  Nochmals spielen",
                Size = new Size(240, 44),
                Location = new Point((ContentPanel.ClientSize.Width - 240) / 2, keyboardY + 190),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                CornerRadius = 12,
                Visible = false,
                Anchor = AnchorStyles.Top
            };
            playAgainButton.Click += async (s, e) => await StartNewGameAsync();
            ContentPanel.Controls.Add(playAgainButton);
        }

        private void BuildKeyboard()
        {
            string[] rows = { "QWERTZUIOP", "ASDFGHJKL", "YXCVBNM" };
            int keyWidth = 44;
            int keyHeight = 52;
            int keyGap = 6;

            for (int r = 0; r < rows.Length; r++)
            {
                string row = rows[r];
                int rowWidth = row.Length * keyWidth + (row.Length - 1) * keyGap;

                if (r == 2)
                {
                    rowWidth += 2 * (keyGap + 70);
                }

                int startX = (keyboardPanel.ClientSize.Width - rowWidth) / 2;
                int y = r * (keyHeight + keyGap);

                int x = startX;

                if (r == 2)
                {
                    var enterBtn = new RoundedButton
                    {
                        Text = "ENTER",
                        Size = new Size(70, keyHeight),
                        Location = new Point(x, y),
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        CornerRadius = 8
                    };
                    enterBtn.Click += (s, e) => { SubmitGuess(); FocusForm(); };
                    keyboardPanel.Controls.Add(enterBtn);
                    keyButtons['\n'] = enterBtn;
                    x += 70 + keyGap;
                }

                foreach (char letter in row)
                {
                    char captured = letter;
                    var btn = new RoundedButton
                    {
                        Text = letter.ToString(),
                        Size = new Size(keyWidth, keyHeight),
                        Location = new Point(x, y),
                        Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                        CornerRadius = 8
                    };
                    btn.Click += (s, e) => { TypeLetter(captured); FocusForm(); };
                    keyboardPanel.Controls.Add(btn);
                    keyButtons[letter] = btn;
                    x += keyWidth + keyGap;
                }

                if (r == 2)
                {
                    var backBtn = new RoundedButton
                    {
                        Text = "⌫",
                        Size = new Size(70, keyHeight),
                        Location = new Point(x, y),
                        Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                        CornerRadius = 8
                    };
                    backBtn.Click += (s, e) => { DeleteLetter(); FocusForm(); };
                    keyboardPanel.Controls.Add(backBtn);
                    keyButtons['\b'] = backBtn;
                }
            }
        }

        private void FocusForm()
        {

            this.Focus();
        }

        private void GameForm_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!wordLoaded || gameOver) return;

            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                char letter = (char)('A' + (e.KeyCode - Keys.A));
                TypeLetter(letter);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            else if (e.KeyCode == Keys.Back)
            {
                DeleteLetter();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SubmitGuess();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void TypeLetter(char letter)
        {
            if (currentCol >= WordLength) return;
            if (currentRow >= MaxAttempts) return;

            letter = char.ToUpper(letter);
            var box = letterBoxes[currentRow, currentCol];
            box.Text = letter.ToString();
            box.ForeColor = ThemeManager.Foreground;

            box.BorderStyle = BorderStyle.Fixed3D;

            currentCol++;
        }

        private void DeleteLetter()
        {
            if (currentCol <= 0) return;

            currentCol--;
            var box = letterBoxes[currentRow, currentCol];
            box.Text = "";
            box.BorderStyle = BorderStyle.FixedSingle;
        }

        private async Task StartNewGameAsync()
        {

            currentRow = 0;
            currentCol = 0;
            gameOver = false;
            wordLoaded = false;
            secretWord = "";
            ResetGrid();
            ResetKeyboardColors();

            playAgainButton.Visible = false;
            statusLabel.Text = "Wort wird geladen...";

            try
            {
                var words = await SupabaseService.GetAllWordsAsync();

                if (words.Count == 0)
                {
                    MessageBox.Show(
                        "Es sind keine Wörter in der Datenbank!\n" +
                        "Bitte füge zuerst Wörter über die Wort-Verwaltung hinzu.",
                        "Keine Wörter",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                var random = new Random();
                secretWord = words[random.Next(words.Count)].ToUpper();

                wordLoaded = true;
                UpdateStatus();
                this.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Keine Verbindung zur Datenbank!\n\n" +
                    "Bitte prüfe deine Internetverbindung und die Supabase-Einstellungen.\n\n" +
                    "Details: " + ex.Message,
                    "Verbindungsfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ResetGrid()
        {
            for (int r = 0; r < MaxAttempts; r++)
            {
                for (int c = 0; c < WordLength; c++)
                {
                    letterBoxes[r, c].Text = "";
                    letterBoxes[r, c].BackColor = ThemeManager.BoxBackground;
                    letterBoxes[r, c].ForeColor = ThemeManager.Foreground;
                    letterBoxes[r, c].BorderStyle = BorderStyle.FixedSingle;
                }
            }
        }

        private void ResetKeyboardColors()
        {
            foreach (var kv in keyButtons)
            {
                if (kv.Key == '\n' || kv.Key == '\b') continue;
                kv.Value.BackColor = ThemeManager.SubtleForeground;
                kv.Value.ForeColor = Color.White;
                kv.Value.Invalidate();
            }
        }

        private void UpdateStatus()
        {
            statusLabel.Text = $"Spieler: {playerName}    |    Versuch {currentRow + 1} / {MaxAttempts}";
        }

        private async void SubmitGuess()
        {
            if (gameOver || !wordLoaded) return;

            if (currentCol < WordLength)
            {
                ShakeCurrentRow();
                statusLabel.Text = $"⚠️ Bitte alle {WordLength} Buchstaben eingeben!";
                return;
            }

            string guess = "";
            for (int i = 0; i < WordLength; i++)
            {
                guess += letterBoxes[currentRow, i].Text;
            }
            guess = guess.ToUpper();

            ColorRow(currentRow, guess);
            UpdateKeyboardColors(guess);

            currentRow++;
            currentCol = 0;

            if (guess == secretWord)
            {
                gameOver = true;
                statusLabel.Text = $"🎉 Gewonnen in {currentRow} Versuch(en)!";
                await EndGameAsync(won: true, attempts: currentRow);
            }

            else if (currentRow >= MaxAttempts)
            {
                gameOver = true;
                statusLabel.Text = $"❌ Verloren! Das Wort war: {secretWord}";
                await EndGameAsync(won: false, attempts: MaxAttempts);
            }
            else
            {
                UpdateStatus();
            }
        }

        private void ShakeCurrentRow()
        {

            for (int c = 0; c < WordLength; c++)
            {
                var box = letterBoxes[currentRow, c];
                box.BackColor = Color.FromArgb(255, 230, 230);
            }

            var timer = new System.Windows.Forms.Timer { Interval = 200 };
            timer.Tick += (s, e) =>
            {
                for (int c = 0; c < WordLength; c++)
                {
                    letterBoxes[currentRow, c].BackColor = ThemeManager.BoxBackground;
                }
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ColorRow(int row, string guess)
        {
            char[] secret = secretWord.ToCharArray();
            bool[] usedInSecret = new bool[WordLength];
            Color[] colors = new Color[WordLength];

            for (int i = 0; i < WordLength; i++)
            {
                if (guess[i] == secret[i])
                {
                    colors[i] = ThemeManager.CorrectColor;
                    usedInSecret[i] = true;
                }
            }

            for (int i = 0; i < WordLength; i++)
            {
                if (colors[i] == ThemeManager.CorrectColor) continue;

                bool foundElsewhere = false;
                for (int j = 0; j < WordLength; j++)
                {
                    if (!usedInSecret[j] && secret[j] == guess[i])
                    {
                        colors[i] = ThemeManager.PresentColor;
                        usedInSecret[j] = true;
                        foundElsewhere = true;
                        break;
                    }
                }
                if (!foundElsewhere)
                {
                    colors[i] = ThemeManager.AbsentColor;
                }
            }

            for (int i = 0; i < WordLength; i++)
            {
                letterBoxes[row, i].BackColor = colors[i];
                letterBoxes[row, i].ForeColor = Color.White;
                letterBoxes[row, i].BorderStyle = BorderStyle.None;
            }
        }

        private void UpdateKeyboardColors(string guess)
        {
            for (int i = 0; i < WordLength; i++)
            {
                char letter = guess[i];
                if (!keyButtons.ContainsKey(letter)) continue;

                var btn = keyButtons[letter];
                Color newColor;

                if (secretWord[i] == letter)
                    newColor = ThemeManager.CorrectColor;
                else if (secretWord.Contains(letter))
                    newColor = ThemeManager.PresentColor;
                else
                    newColor = ThemeManager.AbsentColor;

                if (btn.BackColor == ThemeManager.CorrectColor) continue;
                if (btn.BackColor == ThemeManager.PresentColor && newColor == ThemeManager.AbsentColor) continue;

                btn.BackColor = newColor;
                btn.ForeColor = Color.White;
                btn.Invalidate();
            }
        }

        private async Task EndGameAsync(bool won, int attempts)
        {

            int points = won ? (7 - attempts) : 0;

            string title = won ? "🎉 Gewonnen!" : "❌ Verloren";
            string text = won
                ? $"Du hast das Wort \"{secretWord}\" in {attempts} Versuch(en) erraten!\n\nPunkte: {points}"
                : $"Schade! Das gesuchte Wort war: \"{secretWord}\"\n\nPunkte: 0";

            try
            {
                await SupabaseService.UpdatePlayerScoreAsync(playerName, won, attempts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Das Ergebnis konnte nicht gespeichert werden.\n\n" + ex.Message,
                    "Speicherfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            MessageBox.Show(text, title, MessageBoxButtons.OK,
                won ? MessageBoxIcon.Information : MessageBoxIcon.Exclamation);

            playAgainButton.Visible = true;
        }

        protected override void ApplyContentTheme()
        {
            statusLabel.ForeColor = ThemeManager.Foreground;

            for (int r = 0; r < MaxAttempts; r++)
            {
                for (int c = 0; c < WordLength; c++)
                {
                    var box = letterBoxes[r, c];
                    if (string.IsNullOrEmpty(box.Text))
                    {
                        box.BackColor = ThemeManager.BoxBackground;
                        box.ForeColor = ThemeManager.Foreground;
                    }
                }
            }

            foreach (var kv in keyButtons)
            {
                var btn = kv.Value;
                if (kv.Key == '\n')
                {

                    btn.BackColor = ThemeManager.Accent;
                    btn.ForeColor = Color.White;
                }
                else if (kv.Key == '\b')
                {

                    btn.BackColor = ThemeManager.Accent;
                    btn.ForeColor = Color.White;
                }
                else if (btn.BackColor != ThemeManager.CorrectColor &&
                         btn.BackColor != ThemeManager.PresentColor &&
                         btn.BackColor != ThemeManager.AbsentColor)
                {

                    btn.BackColor = ThemeManager.SubtleForeground;
                    btn.ForeColor = Color.White;
                }
                btn.Invalidate();
            }

            playAgainButton.BackColor = ThemeManager.Accent;
            playAgainButton.ForeColor = Color.White;
        }
    }
}
