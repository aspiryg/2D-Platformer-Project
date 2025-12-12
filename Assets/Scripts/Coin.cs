using UnityEngine;
using TMPro;


public class Coin : MonoBehaviour
{
    //
    [Header("Coin Settings")]
    public AudioClip coinClip;
    public int coinsToGive = 1;
    // References
    private static TextMeshProUGUI coinText;
    private void Awake()
    {
        // find the text only once
        if (coinText == null)
        {
            GameObject coinTextObject = GameObject.FindWithTag("CoinText");
            if (coinTextObject != null)
            {
                coinText = coinTextObject.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning("Coin: No GameObject with tag 'CoinText' found. Please add this tag to your coin counter UI.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player collected this coin
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                // Award coins to player
                player.coins += coinsToGive;

                // Play collection sound
                AudioManager.Instance.PlaySFX(coinClip, 0.4f);

                // Update UI if available
                if (coinText != null)
                {
                    coinText.text = player.coins.ToString();
                }

                // Destroy this coin
                Destroy(gameObject);
            }
        }
    }
}
