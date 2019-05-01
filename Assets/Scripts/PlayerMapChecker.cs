using UnityEngine;
using System.Collections;

public class PlayerMapChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerControler>() != null)
            Event<HideMapEvent>.Broadcast(new HideMapEvent(true));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerControler>() != null)
            Event<HideMapEvent>.Broadcast(new HideMapEvent(false));
    }
}
