using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GoldGain : MonoBehaviour
{
    [SerializeField] float m_showTime = 2;
    [SerializeField] Vector2 m_moveSpeed = new Vector2(0, 1);

    public void SetValue(int value)
    {
        TextMesh text = GetComponentInChildren<TextMesh>();
        if (text == null)
            return;

        text.text = "";
        if (value > 0)
            text.text = "+";
        text.text += value.ToString();
    }

    private void Start()
    {
        transform.DOMove(transform.position + new Vector3(m_moveSpeed.x, m_moveSpeed.y) * m_showTime, m_showTime);
        TextMesh text = GetComponentInChildren<TextMesh>();
        if (text != null)
            DOVirtual.Float(1, 0, m_showTime, x =>
            {
                if (text == null)
                    return;

                Color c = text.color;
                c.a = x;
                text.color = c;
            });

        SpriteRenderer s = GetComponentInChildren<SpriteRenderer>();
        if (s != null)
        {
            var c = s.color;
            c.a = 0;
            s.DOColor(c, m_showTime);
        }
    }
}
