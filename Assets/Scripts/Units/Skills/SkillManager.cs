using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public SkillData skill;
    private GameObject source;
    Button triggerButton;
    bool ready;

    public void Initialize(SkillData skillData, GameObject source)
    {
        this.skill = skillData;
        this.source = source;
    }

    public void Trigger(GameObject target = null)
    {
        if (!ready) return;
        StartCoroutine(WrappedTrigger(target));
    }

    public void SetButton(Button b)
    {
        this.triggerButton = b;
        SetReady(true);
    }

    private IEnumerator WrappedTrigger(GameObject target)
    {
        SetReady(false);

        yield return new WaitForSeconds(skill.castTime);
        skill.Trigger(source, target);
        yield return new WaitForSeconds(skill.cooldown);
        SetReady(true);
    }

    private void SetReady(bool r)
    {
        this.ready = r;
        if (triggerButton != null)
        {
            triggerButton.interactable = ready;
        }
    }
}
