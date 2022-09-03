using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomCtrl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
            other.gameObject.GetComponent<TowerDamage>().TakeDamage(20);

        SoundManager.Instance.PlayEffSound("BombSound", transform.position);

        Destroy(gameObject, 1f);
    }
}
