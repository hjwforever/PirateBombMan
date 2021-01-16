using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGuy : Enemy
{
    public Transform pickupPoint;

    public void PickUpBomb()  // 抱起炸弹
    {
        if (targetPoint.CompareTag("Bomb") && !hasBomb)
        {
            targetPoint.gameObject.transform.position = pickupPoint.position;
            targetPoint.SetParent(pickupPoint);
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            hasBomb = true;
        }
        // this.targetPoint.GetComponent<Bomb>().TurnOff();
        // Destroy(this.targetPoint.GetComponent<Bomb>().gameObject);
        
        //this.targetPoint.GetComponent<Bomb>().transform.position = Vector2.MoveTowards(this.targetPoint.GetComponent<Bomb>().transform.position, targetPoint.position, speed * Time.deltaTime);      
    }
    
    public void ThrowBomb()  // 扔走炸弹
    {
        if (hasBomb)
        {
            targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            targetPoint.SetParent(transform.parent.parent);

            if (FindObjectOfType<PlayerController>().gameObject.transform.position.x - transform.position.x < 0)
                targetPoint.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-froce, froce/2), ForceMode2D.Impulse);
            else
                targetPoint.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(froce, froce/2), ForceMode2D.Impulse);
        }

        hasBomb = false;

/*        Vector3 pos = new Vector3(transform.rotation.y == 1 ? -froce : froce, 0, 0);
        this.targetPoint.GetComponent<Bomb>().GetComponent<Collider2D>().GetComponent<Rigidbody2D>().AddForce((pos + Vector3.up), ForceMode2D.Impulse);*/
    }
}
