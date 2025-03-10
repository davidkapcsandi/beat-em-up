using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColour = Color.red;
    private Color defaultColour;

    private float npcHealth;
    public float npcMaxHealth = 2;

    // Start is called before the first frame update
    void Start()
    {
        SetupHealth();
        
        //Set Default Colour
        defaultColour = spriteRenderer.color;
    }

    void SetupHealth()
    {
        npcHealth = npcMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack")
        {
            Debug.Log("Ouch!");
            spriteRenderer.color = hurtColour;
            npcHealth --;
        }
        else
        {
            spriteRenderer.color = defaultColour;

        }
    }
}
