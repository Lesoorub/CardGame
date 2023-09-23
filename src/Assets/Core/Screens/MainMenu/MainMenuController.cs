using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string NextScene = "";
    public void _Play()
    {
        SceneManager.LoadScene(this.NextScene);
    }
}
