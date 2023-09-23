using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Обработчик игрока на карте
/// </summary>
public class MapPlayer : MonoBehaviour
{

    /// <summary>
    /// Текущий узел
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
    /// Текущий узел
    /// </summary>
    [SerializeField]
    private Node currentNode;

    /// <summary>
    /// Скорость передвижения по карте.
    /// Умножается на текущую ширину экрана в пикселях, т.е является относительной.
    /// </summary>
    public float MovementSpeed = 1;

    /// <summary>
    /// Флаг определяющий запущено ли движение в данный момент времени
    /// </summary>
    private bool isMoving = false;

    /// <summary>
    /// Событие входа на узел. Вызывается при появлении игрока в мире
    /// </summary>
    public UnityEvent<MapPlayer, Node> OnNodeEnter = new UnityEvent<MapPlayer, Node>();

    /// <summary>
    /// Событие передвижения. Вызывается каждый раз при движении игрока
    /// </summary>
    public UnityEvent<MapPlayer> OnMovement = new UnityEvent<MapPlayer>();

    /// <summary>
    /// Следующие узлы пути игрока
    /// </summary>
    public Queue<Node> nextPath { get; } = new Queue<Node>();

    void Start()
    {
        if (this.currentNode != null)
            this.transform.position = this.currentNode.transform.position;
        this.OnNodeEnter?.Invoke(this, this.currentNode);
    }

    /// <summary>
    /// Подолжить движение, если это возможно и необходимо
    /// </summary>
    public void ResumeMoving()
    {
        if (!this.isMoving)
            this.StartCoroutine(this.Movement());
    }

    /// <summary>
    /// Направить игрока по пути
    /// </summary>
    /// <param name="path">Путь</param>
    public void GoToNode(IEnumerable<Node> path)
    {
        this.nextPath.Clear();
        foreach (var node in path)
            this.nextPath.Enqueue(node);

        this.ResumeMoving();
    }

    /// <summary>
    /// Асинхронный (на сколько это возможно) метод движения игрока
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
    /// Переназначает игроку путь, ведущий к предидущему узлу
    /// </summary>
    public void GoToPreviousNode()
    {
        this.nextPath.Clear();
        this.nextPath.Enqueue(this.CurrentNode);
        this.ResumeMoving();
    }

    /// <summary>
    /// Останавливает игрока
    /// </summary>
    public Node[] StopPlayerMovement()
    {
        var path = this.nextPath.ToArray();
        this.nextPath.Clear();
        return path;
    }
}
