using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        SceneType = Define.EScene.TitleScene;

        StartLoadAssets();

        return true;
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalcount) =>
        {
            Debug.Log($"{key} {count}/{totalcount}");
            if(count == totalcount)
            {
                // 메니져서에서 초기화.
            }
        });
    }
}
