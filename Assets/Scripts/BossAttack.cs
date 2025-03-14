using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public float attackCooldown = 3f; // 3-second delay between attacks
    private float lastAttackTime = 0f;
    public Animator bossAnimator;
    public DamageTaken damageTakenScript; // Reference to the DamageTaken script

    private float attackRadius = 2f;

    private void Update()
    {

        // If enough time has passed since the last attack, check if the player is in range
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    Debug.Log("Enemy sees the player and is ready to attack!");
                    BossAttacked();
                    lastAttackTime = Time.time; // Update last attack time
                    break;
                }
            }
        }
    }

    public void BossAttacked()
    {
        Debug.Log("Enemy is attacking!");

        // Trigger the LightPunch animation (set the boolean to true)
        bossAnimator.SetTrigger("Attack");

        // Check if the player is in range to apply damage
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.PlayerDamage(5); // Apply damage to the player
                }
            }
        }

    }

}