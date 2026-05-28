using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private float shotPower = 10f;

    [Header("Monst Speed Settings")]
    public float stopThreshold = 1.5f;
    [SerializeField] private float bounceDamping = 0.9f;

    private Rigidbody2D rb;
    private Camera cam;
    private float radius;

    public int comboCount = 0;
    public int maxCombo = 0;
    public UnityEngine.UI.Text comboText; // Text をインスペクタでセット

    [Header("Shot Count Settings")]
    public int shotCount = 0;
    public int shotLimit = 3; // 3回で爆弾生成
    public GameObject bombPrefab; // 爆弾プレハブをセット

    // ★★★ 追加：HP 関連 ★★★
    [Header("Player HP Settings")]
    public int maxHP = 5;
    public int currentHP;
    public UnityEngine.UI.Text hpText; // HP 表示用（任意）

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        rb.gravityScale = 0;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = GetComponent<CircleCollider2D>();
        radius = col.radius * transform.localScale.x;

        // ★★★ 追加：HP 初期化 ★★★
        currentHP = maxHP;
        UpdateHPText();
    }

    public void SetPositionByCanvas(Canvas canvas, Vector2 canvasPos)
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            canvasPos,
            canvas.worldCamera,
            out worldPos
        );

        transform.position = worldPos;
    }

    public void Shoot(Vector2 direction)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * shotPower, ForceMode2D.Impulse);

        // ★ ショット回数カウント
        shotCount++;

        // ★ 3回目で爆弾生成
        if (shotCount >= shotLimit)
        {
            SpawnBomb();

            // ★★★ 確実にリセット ★★★
            shotCount = 0;
        }
    }


    void FixedUpdate()
    {
        StopIfSlow();
        ClampAndBounce();
    }

    void StopIfSlow()
    {
        if (rb.linearVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector2.zero;

            // ★★★ 追加：止まったらコンボリセット ★★★
            if (comboCount != 0)
            {
                comboCount = 0;
                UpdateComboText();
            }
        }
    }

    void ClampAndBounce()
    {
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0, 0, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1, 1, z));

        Vector3 pos = transform.position;
        Vector2 v = rb.linearVelocity;
        bool bounced = false;

        if (pos.x - radius < min.x)
        {
            pos.x = min.x + radius;
            v.x *= -1;
            bounced = true;
        }
        else if (pos.x + radius > max.x)
        {
            pos.x = max.x - radius;
            v.x *= -1;
            bounced = true;
        }

        if (pos.y - radius < min.y)
        {
            pos.y = min.y + radius;
            v.y *= -1;
            bounced = true;
        }
        else if (pos.y + radius > max.y)
        {
            pos.y = max.y - radius;
            v.y *= -1;
            bounced = true;
        }

        if (bounced)
        {
            v *= bounceDamping;
            rb.linearVelocity = v;
            transform.position = pos;
        }
    }

    public bool IsStopped()
    {
        return rb.linearVelocity.magnitude < stopThreshold;
    }

    // ★★★ 完全統合版 OnCollisionEnter2D（何も消さずにまとめた）★★★
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            comboCount++;

            if (comboCount > maxCombo)
            {
                maxCombo = comboCount;
            }

            UpdateComboText();
            BounceFromEnemy(collision);
            return;
        }

        if (collision.collider.CompareTag("Block"))
        {
            BounceFromBlock(collision);
            return;
        }

        if (collision.collider.CompareTag("Bomb"))
        {
            TakeDamage(1); // ダメージ
            Destroy(collision.gameObject); // ★ 追加：爆弾を消す
            return;
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Vector2 normal = (transform.position - other.transform.position).normalized;
            ReflectVelocity(normal);
        }
    }

    void BounceFromEnemy(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        ReflectVelocity(normal);
    }

    void BounceFromBlock(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        ReflectVelocity(normal);
    }

    void ReflectVelocity(Vector2 normal)
    {
        Vector2 v = rb.linearVelocity;

        Vector2 reflected = Vector2.Reflect(v, normal);

        reflected *= bounceDamping;

        rb.linearVelocity = reflected;
    }

    void UpdateComboText()
    {
        if (comboText != null)
        {
            comboText.text = "Combo: " + comboCount;
        }
    }

    // ★★★ 追加：HP 表示更新 ★★★
    void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHP;
        }
    }

    // ★★★ 追加：ダメージ処理 ★★★
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
            currentHP = 0;

        UpdateHPText();

        if (currentHP == 0)
        {
            Debug.Log("Player Dead");

            PlayerPrefs.SetInt("MaxCombo", maxCombo);

            SceneManager.LoadScene("GameOver");
        }
    }

    void SpawnBomb()
    {
        // 画面内ランダム位置に生成
        float z = Mathf.Abs(cam.transform.position.z);

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(0.1f, 0.1f, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(0.9f, 0.9f, z));

        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);

        Vector3 pos = new Vector3(x, y, 0);

        Instantiate(bombPrefab, pos, Quaternion.identity);
    }
}
