using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour
{
    public bool isPlayerProjectile = true;

    public abstract void SetDirection(Vector2 dir);
}
