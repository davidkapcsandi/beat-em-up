using UnityEngine;
using System.Collections;

public class TestEnemy : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColour = Color.red;
    public Color defaultColour;
    public float health = 10;
    private DamageTextSpawner damageTextSpawner;

    private void Start()
    {
        spriteRenderer = transform.root.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColour = spriteRenderer.color;
        }

        // Find DamageTextSpawner in the scene
        damageTextSpawner = FindObjectOfType<DamageTextSpawner>();
        if (damageTextSpawner == null)
        {
            Debug.LogError("DamageTextSpawner not found in the scene!");
        }
    }

    public void TakeDamage(float damageMeter)
    {
        health -= damageMeter;
        Debug.Log("Enemy health: " + health);

        spriteRenderer.color = hurtColour;
        StartCoroutine(ResetColor());

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColour;
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(transform.root.gameObject);
    }
}