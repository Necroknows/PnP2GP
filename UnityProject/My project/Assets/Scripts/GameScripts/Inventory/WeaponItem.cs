using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]

public class WeaponItem : Item
{
    //general
    public GameObject bullet;
    
    //shoot stats
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    
    //ammo
    public int ammoCur, ammoMax;

    //audio
    public AudioClip[] shootSound;
    public float shootVol;

    public void Awake()
    {
        itemType = ItemType.Weapon;   //set type automatically
    }

}//END