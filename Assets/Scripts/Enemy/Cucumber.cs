using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cucumber : Enemy
{
    public void setOff()  // 吹灭炸弹
    {
        this.targetPoint.GetComponent<Bomb>().TurnOff();
    }
}
