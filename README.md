# 🎮 Wordle By FULLYERIK – Schnellstart

So bringst du das Projekt nach dem Entpacken zum Laufen.

---

## 📂 Was ist im ZIP?

```
WordleByFullyerik/                      ← dieser Ordner
├── WordleByFullyerik.sln               ← Solution-Datei (hiermit öffnen!)
├── supabase_setup.sql                  ← SQL für die Online-Datenbank
├── START_HIER.md                       ← diese Anleitung
└── WordleByFullyerik/                  ← der eigentliche Projekt-Ordner
    ├── WordleByFullyerik.csproj
    ├── Program.cs
    ├── BaseAppForm.cs
    ├── MainMenuForm.cs
    ├── GameForm.cs
    ├── PlayerNameForm.cs
    ├── LeaderboardForm.cs
    ├── WordManagementForm.cs
    ├── HelpForm.cs
    ├── RoundedButton.cs
    ├── ThemeManager.cs
    └── SupabaseService.cs
```

---

## 🚀 Schritt 1: Entpacken

Entpacke das ZIP irgendwohin, z.B. nach `C:\Daten\IMS-1a\`.
**Wichtig:** Den kompletten Ordner entpacken, nicht nur einzelne Dateien rausziehen.

## 🚀 Schritt 2: In Rider öffnen

1. Rider öffnen
2. **"Open"** klicken
3. Die Datei **`WordleByFullyerik.sln`** auswählen (NICHT den Ordner!)
4. **"Trust Project"** klicken falls gefragt
5. Kurz warten bis Rider fertig geladen hat (unten rechts läuft die Anzeige)

> 💡 In Visual Studio 2022 geht es genauso: Doppelklick auf `WordleByFullyerik.sln`.

## 🚀 Schritt 3: Supabase-Zugangsdaten eintragen

Öffne die Datei **`SupabaseService.cs`** und trage ganz oben deine Daten ein:

```csharp
private const string SupabaseUrl = "DEIN-PROJEKT.supabase.co";   // OHNE https://
private const string SupabaseKey = "DEIN-ANON-KEY";              // anon public key
```

Wo du die findest: Supabase → Settings ⚙ → API → "Project URL" und "anon public".

## 🚀 Schritt 4: Datenbank einrichten (nur beim ersten Mal)

Falls deine Supabase-Tabellen noch leer / nicht vorhanden sind:

1. Supabase → **SQL Editor** → **New query**
2. Inhalt von **`supabase_setup.sql`** reinkopieren
3. **Run** klicken

Das erstellt die Tabellen, lädt 76 Wörter und schaltet die Sicherheitssperre (RLS) ab.

## 🚀 Schritt 5: Starten!

In Rider oben rechts auf den **grünen Play-Button ▶️** klicken (oder `Shift + F10`).

🎉 Fertig – das Hauptmenü öffnet sich!

---

## 🆘 Falls etwas nicht geht

**"Wort konnte nicht hinzugefügt werden – 401 Unauthorized"**
→ Die Sicherheitssperre (RLS) ist noch aktiv. Im Supabase SQL Editor ausführen:
```sql
ALTER TABLE words DISABLE ROW LEVEL SECURITY;
ALTER TABLE leaderboard DISABLE ROW LEVEL SECURITY;
```

**"Keine Verbindung zur Datenbank"**
→ Hast du in `SupabaseService.cs` die zwei Zeilen mit deinen echten Daten ersetzt?
→ Internetverbindung da?

**Build-Fehler mit "AssemblyInfo doppelt"**
→ Lösch die Ordner `bin` und `obj` im Projektordner und starte neu.

**Rider zeigt "Sonstige Dateien" / "Add Configuration"**
→ Du hast den Ordner statt der `.sln` geöffnet. Schließen und nochmal die `.sln` öffnen.
