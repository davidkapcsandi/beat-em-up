using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float lifetime = 1f;
    public float floatSpeed = 1f;
    public TextMeshProUGUI text;
    private Vector3 randomOffset;

    public void SetText(int damageMeter)
    {
        text.text = damageMeter.ToString();
        Destroy(gameObject, lifetime);

        // Add a slight random offset to avoid overlapping texts
        randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.2f, 0.5f), 0);
        transform.position += randomOffset;
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}