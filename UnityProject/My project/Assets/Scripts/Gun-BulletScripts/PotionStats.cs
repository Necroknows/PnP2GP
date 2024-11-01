using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu]
public class PotionStats : ScriptableObject
{
    public string potionName;
    public int effectIndex;
    public float maxFill;
    public GameObject model;
    public AudioClip useSound; // sound effect loop when potion is used 

}
