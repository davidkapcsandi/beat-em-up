using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColour = Color.red;
    public Color defaultColour;
    public Animator bossAnimator;
    public int health = 10;
    private void Start()
    {
        spriteRenderer = transform.root.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColour = spriteRenderer.color;
        }

    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Boss health: " + health);
        spriteRenderer.color = hurtColour;
        StartCoroutine(ResetColor());
        bossAnimator.SetTrigger("GetHit");

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
        Debug.Log("Enemy died!");
        bossAnimator.SetTrigger("Death");
        // Destroy the main (root) object, not just this object
        

    }
    
}
