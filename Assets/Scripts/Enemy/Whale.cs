using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whale : Enemy
{
    public float scale;
    public float maxScale;
    public float recoveryHealth;

    public void Swalow()
    {
        targetPoint.GetComponent<Bomb>().TurnOff();
        Destroy(targetPoint.gameObject);

        if(Mathf.Abs(transform.localScale.x) < maxScale)
            transform.localScale *= scale;

        Debug.Log(enemyName+"血量回复了"+ recoveryHealth);
        health += recoveryHealth;
    }
}
