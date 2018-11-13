using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : PlayerController
{
    

    public LayerMask SelectionLayer;
    public LayerMask MovementLayer;

    public GameObject ClickMark;

    public GameObject AirBenderAbilityPanel;
    public GameObject EarthBenderAbilityPanel;
    public GameObject FireBenderAbilityPanel;
    public GameObject WaterBenderAbilityPanel;

    private Bender _selection;
    private RaycastHit hit;

    [HideInInspector]
    public bool ClickHandled = false;


    public Bender Selection
    {
        get
        {
            return _selection;
        }

        set
        {
            if (_selection != null)
            {
                _selection.GotDeselected();
                AirBenderAbilityPanel.SetActive(false);
                EarthBenderAbilityPanel.SetActive(false);
                FireBenderAbilityPanel.SetActive(false);
                WaterBenderAbilityPanel.SetActive(false);
            }
            _selection = value;
            if (_selection != null)
            {
                _selection.GotSelected();
                switch (_selection.BenderType)
                {
                    case Bender.BenderTypes.Air:
                        AirBenderAbilityPanel.SetActive(true);
                        for (int i = 0; i < AirBenderAbilityPanel.transform.childCount; i++)
                        {
                            AirBenderAbilityPanel.transform.GetChild(i).GetComponent<AbilityIcon>().BoundBender =
                                _selection;
                        }
                        break;
                    case Bender.BenderTypes.Earth:
                        EarthBenderAbilityPanel.SetActive(true);
                        for (int i = 0; i < EarthBenderAbilityPanel.transform.childCount; i++)
                        {
                            EarthBenderAbilityPanel.transform.GetChild(i).GetComponent<AbilityIcon>().BoundBender =
                                _selection;
                        }
                        break;
                    case Bender.BenderTypes.Fire:
                        FireBenderAbilityPanel.SetActive(true);
                        for (int i = 0; i < FireBenderAbilityPanel.transform.childCount; i++)
                        {
                            FireBenderAbilityPanel.transform.GetChild(i).GetComponent<AbilityIcon>().BoundBender =
                                _selection;
                        }
                        break;
                    case Bender.BenderTypes.Water:
                        WaterBenderAbilityPanel.SetActive(true);
                        for (int i = 0; i < WaterBenderAbilityPanel.transform.childCount; i++)
                        {
                            WaterBenderAbilityPanel.transform.GetChild(i).GetComponent<AbilityIcon>().BoundBender =
                                _selection;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void Update()
    {
        if (ClickHandled)
        {
            ClickHandled = false;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, SelectionLayer) &&
                hit.transform.tag == "Bender")
            {
                Bender bender = hit.transform.GetComponent<Bender>();
                if (bender.Owner == this)
                {
                    Selection = bender;
                }
            }
            else
            {
                Selection = null;
            }
        }
        else if (Input.GetMouseButtonUp(1) && _selection != null)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, MovementLayer))
            {
                Instantiate(ClickMark, hit.point, new Quaternion());
                _selection.NavAgent.SetDestination(hit.point);
            }
        }
    }
}