using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DropBank : MonoBehaviour
{
    static DropBank m_instance = null;

    [SerializeField] List<GameObject> m_prefabs = new List<GameObject>();

    public static DropBank Instance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;
    }

    public void Spawn(int index, Vector3 pos)
    {
        if (index < 0 || index >= m_prefabs.Count)
            return;

        var obj = Instantiate(m_prefabs[index]);
        obj.transform.position = pos;
    }
}
