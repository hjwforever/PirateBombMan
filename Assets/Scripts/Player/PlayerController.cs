using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IDmageable
{
    private Rigidbody2D rb;
    private Animator anim;
    private FixedJoystick joystick;
    private VariableJoystick variableJoystick;

    public float speed = 5;
    public float jumpFrorce = 15;
    private float horizontalInput_keyboard;
    private float horizontalInput;

    private bool invincible = false;

    private enum Direction
    {
        left = -1, right = 1
    }

    [Header("Player State")]
    public float maxHealth;
    public float health;
    public bool isDead;
    private bool isHurt = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("States Check")]
    public bool isGround;
    public bool canJump;
    public bool isJump;

    [Header("Jump FX")]
    public GameObject jumpFX;
    public GameObject landFX;
    public GameObject runFX;

    [Header("Attack Settings")]
    public GameObject bombPrefab;
    public float nextAttack;
    public float bombCD;
    public float attackRate;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //  joystick = FindObjectOfType<FixedJoystick>();
         variableJoystick = FindObjectOfType<VariableJoystick>();

        bombCD = attackRate;

        GameManager.GetInstance().SetPlayer(this);

        GameManager.GetInstance().Load(); // 加载玩家数据
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            anim.SetBool("dead", isDead);
            return; 
        }

        isHurt = anim.GetAnimatorTransitionInfo(1).IsName("Player_Hit");

        CheckInput();
        CheckBombCD();
    }

    public void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        PhysicsCheck();

        // 非受伤状态下进行移动或跳跃
        if (!isHurt)
        {
            Movement();
            Jump();
        }       
    }

    void CheckInput()
    {
        if(Input.GetButtonDown("Jump") && isGround)
        {
            canJump = true;
        }

        if(Input.GetKeyDown(KeyCode.J))
        {
            Attack();
        }
    }

    public void CheckBombCD()
    {
        if (Time.time < nextAttack)
        {
            bombCD = nextAttack - Time.time;
        }
        else bombCD = 0;
    }

    void Movement()
    {
        /*       
           // 键盘操作
           horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 ~ 1 包含小数的过渡，即反向速度由小变大
           // 触屏操作
           horizontalInput = joystick.Horizontal; // -1 到 1
        */


         // horizontalInput = Input.GetAxisRaw("Horizontal") != 0 ? Input.GetAxisRaw("Horizontal") : joystick.Horizontal;  // 优先键盘操作, 再判断触屏操作
         horizontalInput = Input.GetAxisRaw("Horizontal") != 0 ? Input.GetAxisRaw("Horizontal") : variableJoystick.Horizontal;  // 优先键盘操作, 再判断触屏操作

        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        if(horizontalInput != 0)
        {
            // transform.localScale = new Vector3(horizontalInput > 0 ? 1 : -1, 1, 1);  //使用localScale 翻转朝向
            transform.eulerAngles = new Vector3(0, horizontalInput > 0 ? 0 : 180, 0);  //使用Rotation 翻转朝向
        }
    }

    void Jump()
    {
        if(canJump)
        {
            isJump = true;
/*            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0,-0.75f,0);*/
            rb.velocity = new Vector2(rb.velocity.x, jumpFrorce);
            canJump = false;
        }
    }

    // 点击按钮跳跃
    public void ButtonJump()
    {
        if (isGround)
        {
            canJump = true;
        }
    }

        public void Attack()
    {
        if(Time.time > nextAttack)
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);
            nextAttack = Time.time + attackRate;
        }
    }

    void PhysicsCheck()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGround)
        {
            rb.gravityScale = 1;
            isJump = false;
        }
        else
        {
            rb.gravityScale = 4;
        }
    }

    public void JumpFX()
    {
        jumpFX.SetActive(true);
        jumpFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    public void LandFX()
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    public void RunFX()
    {
        runFX.SetActive(true);
        runFX.transform.localScale = new Vector3(horizontalInput > 0 ? 1 : -1, 1, 1);
        runFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    public void GetHit(float damage)
    {
        if(!anim.GetCurrentAnimatorStateInfo(1).IsName("Player_Hit") && !invincible)
        {
            health -= damage;
            if (health < 1)
            {
                health = 0;
                
                isDead = true;
            }
            anim.SetTrigger("hit");

            UIManager.GetInstance().UpdateHealth(maxHealth, health);

            if(health>0)
                Debug.Log("玩家收到伤害，伤害值:"+damage+", 当前生命值: " +health);
            else
            {
                Debug.Log("玩家死亡");
                GameManager.GetInstance().gameOver = true;
            }
        }
    }   

    // 玩家死亡
    public void Dead()
    {
        // Debug.Log("玩家死亡");
        GameManager.GetInstance().gameOver = true;
        UIManager.GetInstance().ShowGameOverUI(isDead);
    }

    public void Revive()
    {
        health = maxHealth;
        isDead = false;
        anim.SetBool("dead", false);
        UIManager.GetInstance().UpdateHealth(maxHealth, maxHealth);
        UIManager.GetInstance().ShowGameOverUI(isDead);

        StartCoroutine(Invincible()); // 无敌时间
    }

    // 短暂时间的无敌效果(并不被敌人注意), 默认为3秒
    IEnumerator Invincible(float totalTime = 3f,float delatTime = 0.2f)
    {
        Color c1 = GetComponent<SpriteRenderer>().color;
        Color c2 = GetComponent<SpriteRenderer>().color;
        c1.a = 0.7f;
        c2.a = 1f;

        float time = 0f;

        GameManager.GetInstance().gameOver = true;
        invincible = true;
        while (time < totalTime)
        {
            GetComponent<SpriteRenderer>().color = c1;
            yield return new WaitForSeconds(delatTime);

            GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(delatTime);

            time += 2*delatTime;
        }
        GetComponent<SpriteRenderer>().color = c2;
        GameManager.GetInstance().gameOver = false;
        invincible = false;
    }
}
