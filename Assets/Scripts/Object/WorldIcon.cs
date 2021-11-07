using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldIcon : StageIcon
{
    //次のステージに進むアイコンか
    //戻るならfalse
    public bool isNext = true;

    public bool Get_isNext()
    {
        return isNext;
    }
}
