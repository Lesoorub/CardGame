using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityPlayerPrefs = UnityEngine.PlayerPrefs;

namespace Newtonsoft.Json.PlayerPrefs
{
    public static class JsonPlayerPrefs
    {
        public const string JsonMetaName = "$meta";
        public static void SaveJson(string path, JObject json)
        {
            var meta = new List<JsonMeta>();
            foreach (var (name, token) in json)
            {
                var newpath = Path.Combine(path, name);
                SaveItem(newpath, token);
                meta.Add(new JsonMeta(name, token.Type));
            }
            UnityPlayerPrefs.SetString(Path.Combine(path, JsonMetaName), JsonConvert.SerializeObject(meta));
        }
        public static void DeleteJson(string path)
        {
            var meta_path = Path.Combine(path, JsonMetaName);
            if (!UnityPlayerPrefs.HasKey(meta_path)) return;
            var meta = JsonConvert.DeserializeObject<List<JsonMeta>>(UnityPlayerPrefs.GetString(meta_path));
            foreach (var m in meta)
            {
                var newpath = Path.Combine(path, m.n);
                DeleteItem(newpath, m.t);
            }
            UnityPlayerPrefs.DeleteKey(meta_path);
        }
        public static bool HasJson(string path)
        {
            var meta_path = Path.Combine(path, JsonMetaName);
            return UnityPlayerPrefs.HasKey(meta_path);
        }
        public static JObject LoadJson(string path)
        {
            JObject json = new JObject();
            var meta_path = Path.Combine(path, JsonMetaName);
            if (!UnityPlayerPrefs.HasKey(meta_path)) return json;
            var meta = JsonConvert.DeserializeObject<List<JsonMeta>>(UnityPlayerPrefs.GetString(meta_path));
            foreach (var m in meta)
            {
                var newpath = Path.Combine(path, m.n);
                json[m.n] = LoadItem(newpath, m.t);
            }
            return json;
        }

        private static void SaveItem(string path, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    SaveJson(path, (JObject)token);
                    break;
                case JTokenType.Array:
                    var array = (JArray)token;
                    UnityPlayerPrefs.SetInt(Path.Combine(path, "Count"), array.Count);
                    for (int k = 0; k < array.Count; k++)
                    {
                        var item = array[k];
                        var item_type = (int)item.Type;
                        UnityPlayerPrefs.SetInt(Path.Combine(path, "Type", k.ToString()), item_type);
                        SaveItem(Path.Combine(path, k.ToString()), item);
                    }
                    break;
                case JTokenType.Integer:
                    if ((int)token != 0)
                        UnityPlayerPrefs.SetInt(path, (int)token);
                    break;
                case JTokenType.Float:
                    if ((float)token != 0)
                        UnityPlayerPrefs.SetFloat(path, (float)token);
                    break;
                case JTokenType.String:
                    if (token != null && (string)token != null)
                        UnityPlayerPrefs.SetString(path, (string)token);
                    break;
                case JTokenType.Boolean:
                    UnityPlayerPrefs.SetInt(path, ((bool)token) ? 1 : 0);
                    break;
            }
        }
        private static JToken LoadItem(string path, JTokenType type)
        {
            switch (type)
            {
                case JTokenType.Object:
                    return LoadJson(path);
                case JTokenType.Array:
                    var count = UnityPlayerPrefs.GetInt(Path.Combine(path, "Count"));
                    JArray arr = new JArray();
                    for (int k = 0; k < count; k++)
                    {
                        var elements_type = (JTokenType)UnityPlayerPrefs.GetInt(Path.Combine(path, "Type", k.ToString()));
                        arr.Add(LoadItem(Path.Combine(path, k.ToString()), elements_type));
                    }
                    return arr;
                case JTokenType.Integer:
                    return UnityPlayerPrefs.GetInt(path);
                case JTokenType.Float:
                    return UnityPlayerPrefs.GetFloat(path);
                case JTokenType.String:
                    return UnityPlayerPrefs.GetString(path);
                case JTokenType.Boolean:
                    return UnityPlayerPrefs.GetInt(path) == 1 ? true : false;
            }
            return null;
        }
        private static void DeleteItem(string path, JTokenType type)
        {
            switch (type)
            {
                case JTokenType.Object:
                    DeleteJson(path);
                    break;
                case JTokenType.Array:
                    var count_path = Path.Combine(path, "Count");
                    var count = UnityPlayerPrefs.GetInt(count_path);
                    for (int k = 0; k < count; k++)
                    {
                        var elements_type = (JTokenType)UnityPlayerPrefs.GetInt(Path.Combine(path, "Type", k.ToString()));
                        UnityPlayerPrefs.DeleteKey(Path.Combine(path, "Type", k.ToString()));
                        DeleteItem(Path.Combine(path, k.ToString()), elements_type);
                    }
                    UnityPlayerPrefs.DeleteKey(count_path);
                    break;
                default:
                    UnityPlayerPrefs.DeleteKey(path);
                    break;
            }
        }
        public struct JsonMeta
        {
            /// <summary>
            /// Name
            /// </summary>
            public string n;
            /// <summary>
            /// Type
            /// </summary>
            public JTokenType t;

            public JsonMeta(string name, JTokenType type)
            {
                this.n = name;
                this.t = type;
            }
        }

    }
}