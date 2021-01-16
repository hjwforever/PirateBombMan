using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IDmageable
{
    EnemyBaseState currentState;

    public Animator anim;
    public int animState;

    private GameObject alarmSign;

    [Header("Base State")]
    public string enemyName;
    public float health;
    public float froce;
    public bool isDead;
    public bool isBoos;
    public bool hasBomb;

    [Header("Movement")]
    public float speed = 2;
    public Transform pointA, pointB;
    public Transform targetPoint;

    [Header("Attack Setting")]
    public float attackRate;   
    public float attackRange;
    public float attackDamage;
    private float nextAttack = 0;

    [Header("Skill Setting")]
    public float skillRate;
    public float skillRange;
    // private float nextSkill = 0;
    public bool bombAvailble;

    public List<Transform> attackList = new List<Transform>();

    public ProtalState protalState = new ProtalState();
    public AttackState attackState = new AttackState();

    public virtual void Init()
    {
        this.anim = GetComponent<Animator>();
        this.alarmSign = transform.GetChild(0).gameObject; // 警告条
    }

    public void Awake()
    {
        Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.GetInstance().AddToEnemyList(this); // 添加至敌人列表

        TransitionToState(protalState);

        if (isBoos)
            UIManager.GetInstance().SetBossHealth(health);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (isDead)
        {
            anim.SetBool("dead", isDead);

            if (GameManager.GetInstance().InEnemyList(this))
                GameManager.GetInstance().RemoveFromEnemyList(this);
            return;
        }
       
        currentState.OnUpdate(this);
         anim.SetInteger("state", animState);
    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void MoveToTarget() // 移动至目标点
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FilpDirection();
    }

    public void AttackAction() // 攻击
    {
        if (Vector2.Distance(transform.position , targetPoint.position) < attackRange)
        {
            if (Time.time >= nextAttack)
            {
                anim.SetTrigger("attack");
                Debug.Log(enemyName+"对玩家发动了普通攻击");
                nextAttack = Time.time + attackRate;
            }
        }
    }

    public virtual void SkillAction()  // (对炸弹)独有技能行为
    {
        if (Vector2.Distance(transform.position , targetPoint.position) < skillRange)
        {
            if (Time.time >= nextAttack)
            {
                anim.SetTrigger("skill");
                Debug.Log(enemyName+"对炸弹发动特殊技能");
                nextAttack = Time.time + skillRate;
            }
        }

    }

    public void FilpDirection() // 翻转方向
    {
        if (transform.position.x < targetPoint.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    public void SwitchPoint() // 切换目标点
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform) && !hasBomb && !GameManager.GetInstance().gameOver)
            attackList.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*        if(!isDead && !GameManager.GetInstance().gameOver)
                {
                    anim.Play("run");   
                    StartCoroutine(OnAlarm());
                }*/
        if (!isDead && !GameManager.GetInstance().gameOver)
            StartCoroutine(OnAlarm());
    }

    // 警告信息条 协程
    IEnumerator OnAlarm()
    {
        alarmSign.SetActive(true);
        yield return new WaitForSeconds(alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        alarmSign.SetActive(false);
    }

    // 受伤
    public virtual void GetHit(float damage)
    {
        health -= damage;
        if (health < 1)
        {
            health = 0;
            isDead = true;
        }   
        anim.SetTrigger("hit");

        if (isBoos)
        {
            UIManager.GetInstance().SetBossHealth(health);
        }

        Debug.Log(enemyName + "受到攻击,伤害值:" + damage + "当前生命值: " + health);
        if (health <= 0)
        {
            Dead();
        }
    }


    public void Dead()
    {
        Debug.Log(enemyName + "死亡!");
        StartCoroutine(AfterDead());
        GameManager.GetInstance().RemoveFromEnemyList(this);
    }

    IEnumerator AfterDead(float totalTime = 3f, int steps = 5)
    {
        Color c = GetComponent<SpriteRenderer>().color;

        float time = 0f;
        float delatTime = totalTime / steps;

        isDead = true;
        while (time < totalTime)
        {
            c.a -= (float)1/ steps;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(delatTime);
            time += delatTime;
        }

        // Debug.Log("销毁");
        Destroy(gameObject);
    }
}
