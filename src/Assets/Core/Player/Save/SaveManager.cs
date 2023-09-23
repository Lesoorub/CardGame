using System.Linq;
using Assets.Core.Entity;
using Assets.Core.Items.Containers;
using Assets.Core.Player.Save;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Editors
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(SaveManager))]
    public class SaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = this.target as SaveManager;
            if (GUILayout.Button("Save"))
            {
                script.Save();
            }
            if (GUILayout.Button("Load"))
            {
                script.Load();
            }
            if (GUILayout.Button("Clear save"))
            {
                script.ClearSave();
            }
            this.DrawDefaultInspector();
        }
    }
#endif
}

namespace Assets.Core.Player.Save
{
    public class SaveManager : MonoBehaviour
    {
        static SaveManager instance;
        public static SaveManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = Resources.Load<GameObject>(Paths.SaveManager);
                    instance = obj.GetComponent<SaveManager>();
                    if (instance == null)
                    {
                        Debug.LogError($"Статический префаб по адресу: '{Paths.SaveManager}' должен иметь скрипт '{typeof(SaveManager).Name}', но не имеет его");
                        return null;
                    }
                    instance.Load();
                }
                return instance;
            }
        }
        public string SaveName = "save1";
        public PlayerSave Data;
        public SlotsContainer Equipment;
        public ItemsContainer Inventory;

        public UnityEvent<SaveManager> AfterLoad = new UnityEvent<SaveManager>();
        public UnityEvent<SaveManager> BeforeSave = new UnityEvent<SaveManager>();

        private void OnEnable()
        {
            this.Inventory.OnChanged.AddListener(this.AnyInventory_OnChanged);
            this.Equipment.OnChanged.AddListener(this.AnyInventory_OnChanged);
        }

        private void OnDisable()
        {
            this.Inventory.OnChanged.RemoveListener(this.AnyInventory_OnChanged);
            this.Equipment.OnChanged.RemoveListener(this.AnyInventory_OnChanged);
        }

        public void Save()
        {
            this.BeforeSave?.Invoke(this);

            this.Data.InventoryItems = this.Inventory.Items.ToList();
            this.Data.Equipments = this.Equipment.Items.ToList();

            this.Data.Save(this.SaveName);
        }

        public void Load()
        {
            this.Data = PlayerSave.Load(this.SaveName);

            Application.targetFrameRate = 90;

            this.Inventory.Items = this.Data.InventoryItems.ToArray();
            this.Equipment.Items = this.Data.Equipments.ToArray();

            this.AfterLoad?.Invoke(this);
        }

        public void ClearSave()
        {
            PlayerSave.Delete(this.SaveName);
            this.Load();
        }

        void AnyInventory_OnChanged(IItemsContainer container)
        {

        }
    }
}