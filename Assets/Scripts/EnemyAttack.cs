
using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    public Animator enemyAnimator;

    private float attackRadius = 2f;

    private void Update()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    Attack();
                    enemyAnimator.SetBool("LightPunch", true);
                    lastAttackTime = Time.time;
                    break;
                }
            }
        }
    }

    private void Attack()
    {
        bool attacked = false; // Ensures attack animation is only reset once

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("Enemy Attack");

                PlayerHealth player = hitCollider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.PlayerDamage(1); // Deal 1 damage
                }

                attacked = true; // Track if attack occurred
            }
        }

        if (attacked)
        {
            StartCoroutine(ResetAttackAnimation());
        }
    }

    private IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(2f); // Adjust timing based on animation length
        enemyAnimator.SetBool("LightPunch", false);
    }
}