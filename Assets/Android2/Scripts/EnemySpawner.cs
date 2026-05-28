using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 5;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // ★ 重ならないランダム座標を取得
            Vector3 pos = GetRandomNonOverlapPosition(0.6f);
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }

    // ★ 元の関数は残す（使わないだけ）
    Vector3 GetRandomPositionInScreen()
    {
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, z));

        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);

        return new Vector3(x, y, 0);
    }

    // ★★★ 追加：重なりチェック付きランダム座標 ★★★
    Vector3 GetRandomNonOverlapPosition(float radius)
    {
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, z));

        for (int i = 0; i < 50; i++) // 最大50回試す
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            Vector3 pos = new Vector3(x, y, 0);

            // ★ 半径 radius の円で重なりチェック
            Collider2D hit = Physics2D.OverlapCircle(pos, radius);

            if (hit == null)
            {
                return pos; // 重なってないのでOK
            }
        }

        // 50回失敗したら普通のランダム位置を返す
        return GetRandomPositionInScreen();
    }
}
