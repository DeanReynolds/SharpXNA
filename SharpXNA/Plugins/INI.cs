using System;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

namespace SharpXNA.Plugins
{
    public class INI
    {
        public string FilePath;
        public char? Delimiter;
        public OrderedDictionary Nodes;

        public Flags? SetFlags;
        [Flags] public enum Flags { Quoted = 1, Apostrophized = 2, NoWhitespaces = 4 }

        public INI(Flags? flags = null) { SetFlags = flags; Nodes = new OrderedDictionary(); }
        public INI(string filePath, Flags? flags = null) { SetFlags = flags; FilePath = filePath; Nodes = new OrderedDictionary(); }

        public string this[int index]
        {
            get
            {
                var key = Nodes.KeyFromIndex(index);
                if (key == null) return null;
                if (Nodes.Contains(key))
                {
                    var value = Nodes[key].ToString();
                    if (SetFlags.HasValue)
                    {
                        if (SetFlags.Value.HasFlag(Flags.Quoted) && (value.CountOf('"') >= 2)) value = value.Between((value.IndexOf('"') + 1), value.LastIndexOf('"'));
                        else if (SetFlags.Value.HasFlag(Flags.Apostrophized) && (value.CountOf('\'') >= 2)) value = value.Between((value.IndexOf('\'') + 1), value.LastIndexOf('\''));
                        if (SetFlags.Value.HasFlag(Flags.NoWhitespaces)) value = Regex.Replace(value, @"\s+", string.Empty);
                    }
                    return value;
                }
                return null;
            }
        }
        public string this[string name] => this[Nodes.IndexOfKey(name)];
        public string this[string section, string name] => this[Nodes.IndexOfKey(section + "." + name)];

        public void Set(string name, string value, bool save = false) { Set(null, name, value, save); }
        public void Set(string section, string name, string value, bool save = false)
        {
            var key = ((!string.IsNullOrEmpty(section) ? (section + ".") : string.Empty) + name);
            if (Nodes.Contains(key)) { if (((string)Nodes[key]) != value) { Nodes[key] = value; if (save) Save(); } } else { Nodes.Add(key, value); if (save) Save(); }
        }
        public bool Remove(string key, bool save = false) { if (Nodes.Contains(key)) { Nodes.Remove(key); if (save) Save(); return true; } return false; }
        public bool Remove(string section, string name, bool save = false) { var key = (section + "." + name); if (Nodes.Contains(key)) { Nodes.Remove(key); if (save) Save(); return true; } return false; }

        public void Save(string filePath = null)
        {
            if (filePath != null) FilePath = filePath;
            using (var writer = new StreamWriter(FilePath))
            {
                if (Delimiter.HasValue) writer.WriteLine("[ini]delimiter='" + Delimiter + "'");
                if (SetFlags.HasValue) writer.WriteLine("[ini]flags" + (Delimiter ?? '=') + SetFlags);
                string wroteSection = null;
                for (var i = 0; i < Nodes.Count; i++)
                {
                    string key = Nodes.KeyFromIndex(i).ToString(), section = string.Empty, name = string.Empty, value = Nodes[i].ToString();
                    if (key.Contains('.'))
                    {
                        var indexOfSeparator = key.IndexOf('.');
                        section = key.Substring(0, indexOfSeparator);
                        name = key.Substring((indexOfSeparator + 1), (key.Length - (indexOfSeparator + 1)));
                    }
                    else { if (wroteSection != null) section = null; name = key; }
                    if (!string.IsNullOrEmpty(section))
                    {
                        if (section == "ini") { writer.Write("[ini]"); wroteSection = null; }
                        else if (wroteSection != section) { writer.WriteLine("[" + section + "]"); wroteSection = section; }
                    }
                    else if (wroteSection != null) { writer.WriteLine("{global}"); wroteSection = null; }
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        if (i == (Nodes.Count - 1)) writer.Write(name + " " + (!Delimiter.HasValue ? '=' : Delimiter.Value) + " " + value);
                        else writer.WriteLine(name + " " + (Delimiter ?? '=') + " " + value);
                }
                writer.Close();
            }
        }

        public static INI ReadFile(string file, Flags? flags = null) { if (File.Exists(file)) return ReadString(File.ReadAllText(file), file, true, flags); else return new INI(file, flags); }
        public static INI ReadString(string text, Flags? flags = null) { return ReadString(text, "temp.ini", false, flags); }
        public static INI ReadString(string text, string filePath, bool fromFile, Flags? flags = null)
        {
            if (fromFile && !File.Exists(filePath))
                return new INI(filePath, flags);
            var ini = new INI(filePath, flags);
            var lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            char? delimiter = null;
            var section = string.Empty;
            foreach (var t in lines)
            {
                var line = t.Trim();
                if (line.StartsWith(";") || line.StartsWith("#")) continue;
                if (line.ToLower() == "{global}") section = string.Empty;
                else
                {
                    var newSection = Regex.Match(line, @"(?<=\[).*(?=\])").Value.Trim();
                    if (!string.IsNullOrEmpty(newSection)) section = newSection;
                }
                var name = Regex.Match(line, (@"(?<=^\p{Zs}*|])[^]" + (!delimiter.HasValue ? "=:" : INIExtentions.RegexEscape(delimiter.Value)) + "]*(?=" +
                                                 (!delimiter.HasValue ? "=|:" : INIExtentions.RegexEscape(delimiter.Value)) + ")")).Value.Trim();
                var value = string.Empty;
                if (ini.SetFlags.HasValue)
                {
                    value = Regex.Match(line, "(?<==|:).*").Value.Trim();
                    if (ini.SetFlags.Value.HasFlag(Flags.Quoted) && (value.CountOf('"') >= 2)) value = value.Between(value.IndexOf('"'), (value.LastIndexOf('"') + 1));
                    else if (ini.SetFlags.Value.HasFlag(Flags.Apostrophized) && (value.CountOf('\'') >= 2)) value = value.Between(value.IndexOf('\''), (value.LastIndexOf('\'') + 1));
                    else value = Regex.Match(value, "[^;#]*").Value.Trim();
                }
                else value = Regex.Match(line, "(?<=" + (!delimiter.HasValue ? "=|:" : INIExtentions.RegexEscape(delimiter.Value)) + ")[^;#]*").Value.Trim();
                if (section == "ini")
                {
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                        if (name == "delimiter")
                        {
                            if (value.CountOf('"') >= 2) delimiter = value.Between((value.IndexOf('"') + 1), value.LastIndexOf('"'))[0];
                            if (value.CountOf('\'') >= 2) delimiter = value.Between((value.IndexOf('\'') + 1), value.LastIndexOf('\''))[0];
                            ini.Delimiter = delimiter;
                        }
                        else if (name == "flags") ini.SetFlags = (Flags)Enum.Parse(typeof(Flags), value);
                    section = string.Empty;
                }
                else if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value)) ini.Nodes.Add(((!string.IsNullOrEmpty(section) ? (section + ".") : string.Empty) + name), value);
            }
            return ini;
        }
        public static INI ReadStream(Stream stream, Flags? flags = null) { using (var reader = new StreamReader(stream)) return ReadString(reader.ReadToEnd(), flags); }
        public static INI ReadStream(Stream stream, string file, Flags? flags = null) { using (var reader = new StreamReader(stream)) return ReadString(reader.ReadToEnd(), file, true, flags); }
    }

    public static class INIExtentions
    {
        public static int IndexOfKey(this OrderedDictionary dictionary, string key) { for (var i = 0; i < dictionary.Count; i++) if (dictionary[i] == dictionary[key]) return i; return -1; }
        public static int IndexOfValue(this OrderedDictionary dictionary, object value) { for (var i = 0; i < dictionary.Count; i++) if (dictionary[i] == value) return i; return -1; }
        public static object KeyFromIndex(this OrderedDictionary dictionary, int index) { if (index == -1) return null; return ((dictionary.Count > index) ? dictionary.Cast<DictionaryEntry>().ElementAt(index).Key : null); }
        public static int CountOf(this string @string, char @char) { var count = 0; foreach (var t in @string) if (t == @char) count++; return count; }
        public static string Between(this string @string, int startIndex, int endIndex) { return @string.Substring(startIndex, (endIndex - startIndex)); }
        public static string RegexEscape(char @char) { return Regex.Escape(@char.ToString()); }
    }
}