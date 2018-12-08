using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        List<Bender> benders = GetComponentsInChildren<Bender>().ToList();
        for (int i = 0; i < benders.Count; i++)
        {
            Bender bender = benders[i];
            bender.Owner = this;

            if (Type == PlayerTypes.HumanKeyBoard)
            {
                BenderIconController icon = Instantiate(IconPrefab, UIIconContainer).GetComponent<BenderIconController>();
                icon.DependentBender = bender;
                bender.IconObject = icon;
                bender.NavAgent.enabled = false;
            }
        }
    }
}
