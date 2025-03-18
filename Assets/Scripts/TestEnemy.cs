using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    public int health = 100;
    public GameObject floatingTextPrefab; // Assign in Inspector!

    private void Start()
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("🚨 FloatingDamageText prefab is STILL missing from " + gameObject.name);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (floatingTextPrefab != null)
        {
            GameObject damageTextInstance = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText floatingText = damageTextInstance.GetComponent<FloatingText>();

            if (floatingText != null)
            {
                floatingText.SetText(damage);
            }
            else
            {
                Debug.LogError("FloatingText component is missing on prefab!");
            }
        }
        else
        {
            Debug.LogError("FloatingDamageText prefab is NOT assigned in the Inspector!");
        }

        if (health <= 0)
        {
            // Die();
            Destroy(gameObject);
        }
    }
}

    /* void Die()
    {
        Destroy(gameObject);
    }
}
*/