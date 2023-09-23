using UnityEngine;

namespace Assets.Core.Entity.Preview
{
    /// <summary>
    /// <see cref="ScriptableObject"/> ������ ��������.
    /// </summary>
    [CreateAssetMenu(menuName = Paths.EntityPreviewDir + nameof(EntityPreview), fileName = "new " + nameof(EntityPreview))]
    public class EntityPreview : ScriptableObject
    {
        public Sprite Sprite;
        public Color Color;
        public string Name;
        public string Id => this.name;
    }

}