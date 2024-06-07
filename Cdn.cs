namespace Melancholy
{
    public static class Cdn
    {
        public static async Task GetCdnFiles()
        {
            var version = Extras.PromptInput("Enter game version: ");

            (string, bool) catalog = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/catalog", "Catalog");
            if(catalog.Item2)
            {
                await File.WriteAllTextAsync($"Files/Catalog.json", catalog.Item1);
                Console.WriteLine($"Catalog.json generated");
            } 
            else
            {
                Console.WriteLine(catalog.Item1);
            }

            (string, bool) killswitch = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/killswitch", "Killswitch");
            if(killswitch.Item2)
            {
                await File.WriteAllTextAsync($"Files/Killswitch.json", killswitch.Item1);
                Console.WriteLine($"Killswitch.json generated");
            } else
            {
                Console.WriteLine(killswitch.Item1);
            }

            // var catalogIds = await SendRequest(version, "https://essenceuwu.uk/api/v1/DeadByDaylight/CDN/get-catalog-ids", "CatalogIds");
            // var json = JsonConvert.DeserializeObject<List<string>>(catalogIds);
            // Classes.Ids.CosmeticIds.Clear();
            //
            // foreach (var item in json)
            //     Classes.Ids.CosmeticIds.Add(item);
        }

        private static async Task<(string, bool)> SendRequest(string version, string url, string file)
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
            client.DefaultRequestHeaders.Add("User-Agent", "EssenceOwO");
            client.DefaultRequestHeaders.Add("Access-Key", Cue4Parse.CdnAccessKey);
            client.DefaultRequestHeaders.Add("Game-Version", version[0..3]);

            try
            {
                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (responseContent.Contains("access key") || responseContent.Contains("game version"))
                {
                    return ($"Issue with obtaining {file} file, make sure your game is updated for the latest version and that you provided the version correctly.", false);
                }

                return (responseContent, true);
            }
            catch (TaskCanceledException ex)
            {
                if (ex.CancellationToken.IsCancellationRequested)
                {
                    return ($"The request to get {file} file failed due to manual cancellation.", false);
                }
                else
                {
                    return ($"The request to get {file} file failed due to timeout.", false);
                }
            }
        }
    }
}
