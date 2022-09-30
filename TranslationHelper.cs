using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepL;

namespace LinesPuller
{

    class TranslationHelper
    {

        public static string TransfileSayUN = Path.Combine(BepInEx.Paths.PluginPath, "TransSayUN.txt");
        public static string TransfileMenuUN = Path.Combine(BepInEx.Paths.PluginPath, "TransMenuUN.txt");
        public static string TransfileTAUN = Path.Combine(BepInEx.Paths.PluginPath, "TransTAUN.txt");
        public static string TransfileUITextUN = Path.Combine(BepInEx.Paths.PluginPath, "TransUITextUN.txt");
        public static string TransfileUILabelUN = Path.Combine(BepInEx.Paths.PluginPath, "TransUILabelUN.txt");
        public static async void TranslationDeepl()
        {
            var authKey = "490d69bc-717f-bf91-9f43-08201c927564:fx"; // Replace with your key
            var translator = new Translator(authKey);
            //Menu
            /*IEnumerable<string> MenuUNlines = File.ReadLines(FungusDump.fileMenuUN);
            foreach (var line in MenuUNlines)
            {
                var translatedText = await translator.TranslateTextAsync(
                    line,
                LanguageCode.Chinese,
                LanguageCode.EnglishBritish);
                using (StreamWriter tw = new StreamWriter(TransfileMenuUN, append: true))
                {
                    tw.Write(line + "¤" + translatedText + Environment.NewLine);
                }
            }*/
            //Say
            IEnumerable<string> SayUNLines = File.ReadLines(FungusDump.fileSayUN);
            foreach (var line in SayUNLines)
            {
                var translatedText = await translator.TranslateTextAsync(
                    line,
                LanguageCode.Chinese,
                LanguageCode.EnglishBritish);
                using (StreamWriter tw = new StreamWriter(TransfileSayUN, append: true))
                {
                    tw.Write(line + "¤" + translatedText + Environment.NewLine);
                }
            }
            //UIText
            /*IEnumerable<string> UITextUNLines = File.ReadLines(FungusDump.fileUITextUN);
            foreach (var line in UITextUNLines)
            {
                var translatedText = await translator.TranslateTextAsync(
                    line,
                LanguageCode.Chinese,
                LanguageCode.EnglishBritish);
                using (StreamWriter tw = new StreamWriter(TransfileUITextUN, append: true))
                {
                    tw.Write(line + "¤" + translatedText + Environment.NewLine);
                }
            }*/
            //UILabel
            /*IEnumerable<string> UILabelUNLines = File.ReadLines(FungusDump.fileUILabelUN);
            foreach (var line in UILabelUNLines)
            {
                var translatedText = await translator.TranslateTextAsync(
                    line,
                LanguageCode.Chinese,
                LanguageCode.EnglishBritish);
                using (StreamWriter tw = new StreamWriter(TransfileUILabelUN, append: true))
                {
                    tw.Write(line + "¤" + translatedText + Environment.NewLine);
                }
            }*/
            //TA
            /*
            IEnumerable<string> TAUNLines = File.ReadLines(FungusDump.fileTAUN);
            foreach (var line in TAUNLines)
            {
                var translatedText = await translator.TranslateTextAsync(
                    line,
                LanguageCode.Chinese,
                LanguageCode.EnglishBritish);
                using (StreamWriter tw = new StreamWriter(TransfileTAUN, append: true))
                {
                    tw.Write(line + "¤" + translatedText + Environment.NewLine);
                }
            }*/


        }


    }
}
