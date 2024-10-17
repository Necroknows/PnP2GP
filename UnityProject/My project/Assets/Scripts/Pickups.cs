using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickUpType { Health, Ammo, Fuel };
    public PickUpType pickupType;

    public int healthPickupAmount = 15;
    public int ammoPickupAmount = 30;
    public int fuelPickupAmount = 25;

    //animation
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
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
                    
                    player.setHP(healthPickupAmount);
                    UIManager.Instance.UPdatePlayerHealthBar(healthPickupAmount);
                    break;
                case PickUpType.Ammo:
                    player.setAmmo(ammoPickupAmount);
                    break;
                case PickUpType.Fuel:
                    player.setFuel(fuelPickupAmount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}