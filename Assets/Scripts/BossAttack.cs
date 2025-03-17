using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public float attackCooldown = 3f;
    private float lastAttackTime = 0f;
    public Animator bossAnimator;
    private Transform player;
    private bool playerInRange = false;
    private bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
            Debug.Log("Player entered range.");
            
            if (!isAttacking)
            {
                StartCoroutine(AttackRoutine());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left range.");
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        while (playerInRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                BossAttacked();
                lastAttackTime = Time.time;
                
                yield return new WaitForSeconds(attackCooldown); // Wait before next attack
            }
            yield return null;
        }

        isAttacking = false;
    }

    private void BossAttacked()
    {
        bossAnimator.SetTrigger("Attack");

        StartCoroutine(ApplyDamageAfterDelay(0.5f));

        // ✅ **Reset Trigger After Short Delay**
        StartCoroutine(ResetAttackTrigger());
    }

    private IEnumerator ApplyDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null && playerInRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerDamage(5);
            }
        }
    }

    private IEnumerator ResetAttackTrigger()
    {
        yield return new WaitForSeconds(0.1f); // Small delay before resetting
        bossAnimator.ResetTrigger("Attack"); // ✅ Prevents animation getting stuck
    }
}