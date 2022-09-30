using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using Fungus;
using GUIPackage;
using UnityEngine.Events;
using YSGame.TuJian;
using System.Globalization;
using BepInEx;
using BepInEx.Logging;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using LinesPuller;
using KBEngine;
using AssetsTools.NET.Extra;
using AssetsTools.NET;
using UltimateSurvival.Debugging;
using UltimateSurvival;
using BehaviorDesigner.Runtime.Tasks;
using System.Threading.Tasks;
using System.Media;

namespace LinesPuller
{
    public class StripedWhiteSpaceCompare : IEqualityComparer<string>
    {
        public static string RegexPurify(string s1)
        {
            //return Regex.Replace(Regex.Unescape(s1), @"\s*(\n)", string.Empty);
            return Regex.Replace(s1, @"\s*(\n)", string.Empty);
        }
        public bool CompareString(string s1, string s2)
        {
            //Debug.Log("1");
            return RegexPurify(s1) == RegexPurify(s2);
        }
        bool IEqualityComparer<string>.Equals(string x, string y)
        {
            //Debug.Log("2");
            return CompareString(x, y);
        }

        int IEqualityComparer<string>.GetHashCode(string obj)
        {
            return RegexPurify(obj).GetHashCode();
        }
    }

    public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }
    }








    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {


        public const string pluginGuid = "Cadenza.IWOL.EnMod";
        public const string pluginName = "LinesPuller";
        public const string pluginVersion = "0.5";
        public static bool enabled;
        public static bool enabledDebugLogging = false;
        public static StripedWhiteSpaceCompare comparer = new StripedWhiteSpaceCompare();
        public static Dictionary<string, string> translationDict;
        public static string sourceDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToString();
        public static string parentDir = Directory.GetParent(Main.sourceDir).ToString();
        public static string configDir = Path.Combine(parentDir, "config");
        public static Dictionary<string, string> UILabelsDict;
        public static Dictionary<string, string> TextAssetDict;
        public static Dictionary<string, string> TextAssetDict1;
        public static Dictionary<string, string> TextAssetDict2;
        public static Dictionary<string, string> UITextDict;
        public static Dictionary<string, string> FungusSayDict;
        public static Dictionary<string, string> FungusMenuDict;
        public static Dictionary<string, string> etcDict;
        public static List<string> TAnew = new List<string>();
        public static List<string> newTAKV = new List<string>();
        public static int flag = 0;



        public static Dictionary<string, string> FailedStringsDict = new Dictionary<string, string>(); //String Name, Location; no comparer passed to avoid fuzzy matching invalid strings

        public static void TranslateDictionary<T1>(Dictionary<T1, JSONObject> dict, List<string> fields)
        {
            foreach (var kvp in dict)
            {
                JSONObject jsonObject = kvp.Value;
                foreach (var field in fields)
                {
                    if (jsonObject.HasField(field))
                    {
                        Debug.Log($"Trying to translate: {jsonObject["field"].Str}");
                    }
                }


            }
        }

        public static void AddFailedStringToDict(string s, string location)
        {


            if (FailedStringsDict.ContainsKey(s))
            {

                return;
            }
            FailedStringsDict.Add(s, location);

        }

        public static Dictionary<string, string> FileToDictionary(string dir)
        {
            string ExcludePattern1 = "^神秘铁剑[^¤].*$";
            string ExcludePattern2 = "^昔日身份[^¤].*$";
            string ExcludePattern3 = "^魔道踪影[^¤].*$";
            string ExcludePattern4 = "^御剑门之谜[^¤].*$";
            string ExcludePattern5 = "^往昔追忆开局¤.*$";
            string ExcludePattern6 = "^为神秘铁剑¤.*$";
            string ExcludePattern7 = "^神秘铁剑¤.*$";
            string ExcludePattern8 = "^御剑门传闻.*$";
            string ExcludePattern9 = "^御剑门倪家传闻¤.*$";
            string ExcludePattern10 = "^剑门传闻¤.*$";



            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(sourceDir, "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);
                    if (!Regex.IsMatch(line, ExcludePattern1) && !Regex.IsMatch(line, ExcludePattern2) && !Regex.IsMatch(line, ExcludePattern3) && !Regex.IsMatch(line, ExcludePattern4) && !Regex.IsMatch(line, ExcludePattern5) && !Regex.IsMatch(line, ExcludePattern6) && !Regex.IsMatch(line, ExcludePattern7) && !Regex.IsMatch(line, ExcludePattern8) && !Regex.IsMatch(line, ExcludePattern9) && !Regex.IsMatch(line, ExcludePattern10))
                    {
                        if (!dict.ContainsKey(pair.Key))
                            dict.Add(pair.Key, pair.Value);

                    }
                    else
                    {
                        //Debug.Log("Not touching this with a 10ft pole : " + arr[0]);
                    }
                }
            }


            return dict;

            //return File.ReadLines(Path.Combine(BepInEx.Paths.PluginPath, "Translations", dir))
            //    .Select(line =>
            //    {
            //        var arr = line.Split('¤');
            //        return new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);
            //    })
            //    .GroupBy(kvp => kvp.Key)
            //    .Select(x => x.First())
            //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, comparer);
        }

        public static Dictionary<string, object> FileToExcel(string file)
        {


            Dictionary<string, object> dict = new Dictionary<string, object>();

            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);
                  
                        if (!dict.ContainsKey(pair.Key))
                            dict.Add(pair.Key, pair.Value);

                }
            }


            return dict;

            
        }

        //Average time : 15-17 min
        public void Start()
        {

            Debug.Log("LinesPuller is active !");

            UITextDict = FileToDictionary("UIText.txt");

            TextAssetDict = FileToDictionary("TextAsset.txt");
            etcDict = FileToDictionary("etc.txt");
            TextAssetDict = etcDict.MergeLeft(TextAssetDict);
            TextAssetDict = new Dictionary<string, string>(TextAssetDict, comparer);

            UILabelsDict = FileToDictionary("UILabel.txt");

            FungusSayDict = FileToDictionary("FungusSay.txt");
            FungusMenuDict = FileToDictionary("FungusMenu.txt");

            translationDict = new Dictionary<string, string>().MergeLeft(TextAssetDict, UILabelsDict);

            translationDict = new Dictionary<string, string>(translationDict, comparer);
            Debug.Log(Application.dataPath);


            FungusDump.CleanFiles();
            System.Media.SystemSounds.Beep.Play();
            System.Threading.Thread.Sleep(1000);
            System.Media.SystemSounds.Asterisk.Play();
            System.Threading.Thread.Sleep(1000);
            System.Media.SystemSounds.Exclamation.Play();

            FungusDump.ProcessFungusDumps();
            FungusDump.WriteFungusDump();
            FungusDump.GenerateTADumps();
            TranslationHelper.TranslationDeepl();
            System.Media.SystemSounds.Beep.Play();
            System.Threading.Thread.Sleep(1000);
            System.Media.SystemSounds.Asterisk.Play();
            System.Threading.Thread.Sleep(1000);
            System.Media.SystemSounds.Exclamation.Play();

        }




    public static void PrintDict(Dictionary<string, string> dictionary)
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, string> kvp in dictionary)
        {
            sb.AppendLine($"Key = {kvp.Key}, Value = {kvp.Value}");
        }
        Debug.Log(sb.ToString());
    }





    private void Update()
    {
            if (Input.GetKeyUp(KeyCode.F1) == true)
            {
                ExcelHelper.ConvertToExcel();
            }

        }
}

public static class DictionaryExtensions
{
    // Works in C#3/VS2008:
    // Returns a new dictionary of this ... others merged leftward.
    // Keeps the type of 'this', which must be default-instantiable.
    // Example: 
    //   result = map.MergeLeft(other1, other2, ...)
    public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
        where T : IDictionary<K, V>, new()
    {
        T newMap = new T();
        foreach (IDictionary<K, V> src in
            (new List<IDictionary<K, V>> { me }).Concat(others))
        {
            // ^-- echk. Not quite there type-system.
            foreach (KeyValuePair<K, V> p in src)
            {
                newMap[p.Key] = p.Value;
            }
        }
        return newMap;
    }

    public static Dictionary<TKey, TValue>
    Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
    {
        var result = new Dictionary<TKey, TValue>(dictionaries.First().Comparer);
        foreach (var dict in dictionaries)
            foreach (var x in dict)
                result[x.Key] = x.Value;
        return result;
    }

}

}

