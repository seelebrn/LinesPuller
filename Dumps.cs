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
using Fungus;
using GUIPackage;
using UnityEngine.Events;
using YSGame.TuJian;
using System.Globalization;
using EngTranslatorMod;
using System.Linq.Expressions;
using AssetsTools.NET.Extra;

namespace LinesPuller
{
    static class FungusDump
    {

        public static List<string> SayUN = new List<string>();
        public static List<string> newSayKV = new List<string>();
        public static List<string> MenuUN = new List<string>();
        public static List<string> newMenuKV = new List<string>();
        public static List<string> TAUN = new List<string>();
        public static List<string> newTAKV = new List<string>();
        public static List<string> UITextUN = new List<string>();
        public static List<string> newUITextKV = new List<string>();
        public static List<string> UILabelUN = new List<string>();
        public static List<string> newUILabelKV = new List<string>();
        public static string fileSayUN = Path.Combine(BepInEx.Paths.PluginPath, "SayUN.txt");
        public static string filenewSayKV = Path.Combine(BepInEx.Paths.PluginPath, "newSayKV.txt");
        public static string fileMenuUN = Path.Combine(BepInEx.Paths.PluginPath, "MenuUN.txt");
        public static string filenewMenuKV = Path.Combine(BepInEx.Paths.PluginPath, "newMenuKV.txt");
        public static string fileTAUN = Path.Combine(BepInEx.Paths.PluginPath, "TAUN.txt");
        public static string filenewTAKV = Path.Combine(BepInEx.Paths.PluginPath, "newTAKV.txt");
        public static string fileUITextUN = Path.Combine(BepInEx.Paths.PluginPath, "UITextUN.txt");
        public static string filenewUITextKV = Path.Combine(BepInEx.Paths.PluginPath, "newUITextKV.txt");
        public static string fileUILabelUN = Path.Combine(BepInEx.Paths.PluginPath, "UILabelUN.txt");
        public static string filenewUILabelKV = Path.Combine(BepInEx.Paths.PluginPath, "newUILabelKV.txt");
        public static void CleanFiles()
        {
            if (File.Exists(fileSayUN))
            {
                File.Delete(fileSayUN);
            }
            if (File.Exists(filenewSayKV))
            {
                File.Delete(filenewSayKV);
            }
            if (File.Exists(fileMenuUN))
            {
                File.Delete(fileMenuUN);
            }
            if (File.Exists(filenewMenuKV))
            {
                File.Delete(filenewMenuKV);
            }
            if (File.Exists(fileTAUN))
            {
                File.Delete(fileTAUN);
            }
            if (File.Exists(filenewTAKV))
            {
                File.Delete(filenewTAKV);
            }
            if (File.Exists(fileUITextUN))
            {
                File.Delete(fileUITextUN);
            }
            if (File.Exists(filenewUITextKV))
            {
                File.Delete(filenewUITextKV);
            }
            if (File.Exists(fileUILabelUN))
            {
                File.Delete(filenewUILabelKV);
            }
            if (File.Exists(TranslationHelper.TransfileMenuUN))
            {
                File.Delete(TranslationHelper.TransfileMenuUN);
            }
            if (File.Exists(TranslationHelper.TransfileSayUN))
            {
                File.Delete(TranslationHelper.TransfileSayUN);
            }
            if (File.Exists(TranslationHelper.TransfileUITextUN))
            {
                File.Delete(TranslationHelper.TransfileUITextUN);
            }
            if (File.Exists(TranslationHelper.TransfileUILabelUN))
            {
                File.Delete(TranslationHelper.TransfileUILabelUN);
            }
            if(File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "KVs.xlsx")))
            {
                File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "KVs.xlsx"));
            }


        }
            public static void ProcessFungusDumps()
        {
            var am = new AssetsManager();
            string[] files = Directory.GetFiles(Main.parentDir);
            foreach (var file in files)
            {
                if (!file.Contains(".resS"))
                {
                    if (file.Contains(".assets") || file.Contains("level"))
                    //if(file.Contains("resources.assets"))
                    {

                        var inst = am.LoadAssetsFile(file, true);

                        am.LoadClassPackage(Path.Combine(BepInEx.Paths.PluginPath, "classdata.tpk"));
                        am.LoadClassDatabaseFromPackage(inst.file.typeTree.unityVersion);

                        foreach (var inf in inst.table.GetAssetsOfType((int)AssetClassID.GameObject))
                        {
                            var playerBf = am.GetTypeInstance(inst, inf).GetBaseField();
                            var name = playerBf.Get("m_Name").GetValue().AsString();
                            //Debug.Log("name = " + name);
                            var playerComponentArr = playerBf.Get("m_Component").Get("Array");

                            //first let's search for the MonoBehaviour we want in a GameObject
                            for (var i = 0; i < playerComponentArr.GetChildrenCount(); i++)
                            {
                                try
                                {
                                    //get component info (but don't deserialize yet, loading assets we don't need is wasteful)
                                    var childPtr = playerComponentArr[i].Get("component");
                                    var childExt = am.GetExtAsset(inst, childPtr, true);
                                    var childInf = childExt.info;

                                    //skip if not MonoBehaviour
                                    if (childInf.curFileType != (uint)AssetClassID.MonoBehaviour)
                                        continue;

                                    var componentType = (AssetClassID)childExt.info.curFileType;
                                    childExt = am.GetExtAsset(inst, childPtr, false);

                                    //Deserialize
                                    //Debug.Log("Non Null");
                                    var childBf = childExt.instance.GetBaseField();
                                    //Debug.Log(childBf);
                                    var monoScriptPtr = childBf.Get("m_Script");


                                    //get MonoScript from MonoBehaviour
                                    var monoScriptExt = am.GetExtAsset(childExt.file, monoScriptPtr);
                                    var monoScriptBf = monoScriptExt.instance.GetBaseField();

                                    var className = monoScriptBf.Get("m_ClassName").GetValue().AsString();
                                    //Debug.Log("Class Name = " + className);
                                    var monoBehaviourInf = childInf;
                                    var monoBehaviourBf = childBf;
                                    if (className == "Menu")
                                    {
                                        var managedFolder = Path.Combine(Path.GetDirectoryName(inst.path), "Managed");
                                        monoBehaviourBf = MonoDeserializer.GetMonoBaseField(am, inst, monoBehaviourInf, managedFolder);

                                        var text = monoBehaviourBf.Get("text").GetValue().AsString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
                                        if (text != null && Helpers.IsChinese(text) && text != "")
                                        {
                                            if(!Main.FungusMenuDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                MenuUN.Add(text);
                                            }
                                            if (Main.FungusMenuDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                newMenuKV.Add(text);
                                            }
                                        }
                                    }
                                    if (className == "Say")
                                    {
                                        var managedFolder = Path.Combine(Path.GetDirectoryName(inst.path), "Managed");
                                        monoBehaviourBf = MonoDeserializer.GetMonoBaseField(am, inst, monoBehaviourInf, managedFolder);

                                        var text = monoBehaviourBf.Get("storyText").GetValue().AsString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
                                        if (text != null && Helpers.IsChinese(text) && text != "")
                                        {
                                            if (!Main.FungusSayDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                SayUN.Add(text);
                                            }
                                            if (Main.FungusSayDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                newSayKV.Add(text);
                                            }
                                        }
                                    }
                                    if (className == "Text")
                                    {
                                        var managedFolder = Path.Combine(Path.GetDirectoryName(inst.path), "Managed");
                                        monoBehaviourBf = MonoDeserializer.GetMonoBaseField(am, inst, monoBehaviourInf, managedFolder);

                                        var text = monoBehaviourBf.Get("m_Text").GetValue().AsString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
                                        if (text != null && Helpers.IsChinese(text) && text != "")
                                        {
                                            if (!Main.UITextDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                UITextUN.Add(text);
                                            }
                                            if (Main.UITextDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                newUITextKV.Add(text);
                                            }
                                        }
                                    }
                                    if (className == "UILabel")
                                    {
                                        var managedFolder = Path.Combine(Path.GetDirectoryName(inst.path), "Managed");
                                        monoBehaviourBf = MonoDeserializer.GetMonoBaseField(am, inst, monoBehaviourInf, managedFolder);

                                        var text = monoBehaviourBf.Get("mText").GetValue().AsString().Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
                                        if (text != null && Helpers.IsChinese(text) && text != "")
                                        {
                                            if (!Main.UILabelsDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                UILabelUN.Add(text);
                                            }
                                            if (Main.UILabelsDict.ContainsKey(text) && text != null && text != "")
                                            {
                                                newUILabelKV.Add(text);
                                            }
                                        }
                                    }



                                }
                                catch
                                {

                                }
                            }

                        }
                    }

                }
            }



        }
        public static void WriteFungusDump()
        {
            //FungusMenu
            foreach(var text in MenuUN.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(fileMenuUN, append: true))
                {
                    tw.Write(text + Environment.NewLine);
                }
            }
            foreach (var text in newMenuKV.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(filenewMenuKV, append: true))
                {
                    tw.Write(text + "¤" + Main.FungusMenuDict[text] + Environment.NewLine);
                }
            }
            //FungusSay
            foreach (var text in SayUN.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(fileSayUN, append: true))
                {
                    tw.Write(text + Environment.NewLine);
                }
            }
            foreach (var text in newSayKV.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(filenewSayKV, append: true))
                {
                    tw.Write(text + "¤" + Main.FungusSayDict[text] + Environment.NewLine);
                }
            }
            //UIText
            foreach (var text in UITextUN.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(fileUITextUN, append: true))
                {
                    tw.Write(text + Environment.NewLine);
                }
            }
            foreach (var text in newUITextKV.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(filenewUITextKV, append: true))
                {
                    tw.Write(text + "¤" + Main.UITextDict[text] + Environment.NewLine);
                }
            }
            //UIText
            foreach (var text in UILabelUN.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(fileUILabelUN, append: true))
                {
                    tw.Write(text + Environment.NewLine);
                }
            }
            foreach (var text in newUILabelKV.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(filenewUILabelKV, append: true))
                {
                    tw.Write(text + "¤" + Main.UILabelsDict[text] + Environment.NewLine);
                }
            }
        }

            public static void GenerateTADumps()
        {
            string ExcludePattern1 = "^神秘铁剑[^¤].*$";
            string ExcludePattern2 = "^昔日身份[^¤].*$";
            string ExcludePattern3 = "^魔道踪影[^¤].*$";
            string ExcludePattern4 = "^御剑门之谜[^¤].*$";
            string ExcludePattern5 = "^往昔追忆开局$";
            string ExcludePattern6 = "^为神秘铁剑$";
            string ExcludePattern7 = "^神秘铁剑$";
            string ExcludePattern8 = "^御剑门传闻.*$";
            string ExcludePattern9 = "^御剑门倪家传闻$";
            string ExcludePattern10 = "^剑门传闻$";
            System.IO.DirectoryInfo jsondir = new DirectoryInfo(Path.Combine(Application.dataPath, "Res", "Effect", "json"));
            foreach (FileInfo file in jsondir.GetFiles())
            {
                if (!file.Name.Contains("BadWord"))
                {
                    var read = File.ReadAllText(file.FullName);
                    {
                        var pattern = "\"([^\"]*)\"";
                        MatchCollection matchCollection = Regex.Matches(read, pattern);
                        foreach(var match in matchCollection)
                        {
                            var str = Regex.Unescape(match.ToString()).Replace("\"", "").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\\获", "\\\\u83b7");
                            if (Regex.Unescape(match.ToString().Replace("\"", "")).StartsWith("战斗开始时，若神识高于对手"))
                            {
                                Debug.Log("WARNING : " + str);
                                Debug.Log("WARNING : " + match.ToString());

                            }
                            if (Helpers.IsChinese(str))
                            {
                                if (!Main.TextAssetDict.ContainsKey(str) && !str.StartsWith("local") && !str.StartsWith("return") && !Regex.IsMatch(str, ExcludePattern1) && !Regex.IsMatch(str, ExcludePattern2) && !Regex.IsMatch(str, ExcludePattern3) && !Regex.IsMatch(str, ExcludePattern4) && !Regex.IsMatch(str, ExcludePattern5) && !Regex.IsMatch(str, ExcludePattern6) && !Regex.IsMatch(str, ExcludePattern7) && !Regex.IsMatch(str, ExcludePattern8) && !Regex.IsMatch(str, ExcludePattern9) && !Regex.IsMatch(str, ExcludePattern10)) 
                                {
                                    TAUN.Add(str);
                                }
                                if (Main.TextAssetDict.ContainsKey(str) && !str.StartsWith("local") && !str.StartsWith("return"))
                                {
                                    newTAKV.Add(str);
                                }

                            }
                        }
                    }
                }
            }
            foreach(var item in TAUN.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(fileTAUN, append: true))
                {
                    tw.Write(item + Environment.NewLine);
                }
            }
            foreach (var item in newTAKV.Distinct())
            {
                using (StreamWriter tw = new StreamWriter(filenewTAKV, append: true))
                {
                    tw.Write(item + "¤" + Main.TextAssetDict[item] + Environment.NewLine);
                }
            }

        }


            static Dictionary<string, FieldInfo> GetPropertiesOfType<T1>()
        {
            Dictionary<string, FieldInfo> dict = new Dictionary<string, FieldInfo>();
            FieldInfo[] props = jsonData.instance.GetType().GetFields();

            foreach (FieldInfo prop in props)
            {
                if (prop.FieldType == typeof(T1))
                {
                    dict.Add(prop.Name, prop);
                }
            }

            return dict;
        }
    }
}
