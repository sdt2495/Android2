using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonManager : MonoBehaviour
{
    [Header("Scene Name")]
    public string gameSceneName = "Game";
    public string titleSceneName = "Title";

    // リトライ
    public void Retry()
    {
        SceneManager.LoadScene("Main");
    }

    // タイトルへ
    public void GoTitle()
    {
        SceneManager.LoadScene("Title");
    }
}