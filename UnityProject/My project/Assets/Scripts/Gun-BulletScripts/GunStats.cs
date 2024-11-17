using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    public GameObject bullet;
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int ammoCur, ammoMax;
 

   
    public AudioClip[] shootSound;
    public float shootVol;

    public void SetCurrAmmo(int amount)
    {
        ammoCur = amount;
    }

    public int GetCurrAmmo()
    {
        return ammoCur;
    }
}
