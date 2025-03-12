using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 3f; // 3-second delay between attacks
    private float lastAttackTime = 0f;
    public Animator enemyAnimator;
    public DamageTaken damageTakenScript; // Reference to the DamageTaken script

    private float attackRadius = 2f;

    private void Update()
    {
        // Check if the enemy is stunned. If yes, don't attack.
        if (damageTakenScript.isStunned)
        {
            return; // Exit the Update method early if the enemy is stunned
        }

        // If enough time has passed since the last attack, check if the player is in range
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    Debug.Log("Enemy sees the player and is ready to attack!");
                    Attack();
                    lastAttackTime = Time.time; // Update last attack time
                    break;
                }
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Enemy is attacking!");

        // Trigger the LightPunch animation (set the boolean to true)
        enemyAnimator.SetBool("LightPunch", true);

        // Check if the player is in range to apply damage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.PlayerDamage(1); // Apply damage to the player
                }
            }
        }

        // Start a coroutine to reset the animation after it finishes
        StartCoroutine(ResetAttackAnimation());
    }

    private IEnumerator ResetAttackAnimation()
    {
        // Wait for the duration of the animation (you can adjust this time to fit your animation)
        yield return new WaitForSeconds(1f); // Adjust this time based on your animation length

        // Reset the boolean to false to stop the attack animation
        enemyAnimator.SetBool("LightPunch", false);

        Debug.Log("Attack animation reset!");
    }
}