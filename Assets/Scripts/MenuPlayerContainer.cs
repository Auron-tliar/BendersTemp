using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlayerContainer : MonoBehaviour
{
    [Range(0, 1)]
    public int PlayerNumber;
    public Color PlayerColor;
    public Transform Container;
    public GameObject Catcher;
    public Dropdown PlayerType;

    public int MaxBenders = 4;

    private List<Bender.BenderTypes> _bendersList = new List<Bender.BenderTypes>();

    private bool _isCatching;

    public bool IsCatching
    {
        get
        {
            return _isCatching;
        }

        set
        {
            _isCatching = value;
            Catcher.SetActive(_isCatching);
        }
    }

    public void AddBender(MenuBenderSelector bender)
    {
        if (_bendersList.Count < MaxBenders)
        {
            bender.transform.parent = Container;
            _bendersList.Add(bender.Type);
            bender.SetColor(PlayerColor);
            Debug.Log(PlayerColor);
        }
        else
        {
            Destroy(bender.gameObject);
        }
    }

    public void RemoveBender(int index)
    {
        _bendersList.RemoveAt(index);
    }

    public List<Bender.BenderTypes> GetBendersList()
    {
        return _bendersList;
    }
}
