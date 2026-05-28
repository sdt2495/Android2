using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 10;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            TakeDamage(1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"敵に {damage} ダメージ！ 残りHP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敵を倒した！");

        // ★ 追加：クリア管理に通知
        if (EnemyCountManager.Instance != null)
        {
            EnemyCountManager.Instance.EnemyDead();
        }

        Destroy(gameObject);
    }
}