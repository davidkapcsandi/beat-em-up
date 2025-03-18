using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float destroyTime = 1f;
    public TextMeshPro damageText; // Make sure this is assigned in the Inspector!

    public void SetText(int damage)
    {
        if (damageText == null)
        {
            Debug.LogError("🚨 FloatingText: No TextMeshProUGUI assigned! Check prefab.");
            return;
        }

        damageText.text = damage.ToString(); // Force update the text
        damageText.ForceMeshUpdate(); // Make sure TMP updates immediately

        Debug.Log("✅ FloatingText: Damage text set to " + damage);
    }

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }
}