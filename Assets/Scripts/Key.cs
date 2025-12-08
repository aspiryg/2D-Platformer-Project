using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject door;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Destroy(door);
            Destroy(gameObject);
        }
    }
}
