using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BenderIconController : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI Name;
    public Image Portrait;
    public Image ColoredBackground;
    public GameObject RemovedMask;
    public GameObject SelectionBox;

    public Bender DependentBender
    {
        get
        {
            return _dependentBender;
        }

        set
        {
            _dependentBender = value;
            ColoredBackground.color = _dependentBender.Owner.PlayerColor;
            Portrait.sprite = _dependentBender.PortraitSprite; 
        }
    }

    private Bender _dependentBender;

    public void Start()
    {
        Name.text = DependentBender.Name;
        ColoredBackground.color = DependentBender.Owner.PlayerColor;
    }

    public void GotRemoved()
    {
        RemovedMask.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dependentBender.Owner.Type == PlayerController.PlayerTypes.HumanMouse)
        {
            HumanControllerMouse hc = _dependentBender.Owner.GetComponent<HumanControllerMouse>();
            hc.Selection = _dependentBender;
            hc.ClickHandled = true;
        }
    }
}
