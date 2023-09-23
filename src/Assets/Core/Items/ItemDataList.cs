using System.Linq;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Core.Items
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemDataList))]
    public class ItemDataListEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("UpdateList()"))
            {
                (target as ItemDataList)?.UpdateList();
            }
            base.OnInspectorGUI();
        }
    }
#endif

    [CreateAssetMenu(fileName = "new " + nameof(ItemDataList), menuName = Paths.ItemsDir + nameof(ItemDataList))]
    public class ItemDataList : ScriptableObject
    {
        public List<ItemData> items = new List<ItemData>();
        private static ItemDataList list;
        public static ItemDataList Instance
        {
            get
            {
                if (list == null)
                {
                    list = Resources.Load<ItemDataList>(Paths.ItemDataList);
                }
                return list;
            }
        }
        public ItemData GetByID(string Id)
        {
            return items.FirstOrDefault(x => string.Equals(x.ID, Id));
        }

        public void UpdateList()
        {
#if UNITY_EDITOR
            items.Clear();
            foreach (var path in AssetDatabase.GetAllAssetPaths())
            {
                var item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item == null) continue;
                items.Add(item);
            }
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
