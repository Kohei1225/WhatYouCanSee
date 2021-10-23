using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaScript : BossBase
{
    [SerializeField] private int firstHp = 3;       //パンダの体力
    [SerializeField] private float firstMoveSpeed;  //パンダのスピード
    private const float BATTLE_START_DISTANCE = 40f;  //バトルを開始する距離
    private const float DISTANCE1 = 10f;             //攻撃１をする距離
    private const float DISTANCE2 = 30f;             //攻撃２をする距離
    private float distance;
    public bool startBattle;
    private float firstYPos;
    private bool hasKick = false;

    // Start is called before the first frame update
    void Start()
    {
        this.hp = firstHp;
        this.moveSpeed = firstMoveSpeed;
        this.attackTimeInterval = 100;
        this.attackTime = attackTimeInterval;
        this.damageTimeInterval = 100;
        this.damageTime = damageTimeInterval;
        this.animController = GetComponent<Animator>();
        this.bossSize = transform.localScale.x;
        firstYPos = transform.position.y + 1f;
        //Debug.Log("attackTime:" + attackTime + " inter:" + attackTimeInterval);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Mathf.Abs(GameObject.Find("Player").transform.position.x - transform.position.x);

        //バトルが始まってたら処理する
        if(hasStartBattle || startBattle)
        {
            
            if(!dead)
            {
                Debug.Log("canAttack:" + canAttack);
                Debug.Log("attackTime:" + attackTime + " attackInt:" + attackTimeInterval);
                //攻撃にインターバルをつける。一定の時間が経つまでは攻撃できない。
                if(this.attackTime < attackTimeInterval)
                {
                    this.attackTime++;
                    Debug.Log("足してるはず");
                    //animController.SetBool("CanKick",false);
                    animController.SetBool("CanAttack",false);
                }
                else this.canAttack = true;
        
                
                if(damageTime < damageTimeInterval)damageTime++;

                //キックしたら印をつける
                if(animController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Panda_Kick"
                    && transform.position.y > firstYPos)
                    hasKick = true;

                //キックした後に着地したらアニメーションを終わらせる
                if(hasKick && transform.position.y < firstYPos)
                {
                    hasKick = false;
                    animController.SetBool("CanKick",false);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                }

                if(animController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Panda_Move")
                    TurnToPlayer();

                //ある程度プレイヤーに近づくまでジリジリ近づく
                if( (this.hp == 3 && distance > DISTANCE1) 
                || (this.hp <= 2 && distance > DISTANCE2))
                    Move();

                else if(this.canAttack)
                {
                    //引っ掻き攻撃
                    if(distance < DISTANCE1)
                        Attack1();

                    //飛び蹴り攻撃
                    else if(this.hp <= 2 && distance < DISTANCE2)
                        Attack2();
                
                    this.canAttack = false;
                    attackTime = 0;
                    //Debug.Log("attackTime:" + attackTime);
                }
            }
        }
        //一定の距離近づいたらバトルが始まる
        else if(distance < BATTLE_START_DISTANCE)
        {
            hasStartBattle = true;
            animController.SetBool("IsBattle",true);
        }
    }
        

    //通常状態
    public override void Idle()
    {
        //通常状態の処理
    }

    //移動
    public override void Move()
    {
        //Debug.Log("Move()");
        //移動の処理        
        //animController.SetBool("",true);
        if(animController.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Panda_Move")
            return;

        //移動する
        transform.Translate(-moveSpeed*dir,0f,0f);
    }

    //攻撃1(引っ掻きのみ)
    public override void Attack1()
    {
        animController.SetBool("CanKick",false);
        animController.SetBool("CanAttack",true);
    }

    //攻撃2(飛び蹴り追加)
    public override void Attack2()
    {
        if(transform.position.y > firstYPos)
            return;
        //攻撃２の処理
        animController.SetBool("CanKick",true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(-dir*600f*rb.mass,500f*rb.mass);
        
        rb.AddForce(force);
        this.canAttack = true;
        this.attackTime = this.attackTimeInterval;
    }

    //攻撃3(連続攻撃)
    public override void Attack3()
    {
        //攻撃３の処理
    }

    //ダメージ処理
    public override void Damage()
    {
        //ダメージを受けた時の処理(アニメーション再生とか)
        Debug.Log("ダメージ受けた！！残り残機:" + this.hp);
        if(this.hp == 1)this.attackTimeInterval = 10;
    }

    public override void Down()
    {
        //死ぬ処理
        Debug.Log("ダメージ。あ！死んだ！！");
        animController.SetBool("HasDown",true);
        this.dead = true;
    }
}