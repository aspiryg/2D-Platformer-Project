using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip coinClip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.coins += 1;
            player.PlaySFX(coinClip, 0.4f);
            Destroy(gameObject);
        }
    }
}
