using System;
using UnityEngine;                                  // Note:  no Monobehavior as the Tank Manager is not inheriting from a parent, etc.

[Serializable]                                      // This will put class TankManager in the Inspector
public class TankManager                            // Public so it can be used throughout to manage various aspects of the tanks.  Interacts with the GameManager to manage when tank controls are enable or disabled during the different game phases, etc.
{
    public Color m_PlayerColor;                     // Sets tank color.
    public Transform m_SpawnPoint;                  // Unique spawn point for each player (tank)
    [HideInInspector] public int m_PlayerNumber;    // Identifies which player/tank is associated with this manager.
    [HideInInspector] public string m_ColoredPlayerText;
                                                    // Player text color will be same as tank color.
    [HideInInspector] public GameObject m_Instance; // A reference to the instance of the tank as it is spawned.
    [HideInInspector] public int m_Wins;            // Running tally of rounds won by a given player.


    private TankMovement m_Movement;                // Reference to tank movement script
    private TankShooting m_Shooting;                // Reference to tank shooting script
    private GameObject m_CanvasGameObject;          // Used to control the world space UI.


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
                                                    // Get component references.

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber; // Insure player number is the same for movement and shooting scripts.

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
                                                    // Use HTML to create the string that will be used to display 'Player x' in a particular color in the game's message texts.

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
                                                    // Get the renderers for the tanks.

        for (int i = 0; i < renderers.Length; i++)  // Iterate through the renderers.
        {
            renderers[i].material.color = m_PlayerColor;
                                                    // Set tank color.
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);        // Called when we want to temporarily disable tank controls.
    }


    public void EnableControl()                     // Called when we want to re-enable tank controls.
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()                             // Used to reset the tanks positions/state to start a round/game.
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
