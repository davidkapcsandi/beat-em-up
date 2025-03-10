using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColour = Color.red;
    public Color defaultColour;

    private float npcHealth;
    public float npcMaxHealth = 2;

    // Start is called before the first frame update
    void Start()
    {
        SetupHealth();
        
        //Set Default Colour
      
    }

    void SetupHealth()
    {
        npcHealth = npcMaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Takedamage(int dmg)
    {
        npcHealth -= dmg;

        if (npcHealth <= 0)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerAttack")
        {
            Debug.Log("Ouch!");
            defaultColour = spriteRenderer.color;
        }
        else if (npcHealth <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            spriteRenderer.color = Color.black;

        }
    }
    
}
