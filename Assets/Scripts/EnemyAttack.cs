
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private float attackRadius = 2f;

    private void Update()
    {
        // Check if cooldown has passed and the player is within attack range
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Find the player within attack radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (var hitCollider in hitColliders)
            {
                // Check if the collider is on the Player layer (or Player tag)
                if (hitCollider.CompareTag("Player"))
                {
                    Attack(); // Call the Attack function to deal damage
                    lastAttackTime = Time.time; // Update the last attack time
                    break; // Exit loop after attacking the player
                }
            }
        }

    }
    private void Attack()
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            
            if (hitCollider.CompareTag("Player"))
                Debug.Log("Enemy Attack");
            {
                
                PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.PlayerDamage(1);  // Deal 1 damage
                }
            }
        }
    }
}