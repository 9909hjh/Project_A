using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        Managers.Map.LoadMap("BaseMap");
        Managers.Map.StageTransition.SetInfo();

        HeroCamp camp = Managers.Object.Spawn<HeroCamp>(Vector3.zero, 0);
        camp.SetCellPos(new Vector3Int(0, 0, 0), true);

        for (int i = 0; i < 3; i++)
        {
            //int heroTemplateID = HERO_WIZARD_ID + Random.Range(0, 5);
            int heroTemplateID = HERO_KNIGHT_ID;
            //int heroTemplateID = HERO_LION_ID;

            Vector3Int randCellPos = new Vector3Int(0 + Random.Range(-3, 3), 0 + Random.Range(-3, 3), 0);
            if (Managers.Map.CanGo(null, randCellPos) == false)
                continue;

            Hero hero = Managers.Object.Spawn<Hero>(new Vector3Int(1, 0, 0), heroTemplateID);
            //hero.ExtraCells = 1;
            //hero.SetCellPos(randCellPos, true);
            Managers.Map.MoveTo(hero, randCellPos, true);
        }

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        {
            //Monster monster = Managers.Object.Spawn<Monster>(new Vector3Int(0, 1, 0), MONSTER_BEAR_ID);
            //Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_SLIME_ID);
            //Managers.Object.Spawn<Monster>(new Vector3(2, 1, 0), MONSTER_GOBLIN_ARCHER_ID);
            Monster monster = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_BEAR_ID);
            monster.ExtraCells = 1;
            Managers.Map.MoveTo(monster, new Vector3Int(0, 4, 0), true);

        }

        {
            //Env env = Managers.Object.Spawn<Env>(new Vector3(0, 2, 0), ENV_TREE1_ID);
            //env.EnvState = EEnvState.Idle;
        }


        // Todo - inGame

        return true;
    }

    public override void Clear()
    {
        
    }
}
