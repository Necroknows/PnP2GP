/*
 * Author(s): Alexandria Dixon, Jesse Mercer
 * Date: 10-17-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script serves as the central manager for the game, handling player references, tracking enemy count for game progression, and managing game states such as win or lose conditions.
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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;    // Singleton pattern for easy access

    // --- PLAYER AND UI REFERENCES ---
    public Image playerHpBar;              // Health bar UI reference
    public Image playerFuelBar;            // Fuel bar UI reference

    public GameObject flashDamageScreen;   // Flash screen when the player takes damage
    public GameObject player;              // Player GameObject reference
    public playerController playerScript;  // Player script reference

    public bool isPaused;                  // Tracks if the game is paused
    private int enemyCount;                // Tracks remaining enemy count

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
    }

    // --- UPDATE: Game Loop Logic (if needed) ---
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

    // --- FUTURE: Placeholder for future scene management logic ---
    // This is where future scene management methods will be implemented, handling transitions between different scenes (e.g., levels, menus).
}
