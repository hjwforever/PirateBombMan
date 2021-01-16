using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator anim;
    private Collider2D coll;
    private Rigidbody2D rb;

    public float startTime;
    public float waitTime;
    public float bombForce;
    public float bombDamage;

    [Header("Check")]
    public float radius = 1.38f;
    public LayerMask targetLayer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Bomb_Off"))
        {
            if(Time.time > startTime + waitTime)
            {       
                anim.Play("Bomb_Explotion");
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public void Explotion()
    {
        coll.enabled = false;
        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        rb.gravityScale = 0;
        foreach (var item in aroundObjects)
        {
            Vector3 pos = item.transform.position - transform.position;
            
            item.GetComponent<Rigidbody2D>().AddForce((pos + Vector3.up) * bombForce, ForceMode2D.Impulse);

            if (item.CompareTag("Bomb") && item.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Bomb_Off"))
            {
                item.GetComponent<Bomb>().TurnOn();
            }
            else if (item.CompareTag("Player") || item.CompareTag("NPC"))
                item.GetComponent<IDmageable>().GetHit(bombDamage);
        }
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void TurnOff()  // 炸弹熄灭
    {
        anim.Play("Bomb_Off");
        gameObject.layer = LayerMask.NameToLayer("NPC");
    }

    public void TurnOn()  // 炸弹引燃
    {
        startTime = Time.time;
        anim.Play("Bomb_On");
        gameObject.layer = LayerMask.NameToLayer("Bomb");
    }
}
