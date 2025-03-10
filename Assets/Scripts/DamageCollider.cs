
using System.Collections;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public string newTag = "PlayerAttack";
    public int dmg = 1;

    private string originalTag; 

    void Start()
    {
        // Store the current tag of the GameObject when the script starts
        originalTag = gameObject.tag;
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(ChangeTagTemporarily());
        }
    }
    private IEnumerator ChangeTagTemporarily()
    {

        string currentTag = gameObject.tag;

   
        gameObject.tag = newTag;
        Debug.Log("Tag changed to: " + newTag);

     
        yield return new WaitForSeconds(0.5f);

        gameObject.tag = currentTag;
        Debug.Log("Tag reverted to: " + currentTag);
    }
}