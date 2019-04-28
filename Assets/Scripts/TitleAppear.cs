using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using NRand;

public class TitleAppear : MonoBehaviour
{
    [SerializeField] Vector2 m_startPos = new Vector2(0, 0);
    [SerializeField] float m_startScale = 1;
    [SerializeField] float m_duration = 1;
    [SerializeField] Camera m_camera = null;
    [SerializeField] float m_shakePower = 1;
    [SerializeField] float m_shakeTime = 0.2f;

    bool m_ready = false;
    float m_shakeDuration = 0;

    private void Start()
    {
        var pos = transform.position;
        var scale = transform.localScale;
        transform.position = new Vector3(m_startPos.x, m_startPos.y, 0);
        transform.localScale = new Vector3(m_startScale, m_startScale, m_startScale);

        transform.DOMove(pos, m_duration).OnComplete(() => { m_ready = true; });
        transform.DOScale(scale, m_duration);
    }

    private void Update()
    {
        if (!m_ready || m_shakeDuration > m_shakeTime)
            return;

        m_shakeDuration += Time.deltaTime;
        if(m_shakeDuration > m_shakeTime)
        {
            m_camera.transform.position = new Vector3(0, 0, m_camera.transform.position.z);
            return;
        }

        var dir = new UniformVector2CircleDistribution(m_shakePower).Next(new StaticRandomGenerator<MT19937>());

        m_camera.transform.position = new Vector3(dir.x, dir.y, m_camera.transform.position.z);
    }
}
