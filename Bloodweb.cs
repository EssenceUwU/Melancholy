﻿using Newtonsoft.Json;

namespace Melancholy
{
    public static class Bloodweb
    {
        public static async Task Generate_Bloodweb()
        {
            Classes.BloodwebData bloodwebObj = new()
            {
                PrestigeLevel = Player.PlayerLevel
            };

            var json = JsonConvert.SerializeObject(bloodwebObj, Formatting.Indented);
            await File.WriteAllTextAsync("Files/Bloodweb.json", json);
        }
    }
}
