using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    public GameObject bombPrefab;
    public int spawnCount = 2;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        SpawnBombs();
    }

    void SpawnBombs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = GetRandomNonOverlapPosition(0.6f);
            Instantiate(bombPrefab, pos, Quaternion.identity);
        }
    }

    Vector3 GetRandomNonOverlapPosition(float radius)
    {
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, z));

        for (int i = 0; i < 50; i++)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            Vector3 pos = new Vector3(x, y, 0);

            Collider2D hit = Physics2D.OverlapCircle(pos, radius);

            if (hit == null)
                return pos;
        }

        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            0
        );
    }
}
