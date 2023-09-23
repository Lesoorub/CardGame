using System.Collections.Generic;
using Assets.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.PlayerPrefs;
using UnityEngine;

namespace Assets.Core.Entity
{
    /// <summary>
    /// Данные игрока.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.EntitiesDir + nameof(PlayerData), fileName = "new " + nameof(PlayerData))]
    public class PlayerData : EntityData
    {
        PlayerSave m_data;
        public PlayerSave Data
        {
            get
            {
                if (this.m_data == null)
                    this.m_data = PlayerSave.Load("save1");
                return this.m_data;
            }
        }
    }

    /// <summary>
    /// Объект содержащий информацию о игроке. Поддерживает сериализацию в JSON.
    /// </summary>
    public class PlayerSave
    {
        public string LastNodeName = string.Empty;
        public List<string> VisitedNodes = new List<string>();
        public SerializableVector2 MapPosition = Vector2.zero;
        public List<ItemStack> Equipments = new List<ItemStack>();
        public List<ItemStack> InventoryItems = new List<ItemStack>();
        public List<string> GoPath = new List<string>();

        /// <summary>
        /// Создает сохранение игрока из данных сохраненных в <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static PlayerSave Load(string saveName)
        {
            var data = new PlayerSave();

            if (JsonPlayerPrefs.HasJson(saveName))
            {
                var json = JsonPlayerPrefs.LoadJson(saveName);
                Debug.Log(json);
                data = json.ToObject<PlayerSave>();
            }
            return data;
        }

        /// <summary>
        /// Удаляет данные из <see cref="PlayerPrefs"/>.
        /// </summary>
        /// <param name="saveName"></param>
        public static void Delete(string saveName)
        {
            JsonPlayerPrefs.DeleteJson(saveName);
        }

        /// <summary>
        /// Обновляет сохраненную информацию о игроке.
        /// </summary>
        /// <param name="saveName"></param>
        public void Save(string saveName)
        {
            JsonPlayerPrefs.DeleteJson(saveName);
            JsonPlayerPrefs.SaveJson(saveName, JObject.FromObject(this));
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Vector2 поддерживающий сериализацию <see cref="Newtonsoft.Json.JsonConvert.SerializeObject(object?)"/>.
    /// </summary>
    [System.Serializable]
    public class SerializableVector2
    {
        public float x;
        public float y;

        [JsonIgnore]
        public Vector2 UnityVector
        {
            get
            {
                return new Vector2(x, y);
            }
        }

        public SerializableVector2(Vector2 v)
        {
            x = v.x;
            y = v.y;
        }

        public static List<SerializableVector2> GetSerializableList(List<Vector3> vList)
        {
            List<SerializableVector2> list = new List<SerializableVector2>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(new SerializableVector2(vList[i]));
            }
            return list;
        }

        public static List<Vector3> GetSerializableList(List<SerializableVector2> vList)
        {
            List<Vector3> list = new List<Vector3>(vList.Count);
            for (int i = 0; i < vList.Count; i++)
            {
                list.Add(vList[i].UnityVector);
            }
            return list;
        }

        public static implicit operator Vector2(SerializableVector2 vector2) => vector2.UnityVector;
        public static implicit operator Vector3(SerializableVector2 vector2) => vector2.UnityVector;
        public static implicit operator SerializableVector2(Vector2 vector2) => new SerializableVector2(vector2);
        public static implicit operator SerializableVector2(Vector3 vector2) => new SerializableVector2(vector2);
    }
}