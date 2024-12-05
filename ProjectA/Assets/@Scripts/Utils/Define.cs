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

    public enum EJoystickState
    {
        PointerDown,
        PointerUp,
        Drag,
    }

    // 임시 코드
    public const int CAMERA_PROJECTION_SIZE = 12;

    public const int HERO_WIZARD_ID = 201000;
    public const int HERO_KNIGHT_ID = 201001;

    public const int MONSTER_SLIME_ID = 202001;
    public const int MONSTER_SPIDER_COMMON_ID = 202002;
    public const int MONSTER_WOOD_COMMON_ID = 202004;
    public const int MONSTER_GOBLIN_ARCHER_ID = 202005;
    public const int MONSTER_BEAR_ID = 202006;

    public const int ENV_TREE1_ID = 300001;
    public const int ENV_TREE2_ID = 301000;
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

public static class SortingLayers
{
    public const int SPELL_INDICATOR = 200;
    public const int CREATURE = 300;
    public const int ENV = 300;
    public const int PROJECTILE = 310;
    public const int SKILL_EFFECT = 310;
    public const int DAMAGE_FONT = 410;
}