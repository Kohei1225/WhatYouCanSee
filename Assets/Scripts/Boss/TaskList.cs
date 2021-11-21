using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// タスクという単位で処理を登録して、
/// TaskTypeを追加することで順番に処理を実行するクラス.
///
/// [使い方]
///	Class className
///	{
///		
///		①タスクの種類の定義
///		enum TaskEnum
///		{
///			Idle,
///			Jump,
///			Damage,
///		}
///
///		②インスタンス生成
///		TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();
///
///		void Start()
///		{
///			④タスクの登録
///			_TaskList.DefineTask(TaskEnum.Idle, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
///			_TaskList.DefineTask(TaskEnum.Jump, TaskJumpEnter, TaskJumpUpdate, TaskJumpExit);
///			_TaskList.DefineTask(TaskEnum.Damage, TaskDamageEnter, TaskDamageeUpdate, TaskDamageExit);
///		}
///
///		void Update()
///		{
///			⑤タスクが全部終わったら、実行するタスクを追加する
///			(この場合は Idleの処理 -> Jumpの処理の順番で処理を行う)
///			if(_TaskList.IsEnd)
///			{	
///				_TaskList.AddTask(TaskEnum.Idle);
///				_TaskList.AddTask(TaskEnum.Jump);
///			}
///			
///			⑥タスクの処理を呼び出す
///			_TaskList.UpdateTask();
///		}
///
///		③タスクに応じた処理を書く
///		void TaskIdleEnter()
///		{
///			最初だけ呼び出す処理
///		}
///
///		bool TaskIdleUpdate()
///		{
///			trueを返すまで呼び出し続ける処理
///		}
///
///		void TaskIdleExit()
///		{
///			最後に呼び出す処理
///		}
///		...
///	}
/// 
/// </summary>
/// <typeparam name="T">enumの型</typeparam>
public class TaskList<T>
{
	#region define
	/// <summary> ３つの関数を登録する内部関数 </summary>
	private class Task
	{
		public T TaskType;
		public Action Enter { get; set; }
		public Func<bool> Update { get; set; }//戻り値あり(trueなら処理を終える)
		public Action Exit { get; set; }

		/// <summary>コンストラクタ</summary>
        /// <param name="t">タスクenumの型</param>
        /// <param name="enter">最初の一回だけ実行する関数</param>
        /// <param name="update">繰り返し実行する関数</param>
        /// <param name="exit">最後の一回だけ実行する関数</param>
		public Task(T t, Action enter, Func<bool> update, Action exit)
		{
			TaskType = t;
			Enter = enter;
			// updateの中身がnullであればtrueを返すメソッドを格納する
			Update = update ?? delegate { return true; };
			Exit = exit;
		}
	}
	#endregion

	#region field
	/// <summary> 定義されたタスク </summary>
	Dictionary<T, Task> _DefineTaskList = new Dictionary<T, Task>();
	/// <summary> 今貯まってるタスクたち </summary>
	List<Task> _CurrentTaskList = new List<Task>();
	/// <summary> 今動いてるタスク </summary>
	Task _CurrentTask = null;
	/// <summary> 現在のIndex番号 </summary>
	int _CurrentIndex = 0;
	#endregion

	#region property
	/// <summary>追加されたタスクが全部終了してるか</summary>
	public bool IsEnd
	{
		get { return _CurrentTaskList.Count <= _CurrentIndex; }
	}

	/// <summary>タスクが動いてるか</summary>
	public bool IsMoveTask
	{
		get { return _CurrentTask != null; }
	}

	/// <summary>現在のインデックス</summary>
	public int CurrentIndex
	{
		get { return _CurrentIndex; }
	}
	#endregion

	#region public function
	/// <summary>毎フレーム呼ばれる処理</summary>
	public void UpdateTask()
	{
		// 追加されてるタスクがなければ何もしない
		if (IsEnd)
		{
			return;
		}

		// 現在のタスクがなければ、タスクを取得する
		if (_CurrentTask == null)
		{
			_CurrentTask = _CurrentTaskList[_CurrentIndex];
			// Enterを呼ぶ
			_CurrentTask.Enter?.Invoke();
		}

		// タスクのUpdateを呼ぶ
		var isEndOneTask = _CurrentTask.Update();

		// タスクが終了していれば次の処理を呼ぶ
		if (isEndOneTask)
		{
			// 現在のタスクのExitを呼ぶ
			_CurrentTask?.Exit();

			// Index追加
			_CurrentIndex++;

			// タスクをnullにする
			_CurrentTask = null;

			// 追加されてるタスクがなければクリアする
			if (IsEnd)
			{
				_CurrentIndex = 0;
				_CurrentTaskList.Clear();
				return;
			}
		}
	}

	/// <summary>タスクの定義</summary>
	/// <param name="t">タスクenumの型</param>
	/// <param name="enter">最初だけ実行する関数</param>
	/// <param name="update">処理を繰り返し実行する関数</param>
	/// <param name="exit">最後に一回だけ実行する関数</param>
	public void DefineTask(T t, Action enter, Func<bool> update, Action exit)
	{
		//タスクのインスタンスを作成してListに登録する
		var task = new Task(t, enter, update, exit);
		var exist = _DefineTaskList.ContainsKey(t);
		if (exist)
		{
			return;
		}
		_DefineTaskList.Add(t, task);
	}

	/// <summary>タスクの登録をするメソッド</summary>
	/// <param name="t">タスクenumの型</param>
	public void AddTask(T t)
	{
		Task task = null;
		//定義されたタスクかどうかを調べて、登録(追加)する
		var exist = _DefineTaskList.ContainsKey(t);
		if (!exist)
		{
			Debug.Log(t + "は未定義のタスクです。");
			return;
		}
		task = _DefineTaskList[t];
		_CurrentTaskList.Add(task);
	}

	/// <summary>強制終了するメソッド</summary>
	public void CancelAllTask()
	{
		if (_CurrentTask != null)
		{
			_CurrentTask.Exit();
		}
		_CurrentTask = null;
		_CurrentTaskList.Clear();
		_CurrentIndex = 0;
	}
	#endregion
}
