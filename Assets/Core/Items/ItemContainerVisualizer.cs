using UnityEngine;
namespace Assets.Core.Items.UI
{
    public class ItemContainerVisualizer : MonoBehaviour
    {
        public ItemStackVisualizer[] visualizers;
        public IItemsContainer container;
        public void Show(IItemsContainer container)
        {
            if (container != null)
            {
                container?.OnChanged?.RemoveListener(From);
            }
            this.container = container;
            container?.OnChanged?.AddListener(From);
            From(container);
        }
        public void From(IItemsContainer container)
        {
            if (container == null) return;
            var items = container.Items;
            for (int k = 0; k < Mathf.Min(items.Length, visualizers.Length); k++)
            {
                if (visualizers[k] == null)
                    continue;
                visualizers[k].SetItem(items[k]);
            }
        }
        private void OnEnable()
        {
            container?.OnChanged?.AddListener(From);
        }
        private void OnDisable()
        {
            container?.OnChanged?.RemoveListener(From);
        }
    }

}