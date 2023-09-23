using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Анимация простоя существа.
/// </summary>
public class EntityIsSteppingAnimation : MonoBehaviour
{
    public Image img;
    public float Speed = 1;
    public bool PositiveDirection = true;

    private void OnEnable()
    {
        this.img.fillAmount = 0;
    }

    private void Update()
    {
        this.img.fillAmount += Time.deltaTime * this.Speed * (this.PositiveDirection ? 1 : -1);
        if (this.img.fillAmount >= 1)
        {
            this.img.fillAmount = 1;
            this.PositiveDirection = !this.PositiveDirection;
            this.img.fillClockwise = false;
        }
        if (this.img.fillAmount <= 0)
        {
            this.img.fillAmount = 0;
            this.PositiveDirection = !this.PositiveDirection;
            this.img.fillClockwise = true;
        }
    }
}
