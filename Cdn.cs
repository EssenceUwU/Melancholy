namespace Melancholy
{
    public static class Cdn
    {
        public static async Task GetCdnFiles()
        {
            var version = Extras.PromptInput("Enter game version: ");

            var catalog = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/catalog", "Catalog");
            await File.WriteAllTextAsync($"Files/Catalog.json", catalog);
            Console.WriteLine($"Catalog.json generated");

            var killswitch = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/killswitch", "Killswitch");
            await File.WriteAllTextAsync($"Files/Killswitch.json", killswitch);
            Console.WriteLine($"Killswitch.json generated");

            // var catalogIds = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/get-catalog-ids", "CatalogIds");
            // var json = JsonConvert.DeserializeObject<List<string>>(catalogIds);
            // Classes.Ids.CosmeticIds.Clear();
            //
            // foreach (var item in json)
            //     Classes.Ids.CosmeticIds.Add(item);
        }

        private static async Task<string> SendRequest(string version, string url, string file)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "EssenceOwO");
            client.DefaultRequestHeaders.Add("Access-Key", Cue4Parse.CdnAccessKey);
            client.DefaultRequestHeaders.Add("Game-Version", version[0..3]);

            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (responseContent.Contains("access key") || responseContent.Contains("game version")) throw new Exception(
                $"Issue with obtaining {file} file, make sure your game is updated for the latest version and that you provided the version correctly.");

            return responseContent;
        }
    }
}
