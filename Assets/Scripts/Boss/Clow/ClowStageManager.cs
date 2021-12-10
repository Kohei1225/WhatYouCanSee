using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> カラスのステージを管理するクラス </summary>
public class ClowStageManager : MonoBehaviour
{
    /// <summary> ミラーボールの回転する子オブジェクト </summary>
    [SerializeField] GameObject _MirroBallChild = null;

    /// <summary> カラスの状態 </summary>
    public ClowScript.TaskEnum _TaskEnum;

    /// <summary> ゲームマネージャー </summary>
    [SerializeField] GameManagerScript _GameManager = null;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
