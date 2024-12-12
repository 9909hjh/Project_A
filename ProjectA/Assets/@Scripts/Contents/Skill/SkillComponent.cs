using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillComponent : InitBase
{
    public List<SkillBase> SkillList { get; } = new List<SkillBase>();

    Creature _owner;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public void SetInfo(Creature owner, CreatureData creatureData)
    {
        _owner = owner;

        AddSkill(creatureData.DefaultSkillId, ESkillSlot.Default); // 4가지를 초과해 스킬을 만들지 않을 것임을 기획할 떄.
        AddSkill(creatureData.EnvSkillId, ESkillSlot.Env);
        AddSkill(creatureData.SkillAId, ESkillSlot.A);
        AddSkill(creatureData.SkillBId, ESkillSlot.B);
    }

    public void AddSkill(int skillTemplateID, Define.ESkillSlot skillSlot)
    {
        if (skillTemplateID == 0)
            return;

        if (Managers.Data.SkillDic.TryGetValue(skillTemplateID, out var data) == false)
        {
            Debug.LogWarning($"AddSkill Failed {skillTemplateID}");
            return;
        }

        string className = Managers.Data.SkillDic[skillTemplateID].ClassName;

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.ClassName)) as SkillBase;
        if (skill == null)
            return;

        skill.SetInfo(_owner, skillTemplateID);

        SkillList.Add(skill);
    }

    public SkillBase GetReadySkill()
    {
        // TEMP
        return SkillList[0];
    }
}
