using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageTaken : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color hurtColor = Color.red;
    private Color originalColor;


    private void Start()
    {

        spriteRenderer = transform.root.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }


    public void ColorChangeBoss()
    {
        if (spriteRenderer != null)
        {

            spriteRenderer.color = hurtColor;


            StartCoroutine(ResetColorBoss());


        }
    }


    private IEnumerator ResetColorBoss()
    {
        yield return new WaitForSeconds(0.1f);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;

        }

    }
}
