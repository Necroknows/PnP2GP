/*
 * Author(s): Alexandria Dixon, Jesse Mercer, Orion White
 * Date: 10-17-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script serves as the central manager for the game, handling player references, tracking enemy count for game progression, managing retrievable objects, and managing game states such as win or lose conditions.
 *     It is designed to integrate future scene management functionalities.
 *
 * Version: 1.0
 * 
 * Additional Notes:
 * - The UI-related functionalities have been moved to the UIManager script.
 * - This script focuses on core game management, including future scene handling.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;    // Singleton pattern for easy access

    // --- PLAYER AND UI REFERENCES ---
    public GameObject flashDamageScreen;   // Flash screen when the player takes damage
    public GameObject player;              // Player GameObject reference
    public playerController playerScript;  // Player script reference
    public GameObject playerSpawnPOS;      // Player spawn position reference
    public GameObject pumpkin;             // Example of a retrievable object (can be generalized later)

    public bool isPaused;                  // Tracks if the game is paused
    private int enemyCount;                // Tracks remaining enemy count
    private int retrievableCount;          // Tracks remaining retrievable objects count

    // --- RETRIEVABLE OBJECTS LIST ---
    List<RetrievableObjects> retrievableObjects = new List<RetrievableObjects>();

    // --- AWAKE: Initialize GameManager Singleton and Player Reference ---
    void Awake()
    {
        // Ensure only one instance of the GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Find and assign the player object and its script
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPOS = GameObject.FindWithTag("PlayerSpawnPOS");

        // Fill the list with all retrievable objects in the scene
        fillRetrievables();
    }

   
    void Update()
    {
      
    }

    // --- ENEMY TRACKING: Updates the enemy count, triggers win condition if all enemies are defeated ---
    public void updateGameGoal(int amount)
    {
        enemyCount += amount;

        // Check if all enemies are defeated
        if (enemyCount <= 0)
        {
            // Trigger win condition
            UIManager.Instance.ShowWinScreen();
        }
    }

    // --- GET ENEMY COUNT: Return the current enemy count ---
    public int GetEnemyCount()
    {
        return enemyCount;
    }

    // --- RETRIEVABLE OBJECTS HANDLING ---

    // Fill list with all retrievable objects in the scene
    public void fillRetrievables()
    {
        RetrievableObjects[] allRetrievables = FindObjectsOfType<RetrievableObjects>();

        // Add each retrievable object to the list
        foreach (RetrievableObjects retrievable in allRetrievables)
        {
            retrievableObjects.Add(retrievable);
        }
    }

    // Handle the retrieval of an object
    public void RetrieveObject(RetrievableObjects retrievable)
    {
        if (!retrievable.isRetrieved)
        {
            retrievable.Retrieve();  // Mark the object as retrieved
            updateRetrievableCount(); // Update retrievable count
        }
    }

    // Update the count of retrievable objects
    public int updateRetrievableCount()
    {
        retrievableCount = retrievableObjects.Count;
        return retrievableCount;
    }

    // Reset all retrievable objects for restarting the game/level
    public void ResetAllObjects()
    {
        foreach (var retrievable in retrievableObjects)
        {
            retrievable.ResetObject();  // Reset each object to its original state
        }
    }
}
