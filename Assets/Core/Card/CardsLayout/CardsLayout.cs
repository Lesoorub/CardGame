using UnityEngine;

/// <summary>
/// ������������� ��������� �������������� � ������������� �����.
/// ��������� ������������� ����.
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
    /// ����� ����������� �����, ��������� ������������� �������.
    /// </summary>
    /// <param name="cardPos">������� ������� �����.</param>
    /// <param name="targetPos">�������� ������� �����.</param>
    /// <returns>����� ������� �����.</returns>
    public Vector3 GetNextCardPosition(Vector3 cardPos, Vector3 targetPos)
    {
        return Vector3.Lerp(
                    cardPos,
                    targetPos,
                    this.MovementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// ��������� ������� ������� ����.
    /// ������ ����� ����� ������ � ����������� ������� ��� ����������, 
    /// ������ ��� ������� �������� ������, � ������������� ������� 
    /// ������� ����.
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
    /// ����������� ������� � ��������� � ������ ��������� ��� ���������� ����.
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
    /// �������� �����, ��������� �� � ���� ������, ������������ ������ �������� ��������� ��������� null.
    /// </summary>
    /// <param name="card">���������� ����� ��� null ��� ������ ���������.</param>
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
