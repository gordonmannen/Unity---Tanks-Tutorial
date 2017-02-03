using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;                // First to win 5 rounds is game winner.
    public float m_StartDelay = 3f;                 // Delayed pause between round starting & round playing phases.
    public float m_EndDelay = 3f;                   // Delayed pause between round playing & round ending phases.
                                                    // The idea is to give players a moment to read the text regarding score, round/game winner, etc.
    public CameraControl m_CameraControl;           // Reference to camera control script.
    public Text m_MessageText;                      // Reference to text that will display score/winners, etc.
    public GameObject m_TankPrefab;                 // Reference to prefab Tanks that will be controlled by players.
    public TankManager[] m_Tanks;                   // Reference to managers that will control different aspects of the tanks.


    private int m_RoundNumber;                      // Current round
    private WaitForSeconds m_StartWait;             // Delay before round begins and players can start moving/shooting.
    private WaitForSeconds m_EndWait;               // Delay after round ends to display round/game winner text, etc.
    private TankManager m_RoundWinner;              // Used to announce round winner.
    private TankManager m_GameWinner;               // Used to announce game winner once 5 rounds have been won by a given player.


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
                                                    // Build delays to it is easier to adjust the 'amount' of delay later if desired.

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());                 // Start game after tanks spawned and camera set.
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
                                                    // Iterate through and spawn a number of tanks equal to the total no of players.  Set player number which is then used
                                                    // as a point of reference for the tank controlled by player x, etc.
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;        // This section is used to iterate through the active tanks and determine the valid targets for the camera control.
    }


    private IEnumerator GameLoop()                  // Called to start the game and will run through each phase of the game.
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());
                                                    // Each of these coroutine phases need to be completed in order, ex playing cannot begin until starting is finished.

        if (m_GameWinner != null)                   // Has a player won the game?
        {
            SceneManager.LoadScene(0);              // If so start a new game.
        }
        else
        {
            StartCoroutine(GameLoop());             // If not start a new round.
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();                       // Round is starting - reset tanks and disable controls until round playing phase.

        m_CameraControl.SetStartPositionAndSize();  // Reposition camera position and size based on the spawn points of the player tanks.

        m_RoundNumber++;                            // Increment the round number and display text identifying the round to the players.
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;                   // Desired delay and then continue to playing phase.
    }


    private IEnumerator RoundPlaying()
    {

        EnableTankControl();                        // Re-enable tank controls.

        m_MessageText.text = string.Empty;          // Clear round number text.

        while (!OneTankLeft())                      // While loop until there is only one tank left, then end playing phase and move to ending.
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {

        DisableTankControl();                       // Disable tank controls.

        m_RoundWinner = null;                       // Clear previous round winner text.

        m_RoundWinner = GetRoundWinner();           // Determine new round winner & check that there is not yet a game winner.

        if (m_RoundWinner != null)                  // Increment score.
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();             // Check for a game winner.

        string message = EndMessage();              // Display new message (either round winner or game winner)
        m_MessageText.text = message;

        yield return m_EndWait;                     // Delay before either starting a new round or new game.
    }


    private bool OneTankLeft()                      // Is there one or less tanks active?
    {
        int numTanksLeft = 0;                       // Count could be as low as 0 (draw scenario)

        for (int i = 0; i < m_Tanks.Length; i++)    // Iterate through tanks whether active or not.
        {
            if (m_Tanks[i].m_Instance.activeSelf)   // Increment count based on active tank(s).
                numTanksLeft++;
        }

        return numTanksLeft <= 1;                   // If tanks left is one or none then return true, else false
    }


    private TankManager GetRoundWinner()            // Determine round winner
    {
        for (int i = 0; i < m_Tanks.Length; i++)    // Iterate through tanks
        {
            if (m_Tanks[i].m_Instance.activeSelf)   // Determine if there is a single tank still active, if so that is the round winner.
                return m_Tanks[i];
        }

        return null;                                // No active tanks = a drew = a push - no round winner.
    }


    private TankManager GetGameWinner()             // Determine if round winner has won enough rounds to win the game.
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];                  // We have a winner, return game winner.
        }

        return null;                                // No game winner yet.
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;                         // These messages are returned based on the results of the round/game.
    }


    private void ResetAllTanks()                // Resets the tanks to their starting positions and properties (no damage, etc.)
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()            // Enable controls - allows for a set and controllable point that players can start driving around and shooting.
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()           // Disable controls so players aren't driving around at the end of the round/game.
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}