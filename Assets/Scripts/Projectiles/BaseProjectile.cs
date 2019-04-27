using UnityEngine;
using System.Collections;

public abstract class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float m_baseDamage;

    public bool isPlayerProjectile = true;

    public abstract void SetDirection(Vector2 dir);
}
