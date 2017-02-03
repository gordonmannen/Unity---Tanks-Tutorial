using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                    // Used to make sure shells only affect tanks (players)
    public ParticleSystem m_ExplosionParticles;     // Reference to explosion particle effect.
    public AudioSource m_ExplosionAudio;            // Reference to explosion audio.
    public float m_MaxDamage = 100f;                // Damage done with a perfect hit.
    public float m_ExplosionForce = 1000f;          // Force added to tank if at the center of that perfect hit.
    public float m_MaxLifeTime = 2f;                // Time before a shell is removed if is still in 'play'.
    public float m_ExplosionRadius = 5f;            // Max blast radius.


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);         // If the instantiated shell is still alive (hasn't struck anything) after 2 secs, kill it.
    }


    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
                                                    // Find all the tanks in an area around the shell and damage them.

        for (int i = 0; i < colliders.Length; i++)  // Iterate through colliders
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
                                                    // Find their rigidbody component

            if (!targetRigidbody)                   // If not rigidbody, go to next collider
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
                                                    // Add the explosion force.

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                                                    // Get tank health from the rigidbody target.

            if (!targetHealth)                      // If the rigidbody targetted does not have a tank health (like a building), move on to next collider.
                continue;

            float damage = CalculateDamage(targetRigidbody.position);
                                                    // Calculate damage.

            targetHealth.TakeDamage(damage);        // Deal damage.
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();                // Play particle effect of explosion.

        m_ExplosionAudio.Play();                    // Play audio of explosion.

        ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
        Destroy(m_ExplosionParticles.gameObject, mainModule.duration);
                                                    // When particle effect finishes, destroy game object they are on.

        Destroy(gameObject);                        // Destroy shell.
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
                                                    // Calculate the amount of damage a target should take based on it's position.

        Vector3 explosionToTarget = targetPosition - transform.position;
                                                    //  Determine the vector between shell and target.

        float explosionDistance = explosionToTarget.magnitude;
                                                    // Calculate distance from shell to target.

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
                                                    // Calculate the percentage of the perfect 'direct' hit.

        float damage = relativeDistance * m_MaxDamage;
                                                    // Based on percentage 'success' of hit, calculate damage.

        damage = Mathf.Max(0f, damage);             // Insure minimum damage is zero (rather than adding health to target)

        return damage;
    }
}