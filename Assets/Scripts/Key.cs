using UnityEngine;

public class Key : MonoBehaviour
{

    [Header("Key Settings")]
    public GameObject door;

    [Header("Audio")]
    [Tooltip("Sound played when key is collected")]
    public AudioClip keyClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(door);
            Destroy(gameObject);

            // Play key collection sound
            if (keyClip != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(keyClip, 0.5f);

            }
        }
    }
}
