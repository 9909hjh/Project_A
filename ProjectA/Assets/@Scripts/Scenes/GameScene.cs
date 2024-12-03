using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = EScene.GameScene;

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

        Hero hero = Managers.Object.Spawn<Hero>(Vector3.zero);
        hero.CreatureState = ECreatureState.Idle;

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = hero;


        Managers.UI.ShowBaseUI<UI_Joystick>();

        // Todo - inGame

        return true;
    }

    public override void Clear()
    {
        
    }
}
