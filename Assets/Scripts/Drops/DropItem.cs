using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using NRand;

public class DropItem : SerializedMonoBehaviour
{
    public bool autoloot = false;

    [SerializeField] float m_startDropSpeed = 1.5f;
    [SerializeField] float m_dropAnimationTime = 1.0f;
    

    Rigidbody2D m_rigidbody = null;
    float m_dropTime = 0;
    Vector2 m_dropDir;
    GameObject m_tooltip = null;

    void Start()
    {
        m_tooltip = transform.Find("Tooltip")?.gameObject;
        if(m_tooltip != null)
            m_tooltip.SetActive(false);

        m_rigidbody = GetComponent<Rigidbody2D>();

        m_dropDir = new UniformVector2CircleSurfaceDistribution(1).Next(new StaticRandomGenerator<MT19937>());
    }

    private void Update()
    {
        if(m_dropTime > m_dropAnimationTime)
        {
            m_rigidbody.velocity = new Vector2(0, 0);
            m_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }
        float speedMultiplier = 1 - m_dropTime / m_dropAnimationTime;

        m_rigidbody.velocity = m_dropDir * m_startDropSpeed * speedMultiplier;
        Debug.Log(m_rigidbody.velocity);
        m_dropTime += Time.deltaTime;
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
