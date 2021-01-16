using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    // public float hitDamage = 1;
    public Enemy enemy;

    private int direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        direction = transform.position.x > collision.transform.position.x ? -1 : 1; // 施加给炸弹的力的方向

        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<IDmageable>().GetHit(enemy.attackDamage);
            float health = collision.GetComponent<PlayerController>().health;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * enemy.froce, enemy.froce *2), ForceMode2D.Impulse);
            Debug.Log("玩家受到攻击，伤害值: " + enemy.attackDamage + "点，剩余生命值: " + health);
        }
        if (collision.CompareTag("Bomb") && enemy.bombAvailble )
        {
            Debug.Log(enemy.enemyName+"击飞炸弹");
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction * enemy.froce, enemy.froce/2), ForceMode2D.Impulse);
        }
    }
}
