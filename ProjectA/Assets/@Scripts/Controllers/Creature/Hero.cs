using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Hero : Creature
{
    public override ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;

                //if (value == ECreatureState.Move)
                //    RigidBody.mass = CreatureData.Mass;
                //else
                //    RigidBody.mass = CreatureData.Mass * 0.1f;
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
                    //NeedArrange = true;
                    break;
                case EHeroMoveState.TargetMonster:
                    //NeedArrange = true;
                    break;
                case EHeroMoveState.ForceMove:
                    //NeedArrange = true;
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


    }

    protected override void UpdateMove() 
    {
        if (HeroMoveState == EHeroMoveState.ForceMove)
        {
            Vector3 dir = HeroCampDest.position - transform.position;
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            return;
        }


    }

    protected override void UpdateSkill() 
    { 
    
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
}
