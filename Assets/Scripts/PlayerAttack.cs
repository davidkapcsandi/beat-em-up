using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f;  // Time between attacks (in seconds)
    private float lastAttackTime = 0f; // Time of last attack
    public float damageMeter;

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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f); // Adjust radius if needed
        foreach (var hitCollider in hitColliders)
        {
            // Check if the collider is an Enemy
            if (hitCollider.CompareTag("Enemy"))
            {
                TestEnemy enemy = hitCollider.GetComponent<TestEnemy>(); 
                if (enemy != null)
                {
                    enemy.TakeDamage(damageMeter);
                    Debug.Log ("Return damage");
                }
            }
            else if (hitCollider.CompareTag("Boss"))
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