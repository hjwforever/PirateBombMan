using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    public float radius;
    public LayerMask targetLayer;

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 游戏开始页面 骨头若砸到桌上 惊醒Big Guy
        if (collision.name == "Red Bottle")
        {
            Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
            foreach (var item in aroundObjects)
            {
                if (item.CompareTag("NPC") && item.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("New State"))
                {
                    item.GetComponent<Animator>().SetTrigger("animation");
                }
            }
        }
    }
}
