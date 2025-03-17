using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string weaponName;  // The name of the weapon
    public GameObject weaponPrefab; // The weapon prefab to be picked up
    public Transform weaponHoldPoint; // Reference to the weapon's hold point (player's hand)

    private bool isPlayerInRange = false;  // To check if the player is within range to pick up the weapon

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))  // Press 'E' to pick up the weapon
        {
            PickUpWeapon();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // When the player is close enough
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // When the player leaves the pickup range
        {
            isPlayerInRange = false;
        }
    }

    void PickUpWeapon()
    {
        // Instantiate the weapon at the weapon hold point (the player's hand)
        GameObject weapon = Instantiate(weaponPrefab, weaponHoldPoint.position, weaponHoldPoint.rotation);
        
        weapon.transform.SetParent(weaponHoldPoint);
        
       
        weapon.transform.localPosition = Vector3.zero;  
        weapon.transform.localRotation = Quaternion.identity;  

        Collider weaponCollider = weapon.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        Destroy(gameObject);  
    }
}