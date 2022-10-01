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
using System.Net;
using Ideafixxxer.CsvParser;

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
           
            var MenuUNlines = ToArrayPlus(File.ReadAllLines(FungusDump.fileMenuUN).ToList());
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

        public static class DownloadSheet
        {
            public class WebClientEx : WebClient
            {
                public WebClientEx(CookieContainer container)
                {
                    this.container = container;
                }

                private readonly CookieContainer container = new CookieContainer();

                protected override WebRequest GetWebRequest(Uri address)
                {
                    WebRequest r = base.GetWebRequest(address);
                    var request = r as HttpWebRequest;
                    if (request != null)
                    {
                        request.CookieContainer = container;
                    }
                    return r;
                }

                protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
                {
                    WebResponse response = base.GetWebResponse(request, result);
                    ReadCookies(response);
                    return response;
                }

                protected override WebResponse GetWebResponse(WebRequest request)
                {
                    WebResponse response = base.GetWebResponse(request);
                    ReadCookies(response);
                    return response;
                }

                private void ReadCookies(WebResponse r)
                {
                    var response = r as HttpWebResponse;
                    if (response != null)
                    {
                        CookieCollection cookies = response.Cookies;
                        container.Add(cookies);
                    }
                }
            }

            public static void Main(string url, string path)
            {


            string fullpath = Path.Combine(BepInEx.Paths.PluginPath, "DownloadedFromSpreadSheet", path);

            WebClientEx wc = new WebClientEx(new CookieContainer());
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0");
                wc.Headers.Add("DNT", "1");
                wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                wc.Headers.Add("Accept-Encoding", "deflate");
                wc.Headers.Add("Accept-Language", "en-US,en;q=0.5");

                var outputCSVdata = wc.DownloadString(url);
               
            CsvParser csvparser = new CsvParser();
            var csvarray = csvparser.Parse(outputCSVdata);
            foreach(string[] str in csvarray.Skip(1))
            {
                if (str[0] != null && str[1] != null && str[1] != "")
                {
                    using (StreamWriter tw = new StreamWriter(fullpath, append: true))
                    {
                        tw.Write(str[0] + "¤" + str[1] + Environment.NewLine);
                    }
                }
            }
            }
        }
    
}
