using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanControllerMouse : HumanController
{
    public LayerMask SelectionLayer;
    public LayerMask MovementLayer;

    public GameObject ClickMark;

    [HideInInspector]
    public bool ClickHandled = false;

    private Bender _selection;
    private RaycastHit hit;

    private int abilityPanelKey = 99999;

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
                Debug.Log(Panels.GetComponent<Panels>().removeAbilityPanel(abilityPanelKey));
            }
            _selection = value;
            if (_selection != null)
            {
                if (!_selection.isSelected())
                {
                    _selection.GotSelected();
                    abilityPanelKey = Panels.GetComponent<Panels>().addAbilityPanel(_selection);
                }

            }
        }
    }

    // Update is called once per frame
    void Update () {
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
                if (!_selection.NavAgent.isStopped)
                {
                    _selection.NavAgent.SetDestination(hit.point);
                }
            }
        }
    }
}
