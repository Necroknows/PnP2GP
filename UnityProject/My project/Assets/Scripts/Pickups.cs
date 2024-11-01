using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour, IInteractive
{
    public enum PickUpType { Health, Ammo, Fuel, Herb, Ingredient, Potion, Weapon };
    public PickUpType pickupType;

    public int healthPickupAmount = 15;
    public int ammoPickupAmount = 30;
    public int fuelPickupAmount = 25;
    public WeaponItem gun;
    public HerbItem herb;
    public PotionItem potion;

    // Animation
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;
    private Vector3 startPos;

    // Reference to PlayerController
    private PlayerController playerController;

    //Reference to Inventory
    private Inventory inventory;

    void Awake()
    {
        startPos = transform.position;
        playerController = GameObject.FindObjectOfType<PlayerController>();
        inventory = Inventory.instance;
    }

    public void Interact()
    {
        {
            if (playerController != null)
            {
                switch (pickupType)
                {
                    case PickUpType.Herb:
                        
                            HerbItem newHerb = ScriptableObject.CreateInstance<HerbItem>();
                            inventory.AddItem(newHerb);
                            Destroy(gameObject);
                        
                        break;

                    case PickUpType.Weapon:
                         
                        WeaponItem newGun = ScriptableObject.CreateInstance<WeaponItem>(); //instantiate new gun

                            //Copy properties from existing gun
                            newGun.bullet = gun.bullet;
                            newGun.shootDamage = gun.shootDamage;
                            newGun.shootRate = gun.shootRate;
                            newGun.shootDist = gun.shootDist;
                            newGun.ammoCur = gun.ammoCur;
                            newGun.ammoMax = gun.ammoMax;
                            newGun.shootSound = gun.shootSound;
                            newGun.shootVol = gun.shootVol;

                            //add to inventory
                            inventory.AddItem(newGun);
                            Destroy(gameObject);
                        
                        break;
                }
            }
        }
    }

    void Update()
    {
        // Hovering animation logic
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * (hoverRange / 2);
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    

}//END
