using UnityEngine;
using System.Collections;

public class DisableOnWeb : MonoBehaviour
{
    void Start()
    {
#if UNITY_WEBGL
        Destroy(gameObject);
#endif
    }
}
