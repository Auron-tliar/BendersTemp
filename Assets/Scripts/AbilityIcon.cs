using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AbilityIcon : MonoBehaviour, IPointerClickHandler
{
    [Range(0,2)]
    public int AbilityNumber = 0;

    [HideInInspector]
    public Bender BoundBender;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(BoundBender.State != Bender.States.Idle && BoundBender.State != Bender.States.Moving)
        {
            return;
        }

        switch (AbilityNumber)
        {
            case 0:
                BoundBender.StartAbility(Bender.States.Casting1, 0);
                break;
            case 1:
                BoundBender.StartAbility(Bender.States.Casting2, 1);
                break;
            case 2:
                BoundBender.StartAbility(Bender.States.Casting3, 2);
                break;
            default:
                break;
        }
        BoundBender.Owner.GetComponent<HumanController>().ClickHandled = true;
    }
}
