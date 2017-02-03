using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;               // Starting tank health
    public Slider m_Slider;                             // Used to display a simple visual of remaining tank health
    public Image m_FillImage;                           // Image component, background base, etc.
    public Color m_FullHealthColor = Color.green;       // Color of remaining health.
    public Color m_ZeroHealthColor = Color.red;         // Color of missing health.
    public GameObject m_ExplosionPrefab;                // The prefab base to be instantiated.


    private AudioSource m_ExplosionAudio;               // Audio to play when tank explodes as opposed to a shell explosion.
    private ParticleSystem m_ExplosionParticles;        // Particle effect to play when tank explodes.
    private float m_CurrentHealth;                      // Current tank health.
    private bool m_Dead;                                // Is tank at zero HP?


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
                                                        // Instantiate explosion prefab + get component particle system reference.
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
                                                        // Get component audio source reference.

        m_ExplosionParticles.gameObject.SetActive(false);
                                                        // Disable prefab, it will be reactivated when it is needed.
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;             // Reset tank's health and status.
        m_Dead = false;

        SetHealthUI();                                  // Reset health slider UI.
    }


    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;                      // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        SetHealthUI();                                  // Update health slider as tank takes damage.

        if (m_CurrentHealth <= 0f && !m_Dead)           // Check to see if tank is dead.
        {
            OnDeath();                                  // If tank is dead call OnDeath function.
        }
    }


    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;               // Adjust the value and color of the slider.

        m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
                                                        // Change color of bar based on colors chosen and remaining health.
    }


    private void OnDeath()
    {
        m_Dead = true;                                  // Flip flag to dead.

        m_ExplosionParticles.transform.position = transform.position;
                                                        // Move explosion effect to the current position of the dead tank.
        m_ExplosionParticles.gameObject.SetActive(true);// Re-activate prefab explosion that was previously disabled.

        m_ExplosionParticles.Play();                    // Play explosion particles.

        m_ExplosionAudio.Play();                        // Play explosion audio.

        gameObject.SetActive(false);                    // Deactivate tank, this is probably primarily an issue if you have more than two players and want some tanks to still be active and dead tanks to be inactive.
    }
}