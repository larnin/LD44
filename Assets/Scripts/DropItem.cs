using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour
{
    public bool autoloot = false;
    
    [SerializeField] GameObject m_tooltip;

    void Start()
    {
        if(m_tooltip != null)
            m_tooltip.SetActive(false);
    }

    public virtual void ApplyLoot(GameObject player)
    {

    }

    public void ShowTooltip()
    {
        if (m_tooltip != null)
            m_tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        if (m_tooltip != null)
            m_tooltip.SetActive(false);
    }

}
