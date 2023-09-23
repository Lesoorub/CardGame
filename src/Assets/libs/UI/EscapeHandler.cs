using UnityEngine;
using UnityEngine.Events;

public class EscapeHandler : MonoBehaviour
{
    public UnityEvent OnEscape = new UnityEvent();
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscape?.Invoke();
        }
    }
}
