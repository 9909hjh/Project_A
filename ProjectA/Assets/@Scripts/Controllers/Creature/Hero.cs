using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class Hero : Creature
{
    public bool NeedArrange { get; set; }

    public override ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
            }
        }
    }

    EHeroMoveState _heroMoveState = EHeroMoveState.None;
    public EHeroMoveState HeroMoveState
    {
        get { return _heroMoveState; }
        private set
        {
            _heroMoveState = value;
            switch (value)
            {
                case EHeroMoveState.CollectEnv:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.TargetMonster:
                    NeedArrange = true;
                    break;
                case EHeroMoveState.ForceMove:
                    NeedArrange = true;
                    break;
            }
        }
    }

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        CreatureType = ECreatureType.Hero;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        // Map
        Collider.isTrigger = true;
        RigidBody.simulated = false;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;
    }

    public Transform HeroCampDest
    {
        get
        {
            HeroCamp camp = Managers.Object.Camp;
            if (HeroMoveState == EHeroMoveState.ReturnToCamp)
                return camp.Pivot;

            return camp.Destination;
        }
    }

    #region AI

    protected override void UpdateIdle() 
    {
        /* 1. 이동 상태에서 강제로 변경
         * 2. 몬스터 공격(이거는 설정에서 공격하기 설정 고려해보자)
         *      a. 너무 멀어졌다면 강제로 돌아오기
         * 3. 주변 Env 채집
         * 4. Camp 주변으로 모이기
         */

        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.TargetMonster;
            return;
        }

        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;
        if (env != null)
        {
            Target = env;
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.CollectEnv;
            return;
        }

        if (NeedArrange)
        {
            CreatureState = ECreatureState.Move;
            HeroMoveState = EHeroMoveState.ReturnToCamp;
            return;
        }
    }

    protected override void UpdateMove() 
    {
        /* 0. 너무 멀리 떨어져 있으면 강제 이동
         * 1. 누르면 강제 이동
         * 2. 주변 몬스터 찾기
         * 3. 주변 Env 채집
         * 4. 강제로 camp모이게 하기
         */

        if (HeroMoveState == EHeroMoveState.ForcePath)
        {
            MoveByForcePath();
            return;
        }

        if (CheckHeroCampDistanceAndForcePath())
            return;

        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            EFindPathResult result = FindPathAndMoveToCellPos(HeroCampDest.position, HERO_DEFAULT_MOVE_DEPTH);
            return;
        }

        if (HeroMoveState == EHeroMoveState.TargetMonster)
        {
            // 몬스터 죽었으면 포기.
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, AttackDistance);
            return;
        }

        if (HeroMoveState == EHeroMoveState.CollectEnv)
        {
            // 몬스터가 있으면 포기.
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                HeroMoveState = EHeroMoveState.TargetMonster;
                CreatureState = ECreatureState.Move;
                return;
            }

            // Env 이미 채집했으면 포기.
            if (Target.IsValid() == false)
            {
                HeroMoveState = EHeroMoveState.None;
                CreatureState = ECreatureState.Move;
                return;
            }

            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, AttackDistance);
            return;
        }

        // camp주변으로 모이게 하기
        if (HeroMoveState == EHeroMoveState.ReturnToCamp)
        {
            Vector3 destPos = HeroCampDest.position;
            if (FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH) == EFindPathResult.Success)
                return;

            // 실패 사유 검사.
            BaseObject obj = Managers.Map.GetObject(destPos);
            if (obj.IsValid())
            {
                // 내가 그 자리를 차지하고 있다면
                if (obj == this)
                {
                    HeroMoveState = EHeroMoveState.None;
                    NeedArrange = false;
                    return;
                }

                // 다른 영웅이 멈춰있다면.
                Hero hero = obj as Hero;
                if (hero != null && hero.CreatureState == ECreatureState.Idle)
                {
                    HeroMoveState = EHeroMoveState.None;
                    NeedArrange = false;
                    return;
                }
            }
        }

        // 누르다 땠을 떄
        if (LerpCellPosCompleted)
            CreatureState = ECreatureState.Idle;
    }

    // 몇몇 히어로가 벽에 걸려 이동에 실패했을 경우 전체 이동 경로 서치
    Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();

    bool CheckHeroCampDistanceAndForcePath()
    {
        // 너무 멀어서 못 간다.
        Vector3 destPos = HeroCampDest.position;
        Vector3Int destCellPos = Managers.Map.World2Cell(destPos);
        if ((CellPos - destCellPos).magnitude <= 10)
            return false;

        if (Managers.Map.CanGo(destCellPos, ignoreObjects: true) == false)
            return false;

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destCellPos, 100);
        if (path.Count < 2)
            return false;

        HeroMoveState = EHeroMoveState.ForcePath;

        _forcePath.Clear();
        foreach (var p in path)
        {
            _forcePath.Enqueue(p);
        }
        _forcePath.Dequeue();

        return true;
    }

    void MoveByForcePath()
    {
        if (_forcePath.Count == 0)
        {
            HeroMoveState = EHeroMoveState.None;
            return;
        }

        Vector3Int cellPos = _forcePath.Peek();

        if (MoveToCellPos(cellPos, 2))
        {
            _forcePath.Dequeue();
            return;
        }

        // 실패 사유가 영웅이라면.
        Hero hero = Managers.Map.GetObject(cellPos) as Hero;
        if (hero != null && hero.CreatureState == ECreatureState.Idle)
        {
            HeroMoveState = EHeroMoveState.None;
            return;
        }
    }

    protected override void UpdateSkill() 
    {
        base.UpdateSkill();

        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            CreatureState = ECreatureState.Move;
            return;
        }

        if (Target.IsValid() == false)
        {
            CreatureState = ECreatureState.Move;
            return;
        }
    }

    protected override void UpdateDead() 
    {
    }

    
    #endregion

    private void HandleOnJoystickStateChanged(EJoystickState joystickState)
    {
        switch (joystickState)
        {
            case Define.EJoystickState.PointerDown:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.Drag:
                HeroMoveState = EHeroMoveState.ForceMove;
                break;
            case Define.EJoystickState.PointerUp: // 케릭터 인공지능 상태
                HeroMoveState = EHeroMoveState.None;
                break;
            default:
                break;
        }
    }

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);

    }
}
