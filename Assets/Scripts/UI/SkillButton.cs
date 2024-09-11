using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SkillData skillData;

    public void Initialize(SkillData skillData)
    {
        this.skillData = skillData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.TriggerEvent("HoverSkillButton", this.skillData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventManager.TriggerEvent("UnhoverSkillButton");
    }
}