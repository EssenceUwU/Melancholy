using Melancholy;
using Newtonsoft.Json;

const string SettingsFile = "settings.json";

try
{
    Console.Title = "DBD OwO";
    Extras.Header();

    if (!Directory.Exists("Files")) Directory.CreateDirectory("Files");
    if (!Directory.Exists("Other")) Directory.CreateDirectory("Other");
    if (!Directory.Exists("IDs")) Directory.CreateDirectory("IDs");

    if (File.Exists(SettingsFile))
    {
        Extras.Settings = JsonConvert.DeserializeObject<Classes.Settings>(File.ReadAllText(SettingsFile));

        Console.WriteLine(
            $"Current settings:\nPak path: {Extras.Settings.PakPath}\nAES key: {Extras.Settings.AesKey}\nMappings path: {Extras.Settings.MappingsPath}\n");
        Console.Write("Would you like to load settings? (Y/n): ");
        switch (Console.ReadLine())
        {
            case "no":
            case "n":
                goto PopulateSettings;
            default:
                goto SkipSettings;
        }
    }

PopulateSettings:
    Extras.Settings.PakPath = Extras.PromptInput("Provide path to your paks folder: ");
    Extras.Settings.AesKey = Extras.PromptInput("Provide AES key: ");
    Extras.Settings.MappingsPath = Extras.PromptInput("Provide path to DBD.usmap file: ");

    File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(Extras.Settings, Formatting.Indented));

SkipSettings:
    Console.Clear();
    Extras.Header();

    Console.WriteLine("Doing CUE4Parse stuffs...");

    Cue4Parse.Initialize();
    Cue4Parse.CdnAccessKey = Cue4Parse.GetAccessKey();
    Cue4Parse.Get_Files();

    Market.ItemAmount = Extras.PromptIntInput("Enter desired item amount (0 for random): ");
    Market.PrestigeLevel = Extras.PromptIntInput("Enter desired prestige for characters: ");
    Player.PlayerLevel = Extras.PromptIntInput("Enter desired player level: ");
    Player.DevotionLevel = Extras.PromptIntInput("Enter desired devotion level: ");

    Console.Clear();
    Extras.Header();

    await Cdn.GetCdnFiles();

    await Extras.MakeFile(Classes.Ids.DlcIds, "Dlc.json");
    await Extras.MakeFile(Classes.Ids.ItemIds, "Items.json");
    await Extras.MakeFile(Classes.Ids.AddonIds, "Addons.json");
    await Extras.MakeFile(Classes.Ids.OfferingIds, "Offerings.json");
    await Extras.MakeFile(Classes.Ids.PerkIds, "Perks.json");

    await GeneratePrestigeList.GenerateList();

    await Market.Generate_Market("all");
    await Market.Generate_Market("dlconly");
    await Market.Generate_Market("notemp");
    await Market.Generate_Market("tempNoCosmetics");
    await Market.Generate_Market("perks");
    Console.WriteLine("Market files generated");

    await GetAll.Generate_GetAll();
    Console.WriteLine("GetAll.json generated");

    await Bloodweb.Generate_Bloodweb();
    Console.WriteLine("Bloodweb.json generated");

    await Player.Generate_PlayerLevel();

    Console.WriteLine("\nPress any key to close...");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Console.ReadLine();
}