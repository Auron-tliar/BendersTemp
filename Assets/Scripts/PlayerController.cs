using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public enum PlayerTypes
    {
        Human,
        AI
    }

    public PlayerTypes Type = PlayerTypes.AI;

    public Color PlayerColor;
    public Transform UIIconContainer;
    public List<Bender> BendersTeam;

    public GameObject IconPrefab;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Bender bender = transform.GetChild(i).GetComponent<Bender>();
            bender.Owner = this;
            BenderIconController icon = Instantiate(IconPrefab, UIIconContainer).GetComponent<BenderIconController>();
            icon.DependentBender = bender;
            bender.IconObject = icon;
        }

    }
}
