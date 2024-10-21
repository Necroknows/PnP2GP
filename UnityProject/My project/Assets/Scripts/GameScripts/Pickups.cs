using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickUpType { Health, Ammo, Fuel,Gun };
    public PickUpType pickupType;

    public int healthPickupAmount = 15;
    public int ammoPickupAmount = 30;
    public int fuelPickupAmount = 25;

    //animation
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;
    private Vector3 startPos;

    public playerController playerCont;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        playerCont=FindAnyObjectByType<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * (hoverRange / 2);
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    //check collider/add health
    void OnTriggerEnter(Collider other)
    {

        playerController player = other.GetComponent<playerController>();

        //if pickup is health
        if (player != null)
        {
            switch (pickupType)
            {
                case PickUpType.Health:
                    {
                        if (playerCont.getHP() < playerCont.getHPOrig())
                        {
                            player.setHP(healthPickupAmount);
                            UIManager.Instance.UpdatePlayerHealthBar(healthPickupAmount);
                            Destroy(gameObject);
                        }
                        else
                        {

                        }
                        break;
                    }
                case PickUpType.Ammo:
                    if(playerCont.getAmmo()<playerCont.getAmmoMax())
                    {
                    player.setAmmo(ammoPickupAmount);
                        Destroy(gameObject);    
                    }
                    break;
                case PickUpType.Fuel:
                    if(playerCont.getFuel()<playerCont.getFuelMax())
                    {
                        UIManager.Instance.UpdatePlayerFuelBar(ammoPickupAmount);
                        player.setFuel(fuelPickupAmount);
                        Destroy(gameObject);
                    }

                    break;

              
            }

        }
    }
}