using BepInEx;
using Config;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Global;
using GameData.Domains.TaiwuEvent.DisplayEvent;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;


namespace TaiwuETC
{
    [BepInPlugin("TaiwuETC", "Taiwu Mods Community", "0.1.0")]
    public class Main : BaseUnityPlugin
    {
        public static Dictionary<string, string> translationDict;
        public static Dictionary<string, string> NamesDict;
        public static Dictionary<string, string> SurnamesDict;
        public static Dictionary<string, string> LocalMonasticTitlesDict;
        public static Dictionary<string, string> LocalTownNamesDict;
        public static Dictionary<string, string> LocalZangNamesDict;
        public static List<string> MissingNames = new List<string>();
        public static List<string> MissingSurnames = new List<string>();
        public static List<string> MissingMonasticTitlesDict = new List<string>();
        public static List<string> MissingTownNames = new List<string>();
        public static List<string> MissingZangNames = new List<string>();
        public static string translationpath = Path.Combine(BepInEx.Paths.PluginPath, "TaiwuETC", "Translations");

        public static Dictionary<string, string> FileToDictionary(string dir)
        {


            Dictionary<string, string> dict = new Dictionary<string, string>();

            IEnumerable<string> lines = File.ReadLines(Path.Combine(BepInEx.Paths.PluginPath, "TaiwuETC", "Translations", dir));

            foreach (string line in lines)
            {

                var arr = line.Split('¤');
                if (arr[0] != arr[1])
                {
                    var pair = new KeyValuePair<string, string>(Regex.Replace(arr[0], @"\t|\n|\r", ""), arr[1]);

                    if (!dict.ContainsKey(pair.Key))
                        dict.Add(pair.Key, pair.Value);
                    //else
                        //Debug.Log($"Found a duplicated line while parsing {dir}: {pair.Key}");



                }
            }

            return dict;

        }




        public void Awake()
        {

            try
            {
                Debug.Log(Path.Combine(Paths.GameRootPath, "Languages", "en"));
                SurnamesDict = FileToDictionary("Surnames.txt");
                NamesDict = FileToDictionary("Names.txt");
                LocalMonasticTitlesDict = FileToDictionary("LocalMonasticTitles.txt");
                LocalTownNamesDict = FileToDictionary("LocalTownNames.txt");
                LocalZangNamesDict = FileToDictionary("LocalZangNames.txt");
                translationDict = SurnamesDict.MergeLeft(NamesDict);
                translationDict = translationDict.MergeLeft(LocalMonasticTitlesDict);
                translationDict = translationDict.MergeLeft(LocalTownNamesDict);
                translationDict = translationDict.MergeLeft(LocalZangNamesDict);
                Debug.Log("Successfully Generated Dict");
            }

            catch (Exception e)
            {
                Debug.Log("Error in generating dicts");
                Debug.LogException(e);
            }
            Debug.Log("!!!!! Taiwu ETC loaded");

            Harmony harmony = new Harmony("TaiwuETC.Taiwu Mods Community.0.1.0");
            harmony.PatchAll();
        }

        private void Update()

        {
            if (Input.GetKeyUp(KeyCode.F11))
            {
                File.WriteAllText(Path.Combine(Main.translationpath, "MissingNames.txt"), String.Empty);
                File.WriteAllText(Path.Combine(Main.translationpath, "MissingSurnames.txt"), String.Empty);
                File.WriteAllText(Path.Combine(Main.translationpath, "MissingMonasticTitles.txt"), String.Empty);
                File.WriteAllText(Path.Combine(Main.translationpath, "MissingTownNames.txt"), String.Empty);
                File.WriteAllText(Path.Combine(Main.translationpath, "MissingZangNames.txt"), String.Empty);
                foreach (string s in Main.MissingNames.Distinct())
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine(Main.translationpath, "MissingNames.txt")))
                        if (Helpers.IsChinese(s))
                        {
                            sw.Write(s);
                        }
                }
                foreach (string s in Main.MissingSurnames.Distinct())
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine(Main.translationpath, "MissingSurnames.txt")))
                    {
                        if (Helpers.IsChinese(s))
                        {
                            sw.Write(s);
                        }
                    }
                }
                foreach (string s in Main.MissingMonasticTitlesDict.Distinct())
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine(Main.translationpath, "MissingMonasticTitles.txt")))
                    {
                        if (Helpers.IsChinese(s))
                        {
                            sw.Write(s);
                        }
                    }
                }
                foreach (string s in Main.MissingTownNames.Distinct())
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine(Main.translationpath, "MissingTownNames.txt")))
                    {
                        if (Helpers.IsChinese(s))
                        {
                            sw.Write(s);
                        }
                    }
                }
                foreach (string s in Main.MissingZangNames.Distinct())
                {
                    using (StreamWriter sw = File.AppendText(Path.Combine(Main.translationpath, "MissingZangNames.txt")))
                    {
                        if (Helpers.IsChinese(s))
                        {
                            sw.Write(s);
                        }
                    }
                }

            }

            if (Input.GetKeyUp(KeyCode.F10))
            {
                File.WriteAllText("EventOptionTips_CN.txt", String.Empty);

                string[] hello = new string[] { };
                string optionAvailableLanguageFileName = "EventOptionTips_" + LocalStringManager.CurLanguageKey;
                string filePath = Path.Combine("RemakeResources/Data/Language_EventOptionTips", optionAvailableLanguageFileName);
                ResLoader.Load<TextAsset>(filePath, delegate (TextAsset asset)
                {
                    hello = asset.text.Split(new char[]
                    {
                '\n'
                    });
                }, null);


                using (StreamWriter sw = File.AppendText(Path.Combine(BepInEx.Paths.GameRootPath, "EventOptionTips_CN.txt")))
                {
                    for (int i = 0; i < hello.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(hello[i]))
                        {
                            sw.Write(hello[i] + "\n");
                        }
                    }

                    sw.Close();
                }

            }
        }

    }

    /*
    [HarmonyPatch(typeof(MonasticTitleItem), "MonasticTitleItem")]
    static class Patch01
    {
        static void Postfix(MonasticTitleItem __instance)
        {
            Debug.Log(__instance.Name);
        }
    }*/
    /*
    [HarmonyPatch(typeof(LocalMonasticTitles), "Init")]

    static class LocalMonasticTitles_Patch
    {

        static void Postfix(LocalMonasticTitles __instance)
        {
            foreach (MonasticTitleItem x in __instance.MonasticTitles)
            {
                try
                {
                    if (x.Name != null)
                    {

                        if (x.Name != "" && Main.translationDict.ContainsKey(x.Name))
                        {
                            // Debug.Log("Found one ! : " + x.Name);
                            FieldInfo fi = x.GetType().GetField("Name");
                            fi.SetValue(x, Main.translationDict[x.Name]);
                            // Debug.Log("Modified Name = " + x.Name);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }
    }
    */
    [HarmonyPatch(typeof(LocalTownNames), "Init")]

    static class LocalTownNames_Patch
    {

        static void Postfix(LocalTownNames __instance)
        {
            foreach (TownNameItem x in __instance.TownNameCore)
            {
                try
                {
                    if (x.Name != null)
                    {

                        if (x.Name != "" && Main.translationDict.ContainsKey(x.Name))
                        {
                            //Debug.Log("Found one Surname ! : " + x.Name);
                            FieldInfo fi = x.GetType().GetField("Name");
                            fi.SetValue(x, Main.translationDict[x.Name]);
                            //Debug.Log("Modified Name = " + x.Name);
                        }
                        else
                        {
                            if (x.Name != null && x.Name!= "")
                            {
                                Main.MissingTownNames.Add(x.Name);
                            }
                        }

                    }
                }
                catch (Exception e)
                {

                }
            }
        }
    }
    [HarmonyPatch(typeof(LocalNames), "Init")]

    static class LocalNames_Patch
    {

        static void Postfix(LocalNames __instance)
        {
            foreach (HanNameItem x in __instance.AllNamesCore)
            {
                try
                { 
                if(x.MiddleChar != null)
                    {
                        if(Main.translationDict.ContainsKey(x.MiddleChar))
                        {
                            x.GetType().GetField("MiddleChar").SetValue(x, Main.translationDict[x.MiddleChar]);
                        }
                        else
                        {
                            Main.MissingNames.Add(x.MiddleChar);
                        }
                    }
                for (int i = 0; i < x.ApartMan.Count(); i++)
                {
                    if (x.ApartMan[i] != null)
                    {
                        var y = x.ApartMan[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("ApartMan");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                    //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                for (int i = 0; i < x.ApartNeutral.Count(); i++)
                {
                    if (x.ApartNeutral[i] != null)
                    {
                        var y = x.ApartNeutral[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("ApartNeutral");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                    //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                for (int i = 0; i < x.ApartWoman.Count(); i++)
                {
                    if (x.ApartWoman[i] != null)
                    {
                        var y = x.ApartWoman[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("ApartWoman");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                    //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                for (int i = 0; i < x.SerialMan.Count(); i++)
                {
                    if (x.SerialMan[i] != null)
                    {
                        var y = x.SerialMan[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("SerialMan");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                    //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                for (int i = 0; i < x.SerialNeutral.Count(); i++)
                {
                    if (x.SerialNeutral[i] != null)
                    {
                        var y = x.SerialNeutral[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("SerialNeutral");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                   //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                for (int i = 0; i < x.SerialWoman.Count(); i++)
                {
                    if (x.SerialWoman[i] != null)
                    {
                        var y = x.SerialWoman[i];
                        if (y != "" && Main.translationDict.ContainsKey(y))
                        {

                            FieldInfo fi = x.GetType().GetField("SerialWoman");
                            string[] value = (string[])fi.GetValue(x);
                            for (int j = 0; j < value.Count(); j++)
                            {
                                if (Main.translationDict.ContainsKey(value[j]))
                                {
                                    //Debug.Log("Found one Name ! : " + value[j]);
                                    value[j] = Main.translationDict[value[j]];
                                    //Debug.Log("Modified Name = " + value[j]);
                                }
                            }
                        }
                        else
                        {


                            if (y != null && y != "")
                            {
                                Main.MissingNames.Add(y);
                            }

                        }
                    }
                }

                }
                catch
                { }
            }
        }
    }

    [HarmonyPatch(typeof(LocalSurnames), "Init")]

    static class LocalSurNames_Patch
    {

        static void Postfix(LocalSurnames __instance)
        {
            foreach (SurnameItem x in __instance.SurnameCore)
            {
                try
                {

                    if (x.Surname != null)
                    {

                        if (x.Surname != "" && Main.translationDict.ContainsKey(x.Surname))
                        {
                            //Debug.Log("Found one ! : " + x.Surname);
                            FieldInfo fi = x.GetType().GetField("Surname");
                            fi.SetValue(x, Main.translationDict[x.Surname]);
                            //Debug.Log("Modified Name = " + x.Surname);
                        }
                        else
                        {
                            if (x.Surname != null && x.Surname != "")
                            {
                                Main.MissingSurnames.Add(x.Surname);
                            }
                        }
                    }
                }

                catch (Exception e)
                {

                }



            }
        }
    }

    [HarmonyPatch(typeof(LocalMonasticTitles), "Init")]

    static class LocalMonasticTitles_Patch
    {

        static void Postfix(LocalMonasticTitles __instance)
        {
            foreach (MonasticTitleItem x in __instance.MonasticTitles)
            {
                try
                {

                    if (x.Name != null)
                    {

                        if (x.Name != "" && Main.translationDict.ContainsKey(x.Name))
                        {
                            //Debug.Log("Found one MT ! : " + x.Name);
                            FieldInfo fi = x.GetType().GetField("Name");
                            //Slightly different patching method. Instead of patching the name display function, I'm directly adding a space before the Monastic Title **while** they are getting translated. May be a bit dangerous. Not sure.
                            fi.SetValue(x, " " + Main.translationDict[x.Name]);
                            //Debug.Log("Modified MT = " + x.Name);
                        }
                        else
                        {
                            if (x.Name != null && x.Name != "")
                            {
                                Main.MissingMonasticTitlesDict.Add(x.Name);
                            }
                        }
                    }
                }

                catch (Exception e)
                {

                }



            }
        }
    }
    [HarmonyPatch(typeof(LocalZangNames), "Init")]

    static class LocalZangNames_Patch
    {

        static void Postfix(LocalZangNames __instance)
        {
            foreach (ZangNameItem x in __instance.ZangNameCore)
            {
                try
                {

                    if (x.Name != null)
                    {

                        if (x.Name != "" && Main.translationDict.ContainsKey(x.Name))
                        {
                            //Debug.Log("Found one ZN ! : " + x.Name);
                            FieldInfo fi = x.GetType().GetField("Name");
                            //Slightly different patching method. Instead of patching the name display function, I'm directly adding a space before the Monastic Title **while** they are getting translated. May be a bit dangerous. Not sure.
                            fi.SetValue(x, Main.translationDict[x.Name]);
                            //Debug.Log("Modified ZN = " + x.Name);
                        }
                        else
                        {
                            if (x.Name != null && x.Name != "")
                            {
                                Main.MissingZangNames.Add(x.Name);
                            }
                        }
                    }
                }

                catch (Exception e)
                {

                }



            }
        }
    }
    /*[HarmonyPatch(typeof(LocalStringManager), "Get", new Type[] { typeof(ushort)})]
    static class FullName_Patch
    {
        static void Postfix(LocalStringManager __instance, ref ushort id, ref string __result)
        {
            if(__result == "Taiwu")
            {
                __result = "Taiwu ";
            }
        }
    }*/
    /*[HarmonyPatch(typeof(NameCenter), "GetMonasticTitleOrName")]
    static class NameCenter_Patch_01
    {
        static void Postfix(NameCenter __instance, ref ValueTuple<string, string> __result)
        {
            __result.Item2 = " " + __result.Item2;
        }
    }*/

    [HarmonyPatch(typeof(NameCenter), "GetName")]
    static class NameCenter_Patch_02
    {
        static void Postfix(NameCenter __instance, ref ValueTuple<string, string> __result)
        {
            if(__result.Item1 != null && __result.Item1 != "" && __result.Item2 != null && __result.Item2 != "")
            { 
            __result.Item2 = " " + __result.Item2;
            }
        }
    }

    [HarmonyPatch(typeof(NameCenter), "GetMonasticTitle")]
    static class NameCenter_Patch_03
    {
        static void Postfix(NameCenter __instance, ref NameRelatedData data, ref string __result)
        {
            if((data.MonkType & 128) > 0)
            {
                MonasticTitleItem[] config = LocalMonasticTitles.Instance.MonasticTitles;
                string seniorityName = config[(int)data.MonasticTitle.SeniorityId].Name;
                string suffixName = config[(int)data.MonasticTitle.SuffixId].Name;
                OrganizationItem orgConfig = Organization.Instance[data.OrgTemplateId];
                short orgMemberId = orgConfig.Members[(int)data.OrgGrade];
                OrganizationMemberItem orgMemberConfig = OrganizationMember.Instance[orgMemberId];
                string titleSuffix = orgMemberConfig.MonasticTitleSuffixes[(int)data.Gender];
                __result = seniorityName + suffixName + " " + titleSuffix;
            }
        }
    }

    [HarmonyPatch(typeof(NameCenter), "HasInvalidCharForName")]
    static class REGEX_Patch
    {
        static void Postfix(ref string nameString, ref bool __result )
        {

            //Debug.Log("NameString = " + nameString);
            //Debug.Log("Result = " + __result);
            Regex InvalidCharRegex = new Regex("[\\u0022\\d~!@#$%^&*()_+-=\\[\\]{}\\\\|;:',.<>/?·`！￥…（）—、【】：；‘’“”《》，。？]");
            __result = InvalidCharRegex.IsMatch(nameString);
        }
    }



    [HarmonyPatch(typeof(FullName), "GetName")]
    static class FullName_GetName_Patch
    {
        static void Postfix(FullName __instance, ref ValueTuple<string, string> __result)
        {

            if (__result.Item2.Length > 1)
            {
                __result.Item2 = __result.Item2.ToLower();
                __result.Item2 = char.ToUpper(__result.Item2[0]) + __result.Item2.Substring(1);

                Debug.Log("Result.Item1 = " + __result.Item1);

                Debug.Log("Result.Item2 = " + __result.Item2);
                if(__result.Item1 != null && !__result.Item1.EndsWith(" "))
                {
                    __result.Item1 = __result.Item1 + " ";
                }
            }
        }
    }

    [HarmonyPatch(typeof(WorldInfo), "Deserialize")]
    static class WorldInfo_GetSerializedSize_Patch
    {
        static void Postfix(WorldInfo __instance)
        {
            Debug.Log("TGN before = " +__instance.TaiwuGivenName);
            __instance.TaiwuGivenName = " " + __instance.TaiwuGivenName;
            Debug.Log("TGN after = " + __instance.TaiwuGivenName);
        }
    }

    [HarmonyPatch(typeof(EventModel), "GetOptionConditionContent")]
    static class EventModel_Patch
    {

    static void Postfix(EventModel __instance, ref string __result, ref short index)
        {
            string[] hello = new string[]{};
            string optionAvailableLanguageFileName = "EventOptionTips_" + LocalStringManager.CurLanguageKey;
            var lines  = File.ReadAllLines(Path.Combine(Paths.GameRootPath, "Languages", "en", "EventOptionTips_CN.txt"));
            string filePath = Path.Combine("RemakeResources/Data/Language_EventOptionTips", optionAvailableLanguageFileName);
            ResLoader.Load<TextAsset>(filePath, delegate (TextAsset asset)
            {
                hello = asset.text.Split(new char[]
                {
                '\n'
                });
            }, null);



            if(hello.Count() == lines.Count() + 1)
            {
                for(int i = 0; i< lines.Count(); i++)
                {
                    if(__result == hello[i])
                    {
                        Debug.Log("Original String = " + __result);
                        __result = lines[i];
                        Debug.Log("Updated Line = " + __result);
                    }
                }
            }


        }
    }
    [HarmonyPatch(typeof(EventInputRequestData), "Deserialize")]
    static class EventInputRequestData_Deserialize_Patch
    {
        static void Postfix(EventInputRequestData __instance)
        {

            __instance.NumberRange[1] = __instance.NumberRange[1] + 6;

        }
    }
    [HarmonyPatch(typeof(TMP_InputField), "characterLimit", MethodType.Getter)]
    static class TMP_InputField_characterLimit_Patch
    {
        static void Postfix(TMP_InputField __instance, ref int __result)
        {

            __result = __result + 6;
        }
    }
    public static class DictionaryExtensions
    {

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
     public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }
    }
}
