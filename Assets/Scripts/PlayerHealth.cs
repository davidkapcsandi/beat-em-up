using UnityEngine.UI;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColour = Color.red;
    public Color defaultColour;
    public Slider healthBar;
    public float maxHealth = 100;
    private float health;
    private void Start()
    {
        health = maxHealth;
        spriteRenderer = transform.root.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColour = spriteRenderer.color;
        }
    }
    public void PlayerDamage(int damage)
    {
        health -= damage;
        healthBar.value = health / maxHealth;
        Debug.Log("Player Health: " + health);
        spriteRenderer.color = hurtColour;
        StartCoroutine(ResetColor());

        if (health <= 0)
        {
            Die();
        }
        else
        {
            spriteRenderer.color = hurtColour;
        }
    }
    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.1f);  // Wait for 0.1 seconds (adjustable)

        if (spriteRenderer != null)
        {
            // Reset the color back to the original color
            spriteRenderer.color = defaultColour;
        }
    }

    private void Die()
    {
        Debug.Log("You died!");
        // Destroy the main (root) object, not just this object
        Destroy(transform.root.gameObject);

    }


}
