using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;



public enum BotState
{
    Patrol,
    Chase,
}

public class BasicEnemy : MonoBehaviour
{
    public UnityEvent<BotState> StateChanged;
    public NavMeshAgent agent;
    public Transform[] waypoints;
    private Transform player; 
    private int currIndex = -1;
    private BotState botState = BotState.Patrol;
    public Animator enemyAnimator;
    private Rigidbody rb;

    private void Start()
    {
        StateChanged?.Invoke(botState);
        SetupEnemy();
    }

    void SetupEnemy ()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        enemyAnimator = GetComponent<Animator>();

    }



    private void Update()
    {


        if (botState == BotState.Patrol)
        {
            Patrol();
        }
        else if (botState == BotState.Chase && player != null)
        {
            ChasePlayer();
        }
    }

    private void Patrol()
    {
        if (!agent.hasPath || agent.remainingDistance <= 0.3f)
        {
            currIndex++;
            if (currIndex >= waypoints.Length)
                currIndex = 0;
        }
    }

    public void ChasePlayer()
    {
        while (player != null)
        {
            agent.SetDestination(player.position);  // Always update the target position
            enemyAnimator.SetBool("Walk", true);
        }
        
        
        {
            enemyAnimator.SetBool("Walk", false);
            enemyAnimator.SetBool("Idle" ,true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            botState = BotState.Chase;
            StateChanged?.Invoke(botState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            botState = BotState.Patrol;
            agent.ResetPath();
            StateChanged?.Invoke(botState);
            player = null;
        }
    }
  
   
}