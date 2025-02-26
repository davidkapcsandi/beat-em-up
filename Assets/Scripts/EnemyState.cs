
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    private Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    public void StateChanged(BotState newBotState)
    {
        switch (newBotState)
        {
            case BotState.Patrol:
                mat.color = Color.yellow;
                break;
            case BotState.Chase:
                mat.color = Color.red;
                break;

        }
    }
}
