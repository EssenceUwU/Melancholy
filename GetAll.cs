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
                PrestigeLevel = Player.PlayerLevel
            }));

            var json = JsonConvert.SerializeObject(getAllData, Formatting.Indented);
            await File.WriteAllTextAsync("Files/GetAll.json", json);
        }

        private static List<Classes.ItemBloodweb> GenerateItemData(Classes.Character character)
        {
            List<Classes.ItemBloodweb> characterItems = [];
            
            characterItems.AddRange(
                Classes.Ids.ItemIds
                    .Where(item => character.CharacterType == item.CharacterType)
                    .Select(item => new Classes.ItemBloodweb
                    {
                        ItemId = item.ItemId,
                        Quantity = (Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount)
                    }));
            
            characterItems.AddRange(
                Classes.Ids.AddonIds
                    .Where(addon =>
                        (character.CharacterType == addon.CharacterType || addon.CharacterType == "EPlayerRole::VE_None") &&
                        !(character.CharacterType == "EPlayerRole::VE_Slasher" && character.CharacterDefaultItem != addon.CharacterDefaultItem))
                    .Select(addon => new Classes.ItemBloodweb
                    {
                        ItemId = addon.ItemId,
                        Quantity = Market.ItemAmount == 0 ? new Random().Next(8, 88) : Market.ItemAmount
                    }));
            
            characterItems.AddRange(
                Classes.Ids.OfferingIds
                    .Where(offering =>
                        character.CharacterType == offering.CharacterType ||
                        offering.CharacterType == "EPlayerRole::VE_None")
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
