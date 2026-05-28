using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCountManager : MonoBehaviour
{
    public static EnemyCountManager Instance;

    public int enemyCount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void EnemyDead()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            // š Å‘åƒRƒ“ƒ{•Û‘¶
            PlayerPrefs.SetInt("MaxCombo", PlayerController.Instance.maxCombo);

            // š Clear‰æ–Ê‚Ö
            SceneManager.LoadScene("Clear");
        }
    }
}