using UnityEngine;
using System.Collections;

public class MouseLocker : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
