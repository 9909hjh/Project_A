using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EObjectType
    {
        None,
        Creature,
        Projectile,
        Env,
    }

    public enum ECreatureType
    {
        None,
        Hero,
        Monster,
        Npc,
    }

    public enum ECreatureState
    {
        None,
        Idle,
        Move,
        Skill,
        Dead
    }

}

// 애니메이션 이름은 기획자 혹은 애니메이터와 상의
public static class AnimName
{
    public const string IDLE = "idle";
    public const string ATTACK_A = "attack_a";
    public const string ATTACK_B = "attack_b";
    public const string MOVE = "move";
    public const string DEAD = "dead";
}