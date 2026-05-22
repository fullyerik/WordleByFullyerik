using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WordleByFullyerik
{

    public class WordEntry
    {
        [JsonPropertyName("word")]
        public string Word { get; set; } = "";
    }

    public class LeaderboardEntry
    {
        [JsonPropertyName("player_name")]
        public string PlayerName { get; set; } = "";

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("games_played")]
        public int GamesPlayed { get; set; }

        [JsonPropertyName("total_attempts")]
        public int TotalAttempts { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        [JsonIgnore]
        public double AverageAttempts => GamesPlayed > 0
            ? (double)TotalAttempts / GamesPlayed
            : 0;
    }

    public static class SupabaseService
    {

        public const string SupabaseUrl = "https://tmtrjvamawnthzjfhbhe.supabase.co";
        public const string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRtdHJqdmFtYXdudGh6amZoYmhlIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Nzc2MjQ3ODUsImV4cCI6MjA5MzIwMDc4NX0.mK7h13MXvjp8vk7d_6m_nFNkG6QA7GPO3UYGegwy-7k";

        private static readonly HttpClient client;

        static SupabaseService()
        {
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            client.DefaultRequestHeaders.Add("apikey", SupabaseKey);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {SupabaseKey}");
        }

        public static async Task<List<string>> GetAllWordsAsync()
        {
            string url = $"{SupabaseUrl}/rest/v1/words?select=word";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var entries = JsonSerializer.Deserialize<List<WordEntry>>(json) ?? new List<WordEntry>();

            var list = new List<string>();
            foreach (var entry in entries)
            {
                list.Add(entry.Word.ToUpper());
            }
            return list;
        }

        public static async Task AddWordAsync(string word)
        {
            string upper = word.Trim().ToUpper();
            if (upper.Length != 5)
            {
                throw new ArgumentException("Das Wort muss genau 5 Buchstaben haben.");
            }

            string url = $"{SupabaseUrl}/rest/v1/words";
            string json = JsonSerializer.Serialize(new { word = upper });

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            content.Headers.Add("Prefer", "return=minimal");

            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }

        public static async Task DeleteWordAsync(string word)
        {
            string upper = word.Trim().ToUpper();
            string url = $"{SupabaseUrl}/rest/v1/words?word=eq.{Uri.EscapeDataString(upper)}";
            var response = await client.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        public static async Task<List<LeaderboardEntry>> GetLeaderboardAsync()
        {
            string url = $"{SupabaseUrl}/rest/v1/leaderboard?select=*&order=points.desc";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new List<LeaderboardEntry>();
        }

        public static async Task<LeaderboardEntry?> GetPlayerAsync(string playerName)
        {
            string url = $"{SupabaseUrl}/rest/v1/leaderboard?player_name=eq.{Uri.EscapeDataString(playerName)}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new List<LeaderboardEntry>();
            return list.Count > 0 ? list[0] : null;
        }

        public static async Task UpdatePlayerScoreAsync(string playerName, bool won, int attempts)
        {

            int points = won ? (7 - attempts) : 0;

            int attemptsToAdd = won ? attempts : 6;

            var existing = await GetPlayerAsync(playerName);

            if (existing == null)
            {

                var newEntry = new
                {
                    player_name = playerName,
                    wins = won ? 1 : 0,
                    losses = won ? 0 : 1,
                    games_played = 1,
                    total_attempts = attemptsToAdd,
                    points = points
                };

                string json = JsonSerializer.Serialize(newEntry);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Add("Prefer", "return=minimal");

                var response = await client.PostAsync($"{SupabaseUrl}/rest/v1/leaderboard", content);
                response.EnsureSuccessStatusCode();
            }
            else
            {

                var updated = new
                {
                    wins = existing.Wins + (won ? 1 : 0),
                    losses = existing.Losses + (won ? 0 : 1),
                    games_played = existing.GamesPlayed + 1,
                    total_attempts = existing.TotalAttempts + attemptsToAdd,
                    points = existing.Points + points
                };

                string json = JsonSerializer.Serialize(updated);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                content.Headers.Add("Prefer", "return=minimal");

                string url = $"{SupabaseUrl}/rest/v1/leaderboard?player_name=eq.{Uri.EscapeDataString(playerName)}";
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
