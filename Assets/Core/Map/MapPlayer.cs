using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���������� ������ �� �����
/// </summary>
public class MapPlayer : MonoBehaviour
{

    /// <summary>
    /// ������� ����
    /// </summary>
    public Node CurrentNode
    {
        get => this.currentNode;
        set
        {
            this.currentNode = value;
            this.transform.position = value.transform.position;
            this.OnNodeEnter?.Invoke(this, value);
        }
    }

    /// <summary>
    /// ������� ����
    /// </summary>
    [SerializeField]
    private Node currentNode;

    /// <summary>
    /// �������� ������������ �� �����.
    /// ���������� �� ������� ������ ������ � ��������, �.� �������� �������������.
    /// </summary>
    public float MovementSpeed = 1;

    /// <summary>
    /// ���� ������������ �������� �� �������� � ������ ������ �������
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// ������� ����� �� ����. ���������� ��� ��������� ������ � ����
    /// </summary>
    public UnityEvent<MapPlayer, Node> OnNodeEnter = new UnityEvent<MapPlayer, Node>();

    /// <summary>
    /// ������� ������������. ���������� ������ ��� ��� �������� ������
    /// </summary>
    public UnityEvent<MapPlayer> OnMovement = new UnityEvent<MapPlayer>();

    /// <summary>
    /// ��������� ���� ���� ������
    /// </summary>
    public Queue<Node> nextPath { get; } = new Queue<Node>();

    void Start()
    {
        if (this.currentNode != null)
            this.transform.position = this.currentNode.transform.position;
        this.OnNodeEnter?.Invoke(this, this.currentNode);
    }

    /// <summary>
    /// ��������� ��������, ���� ��� �������� � ����������
    /// </summary>
    public void ResumeMoving()
    {
        if (!this.isMoving)
            this.StartCoroutine(this.Movement());
    }

    /// <summary>
    /// ��������� ������ �� ����
    /// </summary>
    /// <param name="path">����</param>
    public void GoToNode(IEnumerable<Node> path)
    {
        this.nextPath.Clear();
        foreach (var node in path)
            this.nextPath.Enqueue(node);

        this.ResumeMoving();
    }

    /// <summary>
    /// ����������� (�� ������� ��� ��������) ����� �������� ������
    /// </summary>
    /// <returns></returns>
    IEnumerator Movement()
    {
        const float Epsilon = 1e-3f;

        var speed = Screen.width * this.MovementSpeed;

        this.isMoving = true;

        while (this.nextPath.Count > 0)
        {
            var nextNode = this.nextPath.Peek();
            while (Vector3.Distance(this.transform.position, nextNode.transform.position) >= Epsilon)
            {
                this.transform.position = Vector3.MoveTowards(
                    this.transform.position,
                    nextNode.transform.position,
                    Time.deltaTime * speed);
                this.OnMovement?.Invoke(this);
                yield return new WaitWhile(() => this.nextPath.Count == 0);
                if (this.nextPath.Count > 0)
                    nextNode = this.nextPath.Peek();
                yield return new WaitForEndOfFrame();
            }
            this.CurrentNode = this.nextPath.Dequeue();
        }

        this.isMoving = false;
    }

    /// <summary>
    /// ������������� ������ ����, ������� � ����������� ����
    /// </summary>
    public void GoToPreviousNode()
    {
        this.nextPath.Clear();
        this.nextPath.Enqueue(this.CurrentNode);
        this.ResumeMoving();
    }

    /// <summary>
    /// ������������� ������
    /// </summary>
    public Node[] StopPlayerMovement()
    {
        var path = this.nextPath.ToArray();
        this.nextPath.Clear();
        return path;
    }
}
