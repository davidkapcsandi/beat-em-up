using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Transform target; // The enemy to follow
    public Vector3 offset = new Vector3(0, 2, 0); // Adjust based on enemy size

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(Camera.main.transform);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }
}