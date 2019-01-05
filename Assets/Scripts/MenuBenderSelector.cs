using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuBenderSelector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Bender.BenderTypes Type;
    public Image RaycastZone;
    public Image Background;

    [HideInInspector]
    public bool SelectionMenu = true;

    private MenuController _menuController;
    private Vector2 _holdDelta;

    private void Start()
    {
        _menuController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MenuController>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SelectionMenu)
        {
            GameObject temp = Instantiate(gameObject, transform.parent, true);
            temp.transform.SetSiblingIndex(transform.GetSiblingIndex());
            SelectionMenu = false;
        }
        else
        {
            transform.parent.parent.GetComponent<MenuPlayerContainer>().RemoveBender(transform.GetSiblingIndex());
        }
        transform.parent = transform.parent.parent.parent;
        transform.SetSiblingIndex(int.MaxValue);
        RaycastZone.raycastTarget = false;
        _menuController.IsDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = transform.position + new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MenuPlayerContainer container = 
            eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<MenuPlayerContainer>();
        Debug.Log(eventData.pointerCurrentRaycast.gameObject);
        if (container != null)
        {
            container.AddBender(this);
            RaycastZone.raycastTarget = true;
        }
        else
        {
            Destroy(gameObject);
        }
        _menuController.IsDragging = false;
    }

    public void SetColor(Color color)
    {
        Background.color = color;
    }
}
