using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    [Header("Breakable Wall Settings")]
    public AudioClip breakSound;

    [Tooltip("Particle effect spawned when wall breaks (optional)")]
    public GameObject breakParticles;

    [Tooltip("How long to wait before destroying wall after break")]
    public float destroyDelay = 0.1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            // 
            if (player != null && IsPlayerDashing(player))
            {
                BreakWall();
            }
        }
    }

    private bool IsPlayerDashing(PlayerController player)
    {
        // 
        var dashField = typeof(PlayerController).GetField("isDashing",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (dashField != null)
        {
            return (bool)dashField.GetValue(player);
        }

        return false;
    }

    
    //
    private void BreakWall()
    {
        // Play break sound
        if (breakSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(breakSound, 0.6f);
        }

        // Add camera shake effect (from e CameraShake singleton)
        //if (CameraShake.Instance != null)
        //{
        //    CameraShake.Instance.Shake(0.15f, 0.1f);
        //}

        // Spawn particles effect
        if (breakParticles != null)
        {
            Instantiate(breakParticles, transform.position, Quaternion.identity);
        }

        // Destroy wall after short delay
        Destroy(gameObject, destroyDelay);
    }

}
