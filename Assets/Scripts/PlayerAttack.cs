using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f;  // Time between attacks (in seconds)
    private float lastAttackTime = 0f; // Time of last attack
    private int damageMeter;

    private void Update()
    {
        // Detect R key press
        if (Input.GetKeyDown(KeyCode.R) && Time.time >= lastAttackTime + attackCooldown)
        {
            damageMeter = 1;
            Attack();
            lastAttackTime = Time.time;  // Update the last attack time
        }
        else if (Input.GetKeyDown(KeyCode.F) && Time.time >= lastAttackTime + attackCooldown)
        {
            damageMeter = 2;
            Attack();
            lastAttackTime = Time.time;

        }
    }

    private void Attack()
    {
        // Find all colliders in the trigger zone and deal damage to enemies
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // You can adjust the radius
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider is on the Enemy layer
            if (hitCollider.CompareTag("Enemy"))
            {
                // Call the TakeDamage function on the enemy
                TestEnemy enemy = hitCollider.GetComponent<TestEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageMeter);  // Deal 1 damage
                }
            }
            else if(hitCollider.CompareTag("Boss"))
                {
                BossHealth boss = hitCollider.GetComponent<BossHealth>();
                if (boss != null)
                {
                    boss.TakeDamage(damageMeter);
                }    

                 }
        }
    }
}
