using System.Collections;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColor = Color.red;
    private Color originalColor;

    public bool isStunned = false;


    private void Start()
    {

        spriteRenderer = transform.root.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }


    public void ColorChange()
    {
        if (spriteRenderer != null)
        {

            spriteRenderer.color = hurtColor;


            StartCoroutine(ResetColor());

            isStunned = true;
        }
    }


    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;

        }
        isStunned = false;
    }
   
}