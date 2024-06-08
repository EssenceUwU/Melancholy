using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace Melancholy
{
    public static partial class Cue4Parse
    {
        private static DefaultFileProvider? Provider { get; set; }
        public static string? CdnAccessKey { get; set; }

        public static void Initialize()
        {
            Provider = new DefaultFileProvider(Extras.Settings.PakPath, SearchOption.AllDirectories, true, new VersionContainer(EGame.GAME_DeadbyDaylight));
            Provider.Initialize();
            Provider.SubmitKey(new FGuid(), new FAesKey(Extras.Settings.AesKey));
            Provider.LoadLocalization();
            Provider.MappingsContainer = new FileUsmapTypeMappingsProvider(Extras.Settings.MappingsPath);
        }

        public static string GetAccessKey()
        {
            Provider.TrySaveAsset("/Game/Config/DefaultGame.ini", out var data);
            var lastLine = "";

            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream))
            {
                while (reader.ReadLine() is { } line)
                    if (line.Contains("_live"))
                        lastLine = line;
            }

            var match = MyRegex().Match(lastLine);
            return match.Groups[1].Value;
        }

        public static void Get_Files()
        {
            foreach (var kvp in Provider.Files)
            {
                switch (kvp.Value.Name)
                {
                    case "CustomizationItemDB.uasset":
                        Classes.FilePaths.CustomizationItemDb.Add(kvp.Value.Path); break;
                    case "OutfitDB.uasset":
                        Classes.FilePaths.OutfitDb.Add(kvp.Value.Path); break;
                    case "CharacterDescriptionDB.uasset":
                        Classes.FilePaths.CharacterDescriptionDb.Add(kvp.Value.Path); break;
                    case "ItemDB.uasset":
                        Classes.FilePaths.ItemDb.Add(kvp.Value.Path); break;
                    case "ItemAddonDB.uasset":
                        Classes.FilePaths.ItemAddonDb.Add(kvp.Value.Path); break;
                    case "OfferingDB.uasset":
                        Classes.FilePaths.OfferingDb.Add(kvp.Value.Path); break;
                    case "PerkDB.uasset":
                        Classes.FilePaths.PerkDb.Add(kvp.Value.Path); break;
                }

                /*/* Alternative method for adding cosmetics can be found in Cdn.cs - simply comment this part out
                 and uncomment in Cdn.cs - only necessary if BHVR decide to change game files to break this
                 #1#
                bool hasNestedIconsFolder = kvp.Value.Path.Split('/')
                                                .SkipWhile(part => part != "PlayerCards")
                                                .Any(sub => sub == "Icons" || sub == "HF2") &&
                                            !kvp.Value.NameWithoutExtension.EndsWith("_icon");

                bool isCustomizationOrPlayerCardsPath = kvp.Value.Path.Contains("UMGAssets/Icons/Customization") ||
                                                        kvp.Value.Path.Contains("UMGAssets/Icons/PlayerCards") &&
                                                        hasNestedIconsFolder;

                if (isCustomizationOrPlayerCardsPath) Classes.Ids.CosmeticIds.Add(kvp.Value.NameWithoutExtension);*/
            }

            Add_Values(Classes.FilePaths.CustomizationItemDb, "CustomizationItemDB");
            Add_Values(Classes.FilePaths.OutfitDb, "OutfitDB");
            Add_Values(Classes.FilePaths.CharacterDescriptionDb, "CharacterDescriptionDB");
            Add_Values(Classes.FilePaths.ItemDb, "ItemDB");
            Add_Values(Classes.FilePaths.ItemAddonDb, "ItemAddonDB");
            Add_Values(Classes.FilePaths.OfferingDb, "OfferingDB");
            Add_Values(Classes.FilePaths.PerkDb, "PerkDB");
        }

        private static List<string> exclusiveStrings = new List<string>
        {
            "Twitch",
            "PandaTV",
            "CartoonZ",
            "Smile",
            "AdmBahroo",
            "BloodLetting",
            "KingKong",
            "Bryce",
            "AngryPug",
            "Sxyhxy",
            "72hrs",
            "Crew01",
            "ME_007",
            "ME_008",
            "AP_008",
            "UN_Charm005",
            "UN_Charm003",
            "BA_BDG_99",
            "BA_BNR_99",
            "HU_041",
            "UN_Charm004",
            "ZO_BNR_09",
            "ZO_BDG_11",
            "DF_Torso03_01",
            "Wraith_Body_Ohmwrecker"
        };

        private static void Add_Values(List<string> list, string type)
        {
            foreach (var item in list)
            {
                var export =
                    JsonConvert.DeserializeObject<dynamic>(
                        JsonConvert.SerializeObject(Provider.LoadAllObjects(item))) ?? new ExpandoObject();

                foreach (JProperty p in export[0]?.Rows ?? Enumerable.Empty<JProperty>())
                {
                    var properties = p.Values<JObject>();
                    foreach (var property in properties)
                    {
                        switch (type)
                        {
                            case "CustomizationItemDB":
                                string customizationId = property?["customizationId"]?.ToString();
                                string localizedString =
                                    property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString();
                                
                                Classes.Customization customization = new()
                                {
                                    CosmeticId = property?["customizationId"]?.ToString() ?? string.Empty,
                                    CosmeticName =
                                        property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ??
                                        string.Empty,
                                    CosmeticDescription =
                                        property?["UIData"]?["Description"]?["LocalizedString"]?.ToString() ??
                                        string.Empty,
                                    Category = property?["category"]?.ToString() ?? string.Empty,
                                    AssociatedCharacterIndex =
                                        property?["AssociatedCharacter"]?.ToString() ?? string.Empty,
                                    Rarity = property?["Rarity"]?.ToString() ?? string.Empty,
                                    IsInStore = property?["IsInStore"]?.ToString() ?? string.Empty,
                                    EventId = property?["eventID"]?.ToString() ?? string.Empty,
                                    Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ??
                                                   string.Empty,
                                    FilePath = item ?? string.Empty,
                                    IsLegacy = (property?["InclusionVersion"]?.ToString() == "Legacy"
                                                && Regex.IsMatch(property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString(), @"Legacy.*\b(I|II|III)\b", RegexOptions.IgnoreCase)),
                                    IsExclusive = (exclusiveStrings.Any(s => customizationId.Contains(s)
                                                                             && !customizationId.Contains("Charity")))
                                                  || (localizedString.Contains("Twitchy")
                                                      && !(localizedString.Contains("Flame") || localizedString.Contains("Medkit")))
                                };
                                if (!IsInBlacklist(customization.CosmeticId)) Classes.Ids.CosmeticIds.Add(customization);
                                break;
                            case "OutfitDB":
                                Classes.Outfit outfit = new()
                                {
                                    OutfitId = property?["ID"]?.ToString() ?? string.Empty,
                                    OutfitName = property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ??
                                                 string.Empty,
                                    OutfitDescription =
                                        property?["UIData"]?["Description"]?["LocalizedString"]?.ToString() ??
                                        string.Empty,
                                    CollectionName = property?["CollectionName"]?["LocalizedString"]?.ToString() ??
                                                     string.Empty,
                                    Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ??
                                                   string.Empty,
                                    FilePath = item ?? string.Empty
                                };
                                if (!IsInBlacklist(outfit.OutfitId)) Classes.Ids.OutfitIds.Add(outfit);
                                break;
                            case "CharacterDescriptionDB":
                                if (property?["CharacterId"]?.ToString() == "None") continue;
                                Classes.Character character = new()
                                {
                                    CharacterName = property?["CharacterId"]?.ToString() ?? string.Empty,
                                    CharacterIndex = property?["characterIndex"]?.ToString() ?? string.Empty,
                                    CharacterType = property?["Role"]?.ToString() ?? string.Empty,
                                    CharacterDefaultItem = property?["DefaultItem"]?.ToString() ?? string.Empty,
                                    Name = property?["DisplayName"]?["LocalizedString"]?.ToString() ?? string.Empty,
                                    FilePath = item ?? string.Empty
                                };
                                Classes.Ids.DlcIds.Add(character);
                                break;
                            case "ItemDB":
                                if (property?["Type"]?.ToString() != "EInventoryItemType::Power")
                                {
                                    Classes.ItemOfferingPerk itemData = new()
                                    {
                                        ItemId = property?["ItemId"]?.ToString() ?? string.Empty,
                                        CharacterType = property?["Role"]?.ToString() ?? string.Empty,
                                        Rarity = property?["Rarity"]?.ToString() ?? string.Empty,
                                        Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ?? string.Empty,
                                        Name = property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ?? string.Empty,
                                        FilePath = item ?? string.Empty,
                                        ShouldBeInInventory = property?["Inventory"]?.Value<bool>() ?? true,
                                        EventId = property?["eventID"]?.ToString() ?? string.Empty
                                    };
                                    if (!IsInBlacklist(itemData.ItemId)) Classes.Ids.ItemIds.Add(itemData);
                                }
                                break;
                            case "ItemAddonDB":
                                Classes.ItemAddon itemAddon = new()
                                {
                                    ItemId = property?["ItemId"]?.ToString() ?? string.Empty,
                                    CharacterType = property?["Role"]?.ToString() ?? string.Empty,
                                    CharacterDefaultItem = property?["ParentItem"]?["itemIds"]?.Count() > 0 ? (property?["ParentItem"]?["itemIds"]?[0]?.ToString() ?? string.Empty) : string.Empty,
                                    Rarity = property?["Rarity"]?.ToString() ?? string.Empty,
                                    Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ?? string.Empty,
                                    Name = property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ?? string.Empty,
                                    FilePath = item ?? string.Empty,
                                    ShouldBeInInventory = property?["Inventory"]?.Value<bool>() ?? true,
                                    EventId = property?["eventID"]?.ToString() ?? string.Empty
                                };
                                if (!IsInBlacklist(itemAddon.ItemId)) Classes.Ids.AddonIds.Add(itemAddon);
                                break;
                            case "OfferingDB":
                                Classes.ItemOfferingPerk offering = new()
                                {
                                    ItemId = property?["ItemId"]?.ToString() ?? string.Empty,
                                    CharacterType = property?["Role"]?.ToString() ?? string.Empty,
                                    Rarity = property?["Rarity"]?.ToString() ?? string.Empty,
                                    Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ?? string.Empty,
                                    Name = property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ?? string.Empty,
                                    FilePath = item ?? string.Empty,
                                    ShouldBeInInventory = property?["Inventory"]?.Value<bool>() ?? true,
                                    EventId = property?["eventID"]?.ToString() ?? string.Empty
                                };
                                if (!IsInBlacklist(offering.ItemId)) Classes.Ids.OfferingIds.Add(offering);
                                break;
                            case "PerkDB":
                                Classes.ItemOfferingPerk perk = new()
                                {
                                    ItemId = property?["ItemId"]?.ToString() ?? string.Empty,
                                    CharacterType = property?["Role"]?.ToString() ?? string.Empty,
                                    Rarity = property?["Rarity"]?.ToString() ?? string.Empty,
                                    Availability = property?["Availability"]?["ItemAvailability"]?.ToString() ?? string.Empty,
                                    Name = property?["UIData"]?["DisplayName"]?["LocalizedString"]?.ToString() ?? string.Empty,
                                    FilePath = item ?? string.Empty,
                                    ShouldBeInInventory = property?["Inventory"]?.Value<bool>() ?? true,
                                    EventId = property?["eventID"]?.ToString() ?? string.Empty
                                };
                                if (!IsInBlacklist(perk.ItemId)) Classes.Ids.PerkIds.Add(perk);
                                break;
                        }
                    }
                }
            }
        }

        public static bool IsListEmpty()
        {
            return Classes.Ids.CosmeticIds.Count == 0;
        }

        private static bool IsInBlacklist(string id)
        {
            if (!File.Exists("blacklist.json"))
            {
                var blacklist = new
                {
                    IDs = new List<string>()
                {
                    "Item_LamentConfiguration"
                }
                };

                File.WriteAllText("blacklist.json", JsonConvert.SerializeObject(blacklist, Formatting.Indented));
            }

            string blacklistContent = File.ReadAllText("blacklist.json");
            JObject json = JObject.Parse(blacklistContent);

            bool isBlacklisted = ((JArray)json["IDs"]!)
                .Select(v => (string?)v)
                .Any(blacklistId => blacklistId == id);

            return isBlacklisted;
        }

        [GeneratedRegex(@"Key=""(.*?)""")]
        private static partial Regex MyRegex();
    }
}
