using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickUpType { Health, Ammo };
    public PickUpType pickupType;

    public int healthPickupAmount = 15;
    public int ammoPickupAmount = 30;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                    player.setHP(healthPickupAmount);
                    break;
                case PickUpType.Ammo:
                    player.setAmmo(ammoPickupAmount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}