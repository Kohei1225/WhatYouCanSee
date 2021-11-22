using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    public enum TaskEnum
    {
        IDLE,
        WALK,
        TONGUE_ATTACK,
        TRANSPARENT,
        DAMAGE,
        WAIT
    }
    #endregion

    #region field
    /// <summary> 最大HP </summary>
    [SerializeField] private int _MaxHP = 9;

    /// <summary> 歩くスピード </summary>
    [SerializeField] private float _WalkSpeed = 1;

    /// <summary> プレイヤー </summary>
    [SerializeField] private GameObject _Player = null;

    /// <summary> タスク管理 </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();

    /// <summary> 時間計測用 </summary>
    private TimerScript _TimerScript = new TimerScript();

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //HP設定
        _CurrentHP = _MaxHP;

        _TaskList.DefineTask(TaskEnum.IDLE, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
    }

    // Update is called once per frame
    void Update()
    {
        _TaskList.UpdateTask();
    }

    #region タスク関数
    /// <summary> 待機処理
    private void TaskIdleEnter()
    {
        //アニメーションは待機に
    }

    private bool TaskIdleUpdate()
    {
        return true;
    }

    private void TaskIdleExit()
    {

    }

    /// <summary> 歩き処理
    private void TaskWalkEnter()
    {
        //アニメーションは歩くに
    }

    private void TaskWalkUpdate()
    {
        //プレイヤーの
    }
    #endregion

    #region BoseBaseからのOverride関数
    public override void Attack1()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack2()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack3()
    {
        throw new System.NotImplementedException();
    }

    public override void Damage()
    {
        throw new System.NotImplementedException();
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }

    public override void Idle()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void Wait(float waitTime)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
