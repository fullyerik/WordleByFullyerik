using System.Drawing;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class HelpForm : BaseAppForm
    {
        private Label titleLabel = null!;
        private RichTextBox helpText = null!;

        public HelpForm()
        {
            this.Text = "Hilfe";
            this.Size = new Size(640, 720);
            this.MinimumSize = new Size(560, 600);

            BuildContent();
        }

        private void BuildContent()
        {

            titleLabel = new Label
            {
                Text = "❓  Hilfe & Spielregeln",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(ContentPanel.ClientSize.Width, 50),
                Location = new Point(0, 15),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ContentPanel.Controls.Add(titleLabel);

            helpText = new RichTextBox
            {
                Location = new Point(20, 80),
                Size = new Size(ContentPanel.ClientSize.Width - 40, ContentPanel.ClientSize.Height - 100),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11F),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            FillHelpText();
            ContentPanel.Controls.Add(helpText);
        }

        private void FillHelpText()
        {
            helpText.Clear();

            AppendHeader("Was ist Wordle?");
            AppendNormal(
                "Wordle ist ein Wortspiel. Du musst ein geheimes Wort mit 5 Buchstaben " +
                "erraten und hast dafür 6 Versuche.\n\n");

            AppendHeader("So spielst du");
            AppendNormal(
                "1. Tippe einen Buchstaben mit der Tastatur (oder mit der virtuellen Tastatur unten).\n" +
                "2. Mit ⌫ (Backspace) löschst du den letzten Buchstaben.\n" +
                "3. Mit ENTER bestätigst du dein Wort, sobald alle 5 Buchstaben da sind.\n" +
                "4. Die Buchstaben werden farbig angezeigt:\n");

            AppendColored("    🟩 Grün", Color.FromArgb(83, 141, 78));
            AppendNormal("  = Buchstabe ist richtig und an der richtigen Stelle.\n");

            AppendColored("    🟨 Gelb", Color.FromArgb(181, 159, 59));
            AppendNormal("  = Buchstabe ist im Wort, aber an der falschen Stelle.\n");

            AppendColored("    ⬛ Grau", Color.FromArgb(120, 124, 126));
            AppendNormal("  = Buchstabe kommt im Wort nicht vor.\n\n");

            AppendNormal(
                "5. Du hast 6 Versuche, um das Wort zu erraten.\n" +
                "6. Errätst du das Wort, gewinnst du. Sonst verlierst du.\n\n");

            AppendHeader("Punkte");
            AppendNormal(
                "• Gewonnen in 1 Versuch  = 6 Punkte\n" +
                "• Gewonnen in 2 Versuchen = 5 Punkte\n" +
                "• Gewonnen in 3 Versuchen = 4 Punkte\n" +
                "• Gewonnen in 4 Versuchen = 3 Punkte\n" +
                "• Gewonnen in 5 Versuchen = 2 Punkte\n" +
                "• Gewonnen in 6 Versuchen = 1 Punkt\n" +
                "• Verloren = 0 Punkte\n\n");

            AppendHeader("Bedienung der App");
            AppendNormal(
                "• In der oberen Leiste findest du:\n" +
                "    ← Menü   – zurück zum Hauptmenü\n" +
                "    🌙 / ☀  – Hell-/Dunkelmodus umschalten\n" +
                "    —         – Fenster minimieren\n" +
                "    ✕         – Fenster schließen\n" +
                "• Du kannst das Fenster oben in der Leiste mit der Maus verschieben.\n" +
                "• Im Menü auf \"Spiel starten\" klicken und Spielernamen eingeben.\n" +
                "• Im Spiel kannst du sowohl die echte Tastatur als auch die Tasten unten benutzen.\n" +
                "• In der \"Wort-Verwaltung\" kannst du eigene Wörter hinzufügen oder löschen.\n\n");

            AppendHeader("Hinweis zur Internetverbindung");
            AppendNormal(
                "Diese App benutzt eine Online-Datenbank (Supabase) für Wörter und Leaderboard. " +
                "Ohne Internetverbindung erscheint eine Fehlermeldung.\n");

            helpText.SelectionStart = 0;
            helpText.ScrollToCaret();
        }

        private void AppendHeader(string text)
        {
            helpText.SelectionStart = helpText.TextLength;
            helpText.SelectionLength = 0;
            helpText.SelectionFont = new Font("Segoe UI", 13F, FontStyle.Bold);
            helpText.SelectionColor = ThemeManager.Accent;
            helpText.AppendText(text + "\n");
            helpText.SelectionFont = new Font("Segoe UI", 11F);
            helpText.SelectionColor = ThemeManager.Foreground;
        }

        private void AppendNormal(string text)
        {
            helpText.SelectionStart = helpText.TextLength;
            helpText.SelectionLength = 0;
            helpText.SelectionFont = new Font("Segoe UI", 11F);
            helpText.SelectionColor = ThemeManager.Foreground;
            helpText.AppendText(text);
        }

        private void AppendColored(string text, Color color)
        {
            helpText.SelectionStart = helpText.TextLength;
            helpText.SelectionLength = 0;
            helpText.SelectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
            helpText.SelectionColor = color;
            helpText.AppendText(text);
        }

        protected override void ApplyContentTheme()
        {
            titleLabel.ForeColor = ThemeManager.Accent;

            helpText.BackColor = ThemeManager.BoxBackground;
            helpText.ForeColor = ThemeManager.Foreground;

            FillHelpText();
        }
    }
}
