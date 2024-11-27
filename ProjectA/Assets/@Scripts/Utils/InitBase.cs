using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitBase : MonoBehaviour
{
    protected bool _init = false;

    public virtual bool Init() // 한번이라도 초기화를 했다면 false를 반환
    {
        if(_init) 
            return false;

        _init = true;
        return true;
    }

    private void Awake()
    {
        Init();
    }
}
