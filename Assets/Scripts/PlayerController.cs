using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public enum PlayerTypes
    {
        HumanMouse,
        HumanKeyBoard,
        AI
    }

    public PlayerTypes Type = PlayerTypes.AI;

    public Color PlayerColor;
    public Transform UIIconContainer;
    public List<Bender> BendersTeam;

    public GameObject IconPrefab;

    protected void Start()
    {
        var benders = transform.GetComponentsInChildren<Bender>();
        foreach (var bender in benders)
        {
            bender.Owner = this;
            BenderIconController icon = Instantiate(IconPrefab, UIIconContainer).GetComponent<BenderIconController>();
            icon.DependentBender = bender;
            bender.IconObject = icon;
            if (Type != PlayerTypes.HumanMouse)
            {
                bender.NavAgent.enabled = false;
            }
        }
    }
}
