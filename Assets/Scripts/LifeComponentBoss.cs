using UnityEngine;
using System.Collections;

public class LifeComponentBoss : LifeComponent
{

    protected override void Start()
    {
        base.Start();

        Event<BossLifeChangeEvent>.Broadcast(new BossLifeChangeEvent(m_life, m_maxLife));
    }

    public override void Damage(float power)
    {
        base.Damage(power);

        Event<BossLifeChangeEvent>.Broadcast(new BossLifeChangeEvent(Mathf.Max(m_life, 0), m_maxLife));
    }

    protected override void CreateKillAnimation()
    {
        if (m_deathObject != null)
        {
            var obj = Instantiate(m_deathObject);
            obj.transform.position = transform.position;

            var anim = obj.GetComponent<Animator>();

            var thisAnim = GetComponent<Animator>();
            if(thisAnim != null && !thisAnim.GetBool("Left"))
                anim.SetTrigger("Right");
        }
    }
}
