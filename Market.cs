using Newtonsoft.Json;

namespace Melancholy
{
    public static class Market
    {
        private static Classes.Market marketObj = new();
        private static List<Classes.InventoryItem> inventory = [];

        public static int ItemAmount { get; set; }
        public static int PrestigeLevel { get; set; }

        public static async Task Generate_Market(string market)
        {
            BuildJson();

            if (market != "dlconly" && market != "tempNoCosmetics")
            {
                AddInventoryItem(inventory, Classes.Ids.CosmeticIds, 1);
                AddInventoryItem(inventory, Classes.Ids.OutfitIds, 1);
            }

            AddInventoryItem(inventory, Classes.Ids.DlcIds, 1);

            if (market == "all" || market == "tempNoCosmetics")
            {
                AddInventoryItem(inventory, Classes.Ids.ItemIds, ItemAmount);
                AddInventoryItem(inventory, Classes.Ids.AddonIds, ItemAmount);
                AddInventoryItem(inventory, Classes.Ids.OfferingIds, ItemAmount);
            }

            if (market != "dlconly" && market != "notemp")
                AddInventoryItem(inventory, Classes.Ids.PerkIds, 3);

            var json = JsonConvert.SerializeObject(marketObj, Formatting.Indented);

            var fileNameDict = new Dictionary<string, string>
            {
                { "all", "Market.json" },
                { "dlconly", "MarketDlcOnly.json" },
                { "notemp", "MarketNoSavefile.json" },
                { "tempNoCosmetics", "MarketTempWithNoCosmetics.json" },
                { "perks", "MarketWithPerks.json" }
            };

            if (fileNameDict.TryGetValue(market, out var fileName))
                await File.WriteAllTextAsync($"Files/{fileName}", json);
        }

        private static void BuildJson()
        {
            inventory = [];

            marketObj = new Classes.Market
            {
                Code = 200,
                Message = "OK",
                Data = new Classes.MarketData
                {
                    PlayerId = "MarketUpdater V2",
                    Updated = DateTime.Now.ToString("MM/dd/yyyy"),
                    Comment = "Market created by MarketUpdater V2",
                    DeveloperDiscord = "@bhvr",
                    Inventory = inventory
                }
            };
        }

        private static void AddInventoryItem(List<Classes.InventoryItem> inventory, IEnumerable<object> items, int count)
        {
            inventory.AddRange(items.Select(item =>
            {
                string id = item.ToString() ?? string.Empty;

                if (item is Classes.Character character)
                    id = character.CharacterName;
                else if (item is Classes.ItemOfferingPerk itemOfferingPerk)
                    id = itemOfferingPerk.ItemId;
                else if (item is Classes.ItemAddon addon)
                    id = addon.ItemId;
                else if (item is Classes.Customization customization)
                    id = customization.CosmeticId;
                else if (item is Classes.Outfit outfit)
                    id = outfit.OutfitId;

                return new Classes.InventoryItem
                {
                    LastUpdatedAt = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    ObjectId = id,
                    Quantity = (count == 0 ? new Random().Next(8, 88) : count)
                };
            }));
        }
    }
}
