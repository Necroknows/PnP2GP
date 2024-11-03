/*
 * Author: Jesse Mercer
 * Date: 10-16-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This simple Script serves as a contract for all objects that can receive damage.
 *     It ensures that any class implementing this interface must handle taking damage and optionally apply 
 *     a push back force and direction.
 * Version: 1.5
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    void takeDamage(int amount, Vector3 Dir); // Standard Damage method to pass damage amount and direction 

}

