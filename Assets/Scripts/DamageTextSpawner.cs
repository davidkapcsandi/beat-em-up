using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageTextSpawner : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public Canvas canvas;
    public Camera mainCamera;

    public void ShowDamage(int damageMeter, Vector3 worldPosition)
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        GameObject instance = Instantiate(damageTextPrefab, canvas.transform);
        instance.GetComponent<RectTransform>().position = screenPosition;
        instance.GetComponent<DamageText>().SetText(damageMeter);
    }
}