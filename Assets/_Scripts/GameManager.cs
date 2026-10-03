using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý trạng thái game và các chức năng hệ thống (Reload scene, pause,...)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Load lại Scene hiện tại
    /// </summary>
    public void ResetScene()
    {
        // Lấy scene đang chạy và load lại
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    public void Quit() => Application.Quit();
}
