using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    float RKSpeed = 100.0f;
    [HideInInspector] public GameObject Target = null;
    [HideInInspector] public float RocketDamage;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, RKSpeed / 60);

        if (Target.Equals(null))
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //기본
        if (other.CompareTag("Unit"))
        {
            Target.GetComponent<UnitDamage>().TakeDamage(RocketDamage);
            Destroy(this.gameObject);
        }
    }
}
