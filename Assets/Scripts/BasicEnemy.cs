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

    private int currIndex = -1;
    private BotState botState = BotState.Patrol;

    private void Start()
    {
        StateChanged?.Invoke(botState);
    }
    private void Update()
    {
        // 1. does my agent have destination
        // 2. has my agent reached a destination?


        //
        if (botState == BotState.Patrol)
        {
            if (!agent.hasPath || agent.remainingDistance <= 0.5f)
            {
                currIndex++;

                if (currIndex >= waypoints.Length)
                    currIndex = 0;

//                agent.SetDestination(waypoints[currIndex].position);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            botState = BotState.Chase;
            agent.SetDestination(other.transform.position);
            StateChanged?.Invoke(botState);

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            botState = BotState.Patrol;
            agent.ResetPath();
            StateChanged?.Invoke(botState);
        }
    }

}
