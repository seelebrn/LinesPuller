using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace LinesPuller
{
    class ExcelHelper
    {

        public static List<Tuple<string,string>> ToArrayPlus(Dictionary<string,string> dict)
        {
            List<Tuple<string, string>> scores = new List<Tuple<string, string>>();
            foreach (KeyValuePair<string,string> kvp in dict)
            {

                scores.Add(new Tuple<string, string>(kvp.Key, kvp.Value));
            }
            return scores;
        }
        public static List<Tuple<string, string>> ToArrayPlus(List<string> dict)
        {
            List<Tuple<string, string>> scores = new List<Tuple<string, string>>();
            foreach (string str in dict)
            {

                scores.Add(new Tuple<string, string>(str, ""));
            }
            return scores;
        }
        public static void ConvertToExcel()
        {
            
            var path = Path.Combine(BepInEx.Paths.PluginPath, "KVs.xlsx");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
           
            var MenuUNlines = ToArrayPlus(File.ReadAllLines(FungusDump.filenewMenuKV).ToList());
            Debug.Log(MenuUNlines);
            var SayUNLines = ToArrayPlus(Main.FileToDictionary(TranslationHelper.TransfileSayUN));
            var UITextUNLines = ToArrayPlus(File.ReadAllLines(FungusDump.fileUITextUN).ToList());
            var UILabelUNLines = ToArrayPlus(File.ReadAllLines(FungusDump.fileUILabelUN).ToList());
            var TAUNLines = ToArrayPlus(File.ReadAllLines(FungusDump.fileTAUN).ToList());
           
            var MenuKVLines = ToArrayPlus(Main.FileToDictionary(FungusDump.filenewMenuKV));
            var SayKVLines = ToArrayPlus(Main.FileToDictionary(FungusDump.filenewSayKV));
            var UITextKVLines = ToArrayPlus(Main.FileToDictionary(FungusDump.filenewUITextKV));
            var UILabelKVLines = ToArrayPlus(Main.FileToDictionary(FungusDump.filenewUILabelKV));
            var TAKVLines = ToArrayPlus(Main.FileToDictionary(FungusDump.filenewTAKV));








            var sheets = new Dictionary<string, object>
            {
                ["MenuKV"] = MenuKVLines,
                ["MenuUN"] = MenuUNlines,
                ["SayKV"] = SayKVLines,
                ["SayUN"] = SayUNLines,
                ["UITextKV"] = UITextKVLines,
                ["UITextUN"] = UITextUNLines,
                ["UILabelKV"] = UILabelKVLines,
                ["UILabelUN"] = UILabelUNLines,
                ["TAKV"] = TAKVLines,
                ["TAUN"] = TAUNLines,

            };
            MiniExcel.SaveAs(path, sheets);
            
        }
    }
}
