﻿namespace Melancholy
{
    public static class BloodwebGenerator
    {
        public static List<Classes.ItemBloodweb> TivoTigs = [];
        public static List<Classes.InventoryItemBloodweb> BloodwebList = [];

        public static Classes.Bloodweb Make_Bloodweb(string characterType, string characterPower, bool perkStatus = true)
        {
            TivoTigs = [];
            BloodwebList = [];

            foreach (var item in ConvertToInventoryItemBloodweb(Classes.Ids.ItemIds))
                if (item is Classes.InventoryItemBloodweb inventoryItem)
                {
                    if (!Extras.AddNonInventoryItems && !inventoryItem.ShouldBeInInventory) continue;
                    if (!Extras.AddEventItems && inventoryItem.EventId != "None") continue;

                    BloodwebList.Add(new Classes.InventoryItemBloodweb
                    {
                        ItemId = inventoryItem.ItemId,
                        CharacterType = inventoryItem.CharacterType,
                        CharacterDefaultItem = ""
                    });
                }

            foreach (var item in ConvertToInventoryItemBloodweb(Classes.Ids.AddonIds))
                if (item is Classes.InventoryItemBloodweb addon)
                {
                    if (characterType.Contains("Slasher") && (addon.CharacterDefaultItem != characterPower)) continue;
                    if (!Extras.AddNonInventoryItems && !addon.ShouldBeInInventory) continue;
                    if (!Extras.AddEventItems && addon.EventId != "None") continue;

                    BloodwebList.Add(new Classes.InventoryItemBloodweb
                    {
                        ItemId = addon.ItemId,
                        CharacterType = addon.CharacterType,
                        CharacterDefaultItem = addon.CharacterDefaultItem,
                    });
                }

            foreach (var item in ConvertToInventoryItemBloodweb(Classes.Ids.OfferingIds))
                if (item is Classes.InventoryItemBloodweb offering)
                {
                    if (!Extras.AddNonInventoryItems && !offering.ShouldBeInInventory) continue;
                    if (!Extras.AddEventItems && offering.EventId != "None") continue;
                    if (!Extras.AddRetiredOfferings && offering.Availability == "EItemAvailability::Retired") continue;

                    BloodwebList.Add(new Classes.InventoryItemBloodweb
                    {
                        ItemId = offering.ItemId,
                        CharacterType = offering.CharacterType,
                        CharacterDefaultItem = ""
                    });
                }

            foreach (var item in Classes.Ids.ItemIds)
            {
                if (!Extras.AddNonInventoryItems && !item.ShouldBeInInventory) continue;
                if (!Extras.AddEventItems && item.EventId != "None") continue;

                TivoTigs.Add(new Classes.ItemBloodweb
                {
                    ItemId = item.ItemId,
                    Quantity = (Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount)
                });
            }

            foreach (var item in Classes.Ids.AddonIds)
            {
                if (!Extras.AddNonInventoryItems && !item.ShouldBeInInventory) continue;
                if (!Extras.AddEventItems && item.EventId != "None") continue;

                TivoTigs.Add(new Classes.ItemBloodweb
                {
                    ItemId = item.ItemId,
                    Quantity = (Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount)
                });
            }

            foreach (var item in Classes.Ids.OfferingIds)
            {
                if (!Extras.AddNonInventoryItems && !item.ShouldBeInInventory) continue;
                if (!Extras.AddEventItems && item.EventId != "None") continue;
                if (!Extras.AddRetiredOfferings && item.Availability == "EItemAvailability::Retired") continue;

                TivoTigs.Add(new Classes.ItemBloodweb
                {
                    ItemId = item.ItemId,
                    Quantity = (Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount)
                });
            }

            if (perkStatus)
                foreach (var item in Classes.Ids.PerkIds)
                    TivoTigs.Add(new Classes.ItemBloodweb { ItemId = item.ItemId, Quantity = 3 });

            var pathData = new List<string>
            {
                "0_101",
                "0_102",
                "0_103",
                "0_104",
                "0_105",
                "0_106",
                "101_102",
                "102_103",
                "103_104",
                "104_105",
                "105_106",
                "106_101",
                "101_201",
                "101_202",
                "102_203",
                "102_204",
                "103_205",
                "103_206",
                "104_207",
                "104_208",
                "105_209",
                "105_210",
                "106_211",
                "106_212",
                "201_202",
                "202_203",
                "203_204",
                "204_205",
                "205_206",
                "206_207",
                "207_208",
                "208_209",
                "209_210",
                "210_211",
                "211_212",
                "212_201",
                "301_302",
                "302_303",
                "303_304",
                "304_305",
                "305_306",
                "306_307",
                "307_308",
                "308_309",
                "309_310",
                "310_311",
                "311_312",
                "312_301"
            };

            var ringData = new List<Classes.Ring>
            {
                new Classes.Ring
                {
                    NodeData = [
                        new Classes.Node { NodeId = 0 }
                    ]
                },
                new Classes.Ring
                {
                    NodeData = [
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 101 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 102 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 103 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 104 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 105 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 106 }
                    ]
                },
                new Classes.Ring
                {
                    NodeData = [
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 201 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 202 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 203 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 204 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 205 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 206 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 207 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 208 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 209 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 210 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 211 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 212 }
                    ]
                },
                new Classes.Ring
                {
                    NodeData = [
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 301 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 302 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 303 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 304 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 305 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 306 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 307 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 308 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 309 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 310 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 311 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 312 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 313 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 314 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 315 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 316 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 317 },
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 318 }
                    ]
                },
                new Classes.Ring
                {
                    NodeData = [
                        new Classes.Node { ContentId = GetRandomId(characterType), NodeId = 401 }
                    ]
                }
            };

            return new Classes.Bloodweb
            {
                Paths = pathData,
                RingData = ringData
            };
        }

        private static string GetRandomId(string characterType)
        {
            var filteredList = BloodwebList.Where(item => item.CharacterType == characterType).ToList();

            if (filteredList.Count == 0) return string.Empty;

            Random random = new();
            int randIndex = random.Next(filteredList.Count);

            return filteredList[randIndex].ItemId;
        }

        private static List<Classes.InventoryItemBloodweb> ConvertToInventoryItemBloodweb(IEnumerable<Classes.ItemOfferingPerk> items)
        {
            var bloodwebItem = items.Select(item => new Classes.InventoryItemBloodweb
            {
                ItemId = item.ItemId,
                CharacterType = item.CharacterType,
                CharacterDefaultItem = ""
            }).ToList();

            return bloodwebItem;
        }
    }
}
