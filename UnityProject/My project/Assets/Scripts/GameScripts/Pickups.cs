using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickUpType { Health, Ammo, Fuel, Gun };
    public PickUpType pickupType;

    public int healthPickupAmount = 15;
    public int ammoPickupAmount = 30;
    public int fuelPickupAmount = 25;
    public GunStats gun;

    // Animation
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;
    private Vector3 startPos;

    // Reference to PlayerController
    private PlayerController playerController;

    void Start()
    {
        startPos = transform.position;
        playerController = GameObject.FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Hovering animation logic
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * (hoverRange / 2);
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            switch (pickupType)
            {
                case PickUpType.Health:
                    if (player.getHP() < player.getHPOrig())
                    {
                        player.setHP(healthPickupAmount);
                        UIManager.Instance.UpdatePlayerHealthBar(player.getHP());
                        Destroy(gameObject);
                    }
                    break;

                case PickUpType.Ammo:
                    if (player.GetAmmo() < player.getAmmoMax())
                    {
                        player.setAmmo(ammoPickupAmount);
                        Destroy(gameObject);
                    }
                    break;

                case PickUpType.Fuel:
                    if (player.GetFuel() < player.GetFuelMax())
                    {
                        player.SetFuel(fuelPickupAmount);
                        UIManager.Instance.UpdatePlayerFuelBar(player.GetFuel());
                        Destroy(gameObject);
                    }
                    break;

                case PickUpType.Gun:
                    GameManager.instance.playerScript.getGunStats(gun);
                    gun.ammoCur = gun.ammoMax;
                    Destroy(gameObject);
                    break;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        UIManager.Instance.CloseMenu(); // Close any related menu or clear help text
    }
}
