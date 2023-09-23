using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Core.Entity.Preview
{
    /// <summary>
    /// Визуализатор превью существа.
    /// </summary>
    public class EntityPreviewVisualizer : MonoBehaviour
    {
        public TMP_Text HealthLabel;
        public Slider HealthBar;
        public Image PreviewImage;
        [SerializeField]
        Image Selected;
        public Color32 EnemySelection = Color.red;
        public Color32 FriendSelection = Color.green;
        public bool IsSelected
        {
            get => this.Selected.enabled;
            set => this.Selected.enabled = value;
        }
        public bool IsSelectFriend
        {
            set => this.Selected.color = value ? this.FriendSelection : this.EnemySelection;
        }
        public TMP_Text ShieldsLabel;
        public Image ShieldsImage;
        public EntityIsSteppingAnimation IsSteppingAnimator;
        public bool NowIsStepping
        {
            get => this.IsSteppingAnimator.gameObject.activeSelf;
            set => this.IsSteppingAnimator.gameObject.SetActive(value);
        }

        public string HealthFormat = "{0} / {1}";
        public string ShieldsFormat = "{0}";
        public Entity entity { get; private set; }
        public void Show(Entity entity)
        {
            this.entity = entity;
            entity?.OnChanged.AddListener(this.From);
            this.From(entity);
        }

        private void OnDestroy()
        {
            this.entity?.OnChanged.RemoveListener(this.From);
        }

        void From(Entity entity)
        {
            this.PreviewImage.sprite = entity.Data.preview.Sprite;
            this.PreviewImage.color = entity.Data.preview.Color;
            this.HealthBar.value = (float)entity.CurrentHealth / entity.Data.MaximumHealth;
            this.HealthLabel.text = string.Format(
                format: this.HealthFormat,
                entity.CurrentHealth,
                entity.Data.MaximumHealth);
            this.ShieldsImage.gameObject.SetActive(entity.Shields > 0);
            this.ShieldsLabel.text = string.Format(this.ShieldsFormat, entity.Shields);
        }
    }
}
