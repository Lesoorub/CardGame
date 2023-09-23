using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ��������� (�����) ����.
/// </summary>
public class InvokeCardQueue : MonoBehaviour
{
    /// <summary>
    /// ����� � ������� ����� ���������� ����� ��� ������.
    /// </summary>
    public Transform dropPoint;

    /// <summary>
    /// ����� ������������ �����.
    /// </summary>
    public Transform ShowPoint;

    /// <summary>
    /// �������� ��������� ����.
    /// </summary>
    public Transform StorageParent;

    /// <summary>
    /// ����� ������������ �����.
    /// </summary>
    public float ShowTime = 1;

    /// <summary>
    /// �������� ����������� ����.
    /// </summary>
    public float CardMovementSpeed = 1;

    public AnimationCurve ShowingGrowingModifer = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Vector3 ShowingGrowing = new Vector3(1, 1, 1);
    public AnimationCurve EndDropGrowingModifer = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Vector3 EndDropGrowing = new Vector3(0, 0, 0);

    public Queue<Transform> queue = new Queue<Transform>();

    /// <summary>
    /// ���������� ����� � �������.
    /// </summary>
    /// <param name="card">��������� �����.</param>
    public void Enqueue(Transform card)
    {
        card.SetParent(this.StorageParent ?? this.transform);
        this.queue.Enqueue(card);
        if (!this.DropCardAnimationEnabled)
            this.StartCoroutine(this.DropCards());
    }

    private bool DropCardAnimationEnabled = false;
    
    /// <summary>
    /// ������� ������ ����.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DropCards()
    {
        this.DropCardAnimationEnabled = true;
        while (this.queue.Count > 0)
        {
            var card = this.queue.Dequeue();
            card.SetParent(this.ShowPoint ?? this.transform);

            yield return this.StartCoroutine(this.MoveTo(
                obj: card,
                to: this.ShowPoint ?? this.transform,
                finalSize: this.ShowingGrowing,
                scaleModifier: this.ShowingGrowingModifer
                ));
            yield return new WaitForSeconds(this.ShowTime);
            yield return this.StartCoroutine(this.MoveTo(
                obj: card,
                to: this.dropPoint,
                finalSize: this.EndDropGrowing,
                scaleModifier: this.EndDropGrowingModifer
                ));
            Destroy(card.gameObject);

            yield return new WaitForEndOfFrame();
        }
        this.DropCardAnimationEnabled = false;
    }

    /// <summary>
    /// ������� ����������� �����.
    /// </summary>
    /// <param name="obj">��������� �����.</param>
    /// <param name="to">��������� ����������.</param>
    /// <param name="finalSize">�������� ������.</param>
    /// <param name="scaleModifier">����������� ��������� ����� � �������� ���������� ��������.</param>
    /// <returns></returns>
    IEnumerator MoveTo(
        Transform obj,
        Transform to,
        Vector3 finalSize,
        AnimationCurve scaleModifier
        )
    {
        const float epsilon = 3;
        Vector2 speed = Vector2.zero;
        float startDistance = Vector2.Distance(obj.position, to.position);
        Vector3 startScale = obj.localScale;
        while (Vector2.Distance(obj.position, to.position) >= epsilon)
        {
            float time = Vector2.Distance(
                obj.position, 
                to.position)
                / startDistance;

            obj.localScale = Vector3.Lerp(
                startScale,
                finalSize, 
                scaleModifier.Evaluate(time));

            obj.position = Vector2.SmoothDamp(
                current: obj.position, 
                target: to.position, 
                currentVelocity: ref speed, 
                smoothTime: this.CardMovementSpeed);

            yield return new WaitForEndOfFrame();
        }
    }
}
