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

    AudioSource sourceContextualSource;

    public void Initialize(SkillData skill, GameObject source)
    {
        this.skill = skill;
        this.source = source;
        // try to get the audio source from the source unit
        UnitManager um = source.GetComponent<UnitManager>();
        if (um != null)
            sourceContextualSource = um.contextualSource;
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
        if (sourceContextualSource != null && skill.onStartSound)
        {
            sourceContextualSource.PlayOneShot(skill.onStartSound);
        }
        yield return new WaitForSeconds(skill.castTime);
        if (sourceContextualSource != null && skill.onEndSound)
        {
            sourceContextualSource.PlayOneShot(skill.onEndSound);
        }
        skill.Trigger(source, target);
        SetReady(false);
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
