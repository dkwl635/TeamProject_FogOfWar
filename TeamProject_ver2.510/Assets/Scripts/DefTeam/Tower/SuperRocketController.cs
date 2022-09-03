using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperRocketController : MonoBehaviour
{
    [HideInInspector] public GameObject Target = null;
    [HideInInspector] public float RocketDamage;
    public GameObject Explosion;
    float dis;
    float speed;
    float waitTime;

    // Start is called before the first frame update
    void Start()
    {
        if (Target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        dis = Vector3.Distance(this.transform.position, Target.transform.position);
        Destroy(this.gameObject, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        MoveOperation();
    }

    void MoveOperation()
    {
        if (Target == null)
        {
            Destroy(this.gameObject);
            return;
        }           

        waitTime += Time.deltaTime;

        if (waitTime < 1.5f)
        {
            speed += Time.deltaTime/3.0f;
            transform.Translate(this.transform.forward * speed, Space.World);

            Vector3 directionVec = Target.transform.position - this.transform.position;
            Quaternion qua = Quaternion.LookRotation(directionVec);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, qua, Time.deltaTime * 1.0f);
        }
        else
        {
            speed += Time.deltaTime/1.5f;
            float t = speed / dis;

            this.transform.position = Vector3.LerpUnclamped(this.transform.position, Target.transform.position, t);

            Vector3 directionVec = Target.transform.position - this.transform.position;
            Quaternion qua = Quaternion.LookRotation(directionVec);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, qua, Time.deltaTime * 1.0f);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Target.GetComponent<UnitDamage>().TakeDamage(RocketDamage);
            Destroy(this.gameObject);
            GameObject go = Instantiate(Explosion);
            go.transform.position = this.transform.position;
            Destroy(go.gameObject, 0.5f);
        }
    }
}
