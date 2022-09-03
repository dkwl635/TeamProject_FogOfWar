using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [HideInInspector] public int damage = 10;

    float m_LifeTime = 4.0f;
    Vector3 m_OwnTrPos;
    Vector3 m_DirTgVec;
    Vector3 a_StartPos;

    GameObject target;
    Vector3 targetVec;

    Vector3 a_MoveNextStep;
    float m_MoveSpeed = 70f;

    TrailRenderer trailRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_LifeTime -= Time.deltaTime;
        if (m_LifeTime <= 0)
            Destroy(this.gameObject);

        if (targetVec.x - this.transform.position.x < 1 && targetVec.x - this.transform.position.x > -1
            && targetVec.z - this.transform.position.z < 1 && targetVec.z - this.transform.position.z > -1)
        {
            if(target != null && target.CompareTag("Tower"))
            {
                target.GetComponentInChildren<TowerDamage>().TakeDamage(damage);
            }
            else if(target != null && target.CompareTag("Unit"))
            {
                target.GetComponent<UnitDamage>().TakeDamage(damage);
            }

            Destroy(this.gameObject);
        }

        a_MoveNextStep = m_DirTgVec * Time.deltaTime * m_MoveSpeed;
        transform.position += a_MoveNextStep;
    }

    public void BulletSpawn(Transform a_OwnTr, Vector3 a_DirVec, GameObject a_Target)
    {
        target = a_Target;
        targetVec = a_Target.transform.position;
        m_OwnTrPos = a_OwnTr.position;

        m_DirTgVec = a_DirVec;
        m_DirTgVec.y = 0.05f;
        m_DirTgVec.Normalize();

        a_StartPos = m_OwnTrPos + (m_DirTgVec*1.2f);
        a_StartPos.y = transform.position.y;

        transform.position = a_StartPos;
        transform.rotation = Quaternion.LookRotation(m_DirTgVec);

        m_LifeTime = 4;

        if(trailRenderer != null)
            trailRenderer.time = -1.0f;
    }
}