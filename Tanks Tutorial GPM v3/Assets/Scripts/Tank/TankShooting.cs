using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              // Player Number used to differentiate between players
    public Rigidbody m_Shell;                   // Prefab of the shell
    public Transform m_FireTransform;           // The preset spot where the shells will show up when spawned
    public Slider m_AimSlider;                  // Displays the current launch force
    public AudioSource m_ShootingAudio;         // Reference to audio source for playing shooting audio
    public AudioClip m_ChargingClip;            // Audio clip that plays while charging
    public AudioClip m_FireClip;                // Audio clip that plays when a shell is fired
    public float m_MinLaunchForce = 15f;        // The minimum launch force
    public float m_MaxLaunchForce = 30f;        // The maximum launch force
    public float m_MaxChargeTime = 0.75f;       // The maximum time the launcher can be held before a shell is fired, also the charge time required for max launch force.


    private string m_FireButton;                // The button that serves as the input for firing shells, in this case different for Player 1 & Player 2
    private float m_CurrentLaunchForce;         // Dependent on how long the fire button was held before being released, but has minimum and maximum parameters.
    private float m_ChargeSpeed;                // Speed at which shot is charged up.
    private bool m_Fired;                       // Has shot been fired with current fire button press


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;   // Tank is enabled, reset current launch force & aim slider
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; // firebutton assigned based on Player 1 or Player 2

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
                                                // Time to charge up based on max charge/launch force, limited by min & max parameters
    }


    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;   // Track the current state of the fire button and make decisions based on the current launch force.

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {                                       // not yet fired...

            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();                             // Current launch force is max, fire at max force
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
                                                // have we just pressed the button?
            m_Fired = false;                    // reset fired status and current launch force to minimum
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();             // Play the charging audio clip
        }
        else if(Input.GetButton(m_FireButton) && !m_Fired)
                                                // Holding the fire button, not yet fired
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                                                // Increment launch force & update slider

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if(Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
                                                // We released the button, having not fired yet
            Fire();                             // launch shell
        }
    }


    private void Fire()
    {
        m_Fired = true;                         // Set fired flag.

        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
                                                // Instantiate shell.

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
                                                // Set shell's velocity based on current launch force at time fired

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();                 // Play shooting audio clip

        m_CurrentLaunchForce = m_MinLaunchForce;// Reset launch force
    }
}