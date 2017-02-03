using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;                      // public variables will show up in the inspector in Unity
                                                        // allows the game manager to differentiate between the two tanks used in the game.
    public float m_Speed = 12f;                         // Speed of tank moving forward and backwards.
    public float m_TurnSpeed = 180f;                    // Speed at which tank can rotate in degrees/second
    public AudioSource m_MovementAudio;                 // Reference to the audio for tank movement (idling/driving)
    public AudioClip m_EngineIdling;                    // Clip that plays during idle
    public AudioClip m_EngineDriving;                   // Clip that plays while driving
    public float m_PitchRange = 0.2f;                   // Degree to which pitch will vary


    private string m_MovementAxisName;                  // Axis reference for movement
    private string m_TurnAxisName;                      // Axis reference for turning
    private Rigidbody m_Rigidbody;                      // Rigidbody reference needed to move tank around game environment
    private float m_MovementInputValue;                 // Current movement input
    private float m_TurnInputValue;                     // Current turn input
    private float m_OriginalPitch;                      // pitch at start of the round/game
    private ParticleSystem[] m_particleSystems;         // Reference to particle systems


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;                // As tank is enabled it can start to move.

        m_MovementInputValue = 0f;                      // Reset the input values.
        m_TurnInputValue = 0f;


        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
                                                        // Grab component references to particle systems for a tank in order to enable/disable, play/stop, etc.
        for (int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Play();
        }
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;                 // Tank has stopped, stop moving.

        for(int i = 0; i < m_particleSystems.Length; ++i)
        {
            m_particleSystems[i].Stop();                // Tank has stopped or respawned, stop particle system dust trail.
        }
    }


    private void Start()
    {
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        m_OriginalPitch = m_MovementAudio.pitch;        // Set the starting pitch at the beginning of a round/game.
    }


    private void Update()
    {
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);

        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        EngineAudio();                                  // Store the player's inputs and make sure the audio for the engine is playing.
    }


    private void EngineAudio()
    {
        if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
        {                                               // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();                 // to direct the game to play the new audio (idle) after changing pitch, etc.
            }
        }
        else
        {
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();                 // to direct the game to play the new audio (drive) after changing pitch, etc.
            }
        }
    }


    private void FixedUpdate()
    {
        Move ();                                        // Move and turn the tank and update the position and rotation.
        Turn ();
    }


    private void Move()
    {
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
                                                        // Establish a vector that will determine the new position of the tank based on speed, direction, and time, etc.

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
                                                        // Adjust the position of the tank based on the player's input.
    }


    private void Turn()
    {
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
                                                        // Adjust the rotation of the tank based on the player's input.

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                                                        // Determine the rotation
                                                        // turn = variable in this case, Turn = function in this case

        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
                                                        // Apply the calculated rotation so the tank rigidbody will appear at its new rotation.
    }
}