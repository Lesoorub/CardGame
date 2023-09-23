using UnityEngine;

/// <summary>
/// Предоставляет интерфейс взаимодействия с расположением картю.
/// Управляет расположением карт.
/// </summary>
public class CardsLayout : MonoBehaviour
{
    public Transform TargetsParent;
    public Transform CardsParent;
    public Transform SelectedCardParent;
    public GameObject TargetPrefab;
    public float MovementSpeed = 1;
    public float SelectedMovementSpeed = 1;
    public bool MouseEffect = true;
    public bool SelectedCardParentFollowForMouse = false;
    public float MinDistance = 300;
    public float WideDistance = 300;
    public float CardsElevate = 60;
    [HideInInspector]
    public Transform SelectedCard;

    void Start()
    {
        this.InvalidateTargets();
    }


    void Update()
    {
        if (this.SelectedCardParent != null && this.SelectedCard != null)
        {
            if (this.SelectedCardParentFollowForMouse)
                this.SelectedCardParent.position = Input.mousePosition;
            this.SelectedCard.position = Vector3.Lerp(
                this.SelectedCard.position,
                this.SelectedCardParent.position,
                this.SelectedMovementSpeed * Time.deltaTime);
        }

        if (this.CardsParent == null || this.TargetsParent == null) return;
        if (this.CardsParent.childCount != this.TargetsParent.childCount)
        {
            this.InvalidateTargets();
        }
        if (this.CardsParent.childCount == this.TargetsParent.childCount)
        {
            for (int k = 0; k < this.CardsParent.childCount; k++)
            {
                var card = this.CardsParent.GetChild(k);
                var target = this.TargetsParent.GetChild(k);

                var targetPos = target.position;
                if (this.MouseEffect && Application.isPlaying)
                {
                    targetPos += Vector3.up * this.CardsElevate *
                        Mathf.Max(Mathf.Min(
                            this.MinDistance,
                            this.WideDistance + this.MinDistance - Vector3.Distance(Input.mousePosition, target.position)), 0) / MinDistance;
                }
                card.position = this.GetNextCardPosition(card.position, targetPos);
            }
        }
    }

    /// <summary>
    /// Метод перемещения карты, возвращая промежуточную позицию.
    /// </summary>
    /// <param name="cardPos">Текущая позиция карты.</param>
    /// <param name="targetPos">Конечная позиция карты.</param>
    /// <returns>Новая позиция карты.</returns>
    public Vector3 GetNextCardPosition(Vector3 cardPos, Vector3 targetPos)
    {
        return Vector3.Lerp(
                    cardPos,
                    targetPos,
                    this.MovementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Обновляет целевые позиция карт.
    /// Каждая карта имеет объект к координатам которых она стримиться, 
    /// именно эти объекты являются целями, и предоставляют целевые 
    /// позиции карт.
    /// </summary>
    void InvalidateTargets()
    {
        if (this.CardsParent == null || this.TargetsParent == null) return;
        if (this.CardsParent.childCount == this.TargetsParent.childCount) return;
        if (this.CardsParent.childCount < this.TargetsParent.childCount)
        {
            for (int k = this.CardsParent.childCount; k < this.TargetsParent.childCount; k++)
            {
                this.DestroyEditorSupported(this.TargetsParent.GetChild(k).gameObject);
            }
        }
        else
        {
            for (int k = this.TargetsParent.childCount; k < this.CardsParent.childCount; k++)
            {
                if (this.TargetPrefab == null)
                {
                    var target = new GameObject("point");
                    target.transform.SetParent(this.TargetsParent);
                }
                else
                {
                    Instantiate(this.TargetPrefab, this.TargetsParent);
                }
            }
        }
    }

    /// <summary>
    /// Уничтожение объекта с поддержко в режиме редактора без включенной игры.
    /// </summary>
    /// <param name="obj"></param>
    void DestroyEditorSupported(Object obj)
    {
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }

    /// <summary>
    /// Выбирает карту, перемещая ее в цент экрана, поддерживает снятие текущего выделения значением null.
    /// </summary>
    /// <param name="card">Выбираемая карта или null для снятия выделения.</param>
    public void SelectCard(Transform card)
    {
        if (this.SelectedCard != null)
            this.SelectedCard.SetParent(this.CardsParent);

        this.SelectedCard = card;
        if (this.SelectedCard != null)
        {
            card.position = this.SelectedCardParent.position;
            card.SetParent(this.SelectedCardParent);
        }
    }
}
