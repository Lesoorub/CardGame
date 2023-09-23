using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// "�������" �������. �������� �������� ������� <see cref="Slider"/> � �������� ���������.
/// </summary>
public class LazySliderFollower : MonoBehaviour
{
    /// <summary>
    /// �������� �����������.
    /// </summary>
    public float Delay = 1;

    /// <summary>
    /// �������� "�������" ��������.
    /// </summary>
    [Range(0, 10)]
    public float FallSpeed = 1;

    /// <summary>
    /// ������� �������� �������� ����� ������������.
    /// </summary>
    public Slider target;

    /// <summary>
    /// ������� � ������� ����� ������������ �������� <see cref="target"/>.
    /// </summary>
    public Slider self;


    float FallTime;
    bool AnimationEnabled = false;

    private void OnEnable()
    {
        this.target?.onValueChanged.AddListener(this.OnTargetValueChanged);
        if (this.self != null && this.target != null)
        {
            this.self.value = this.target.value;
            this.OnTargetValueChanged(this.target.value);
        }
    }

    private void OnDisable()
    {
        this.target?.onValueChanged.RemoveListener(this.OnTargetValueChanged);
    }

    /// <summary>
    /// ���������� ������� ��������� �������� �������� ��������.
    /// </summary>
    /// <param name="newValue"></param>
    public void OnTargetValueChanged(float newValue)
    {
        this.FallTime = Time.time + this.Delay;
        if (!this.AnimationEnabled)
            this.StartCoroutine(this.Animation());
    }

    /// <summary>
    /// ������� ��������.
    /// </summary>
    /// <returns></returns>
    IEnumerator Animation()
    {
        this.AnimationEnabled = true;
        while (this.self.value != this.target.value)
        {
            while (this.FallTime > Time.time)
                yield return new WaitForEndOfFrame();

            this.self.value = Mathf.Lerp(this.self.value, this.target.value, this.FallSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        this.AnimationEnabled = false;
    }
}
