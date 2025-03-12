using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public Animator animator; // Reference to the boss's Animator
    public int attackDamage = 20; // Damage dealt to the player
    public float attackCooldown = 2f; // Cooldown between attacks
    private GameObject targetPlayer; // Store the player reference
    private float nextAttackTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            targetPlayer = other.gameObject;
            animator.SetTrigger("Attack"); // Start attack animation
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // This function is called by an animation event at the hit frame
    public void DealDamage()
    {
        if (targetPlayer != null)
        {
            PlayerHealth playerHealth = targetPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerDamage(attackDamage);
            }
        }
    }
     private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPlayer = null; // Stop attacking when player leaves the collider
        }
    }
}