using CUE4Parse.GameTypes.PUBG.Assets.Exports;
using Newtonsoft.Json;

namespace Melancholy
{
    public static class GetAll
    {
        public static async Task Generate_GetAll()
        {
            Classes.GetAllData getAllData = new();

            getAllData.List.AddRange(Classes.Ids.DlcIds.Select(character => new Classes.CharacterItem
            {
                CharacterName = character.CharacterName,
                CharacterItems = GenerateItemData(character),
                BloodWebData = BloodwebGenerator.Make_Bloodweb(character.CharacterType, character.CharacterDefaultItem),
                PrestigeLevel = Market.PrestigeLevel
            }));

            var json = JsonConvert.SerializeObject(getAllData, Formatting.Indented);
            await File.WriteAllTextAsync("Files/GetAll.json", json);
        }

        private static List<Classes.ItemBloodweb> GenerateItemData(Classes.Character character)
        {
            List<Classes.ItemBloodweb> characterItems = [];
            
            characterItems.AddRange(
                Classes.Ids.ItemIds
                    .Where(item =>
                    {
                        bool includeItem = true;

                        if (character.CharacterType == item.CharacterType) includeItem = true;
                        if (!Extras.AddNonInventoryItems && !item.ShouldBeInInventory) includeItem = false;
                        if (!Extras.AddEventItems && item.EventId != "None") includeItem = false;
                        if (!Extras.AddScaryItems && Extras.IsScaryItem(item.ItemId)) includeItem = false;

                        return includeItem;
                    })
                    .Select(item => new Classes.ItemBloodweb
                    {
                        ItemId = item.ItemId,
                        Quantity = (Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount)
                    }));
            
            characterItems.AddRange(
                Classes.Ids.AddonIds
                    .Where(addon =>
                    {
                        bool includeAddon = true;

                        if (character.CharacterType == addon.CharacterType ||
                            addon.CharacterType == "EPlayerRole::VE_None") includeAddon = true;
                        if (character.CharacterType == "EPlayerRole::VE_Slasher" &&
                            character.CharacterDefaultItem != addon.CharacterDefaultItem) includeAddon = false;
                        if (!Extras.AddNonInventoryItems && !addon.ShouldBeInInventory) includeAddon = false;
                        if (!Extras.AddEventItems && addon.EventId != "None") includeAddon = false;
                        if (!Extras.AddScaryItems && Extras.IsScaryItem(addon.ItemId)) includeAddon = false;

                        return includeAddon;
                    })
                    .Select(addon => new Classes.ItemBloodweb
                    {
                        ItemId = addon.ItemId,
                        Quantity = Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount
                    }));
            
            characterItems.AddRange(
                Classes.Ids.OfferingIds
                    .Where(offering =>
                    {
                        bool includeOffering = true;

                        if (character.CharacterType == offering.CharacterType ||
                            offering.CharacterType == "EPlayerRole::VE_None") includeOffering = true;
                        if (!Extras.AddNonInventoryItems && !offering.ShouldBeInInventory) includeOffering = false;
                        if (!Extras.AddEventItems && offering.EventId != "None") includeOffering = false;
                        if (!Extras.AddRetiredOfferings && offering.Availability == "EItemAvailability::Retired")
                            includeOffering = false;
                        if (!Extras.AddScaryItems && Extras.IsScaryItem(offering.ItemId)) includeOffering = false;

                        return includeOffering;
                    })
                    .Select(offering => new Classes.ItemBloodweb
                    {
                        ItemId = offering.ItemId,
                        Quantity = Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount
                    }));

            characterItems.AddRange(
                Classes.Ids.PerkIds
                    .Where(perk => 
                        character.CharacterType == perk.CharacterType ||
                        perk.CharacterType == "EPlayerRole::VE_None")
                    .Select(perk => new Classes.ItemBloodweb
                    {
                        ItemId = perk.ItemId,
                        Quantity = 3
                    }));

            return characterItems;
        }
    }
}
