using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    public Animator bossAnimator;
    public int health = 10;
    private bool isDead = false; // Prevent multiple deaths

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Ignore damage if already dead

        health -= damage;
        

        if (damage > 0) // If still alive, trigger "GetHit"
        {
            health -= damage;
            Debug.Log("Boss health: " + health);
           bossAnimator.SetTrigger("GetHit");

        
        if (health <= 0)
            {
            Die();
            }
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple death calls

        isDead = true; // Mark as dead
        Debug.Log("Boss died!");
        bossAnimator.SetTrigger("Death"); // Trigger death animation

        // Delay destruction to allow death animation to play
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        // Wait for death animation to play
        yield return new WaitForSeconds(2.5f); // Adjust this based on animation length

        Destroy(gameObject); // Destroy the boss after the death animation
    }
}