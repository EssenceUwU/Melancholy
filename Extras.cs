using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Melancholy
{
    public static class Extras
    {
        public static Classes.Settings Settings = new();
        public static bool AddNonInventoryItems { get; set; } = false;
        public static bool AddRetiredOfferings { get; set; } = false;
        public static bool AddEventItems { get; set; } = true;
        public static bool AddBannersBadges { get; set; } = true;
        public static bool AddScaryItems { get; set; } = false;
        public static bool AddLegacy { get; set; } = true;

        private static int Padding(string output) => (Console.WindowWidth / 2) - (output.Length / 2);
        private static void CenterText(string text) => Console.WriteLine("{0}{1}", new string(' ', Padding(text)), text);

        private static List<string> ScaryItems = new List<string>() 
        {
            "TN_Head01_Smile",
            "DF_Torso04_AdmBahroo",
            "DF_Torso04_PandaTVChampKraken",
            "D_Torso02_CartoonZ",
            "DF_Torso04_BloodLetting",
            "DF_Torso04_TwitchKraken",
            "DF_Torso04_KingKongKraken",
            "M_Torso02_Bryce",
            "M_Torso03_AngryPug",
            "MT_Torso04_TwitchKraken",
            "MT_Torso04_KingKongKraken",
            "CM_Torso02_Sxyhxy",
            "CM_Torso03_72hrs",
            "NK_Torso01_Crew01Kraken"
        };

        private static List<string> LegacyCosmetics = new List<string>()
        {
            "CM_Head01_LP01",
            "CM_Legs01_LP01",
            "CM_Torso01_LP01",
            "DF_Head01_LP01",
            "DF_Legs01_LP01",
            "DF_Torso01_LP01",
            "HB_Hammer01_LP01",
            "HB_Legs01_LP01",
            "HB_Torso01_LP01",
            "JP_Head01_LP01",
            "JP_Legs01_LP01",
            "JP_Torso01_LP01",
            "MT_Head01_LP01",
            "MT_Legs01_LP01",
            "MT_Torso01_LP01",
            "NK_Head01_LP01",
            "NK_Legs01_LP01",
            "NK_Torso01_LP01",
            "NR_Body01_LP01",
            "NR_BoneSaw01_LP01",
            "NR_Head01_LP01",
            "TR_Body01_LP01",
            "TR_Head01_LP01",
            "TR_Machete01_LP01",
            "WR_Body01_LP01",
            "WR_Head01_LP01",
            "WR_Machete02_LP01",
        };

        public static bool IsScaryItem(string item) => ScaryItems.Contains(item);
        public static bool IsLegacyCosmetic(string cosmetic) => LegacyCosmetics.Contains(cosmetic);


        public static void Header()
        {
            CenterText("\"Two facts: boys make better girls and furries are superior to all\" - Essence");
            CenterText("MarketUpdater developed by Essence_BHVR (discord: bhvr)");
            Console.WriteLine(Environment.NewLine);
        }

        public static async Task MakeFile<T>(List<T> list, string fileName)
        {
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            await using (var writer = new StreamWriter($"IDs/{fileName}"))
            {
                await writer.WriteAsync(json);
            }

            Console.WriteLine($"{fileName} generated");
        }

        /*public static string NormalizeInput(string input)
        {
            return input.Normalize(NormalizationForm.FormC);
        }

        public static bool ValidateVersionFormat(string version)
        {
            string pattern = @"^\d+\.\d+\.\d+$";
            return Regex.IsMatch(version, pattern);
        }
        
        public static string ConvertToAscii(string input)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding utf8 = Encoding.UTF8;
            byte[] utf8Bytes = utf8.GetBytes(input);
            byte[] asciiBytes = Encoding.Convert(utf8, ascii, utf8Bytes);
            return ascii.GetString(asciiBytes);
        }*/

        public static string PromptInput(string message)
        {
            string input;

            do
            {
                Console.Write(message);
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));

            return input.Replace("\"", "");
        }

        public static string PromptOptionInput(string message)
        {
            Console.Write(message);
            string input = Console.ReadLine().ToLower();

            return input;
        }

        public static int PromptIntInput(string message)
        {
            int parsedInt = 0;
            string input;

            do
            {
                input = PromptInput(message);
                if (!int.TryParse(input, out parsedInt))
                {
                    Console.WriteLine("Invalid input. Please enter a valid number.");
                }
            } while (parsedInt == 0 & !int.TryParse(input, out parsedInt));

            return parsedInt;
        }
    }
}
