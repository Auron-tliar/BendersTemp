using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panels : MonoBehaviour {

    private Dictionary<int, GameObject> panels = new Dictionary<int, GameObject>();

    public GameObject AirBenderAbilityPanel;
    public GameObject EarthBenderAbilityPanel;
    public GameObject FireBenderAbilityPanel;
    public GameObject WaterBenderAbilityPanel;

    
    void Start () {
        AirBenderAbilityPanel.SetActive(false);
        EarthBenderAbilityPanel.SetActive(false);
        FireBenderAbilityPanel.SetActive(false);
        WaterBenderAbilityPanel.SetActive(false);
    }
	
	void Update () {
		
	}

    public int addAbilityPanel(Bender toBeControlled)
    {
        int key = 0;

        while(panels.ContainsKey(key))
        {
            key++;
        }

        GameObject addedPanel = null;

        switch (toBeControlled.BenderType)
        { 
            case Bender.BenderTypes.Air:
                addedPanel = Instantiate(AirBenderAbilityPanel);
                break;
            case Bender.BenderTypes.Earth:
                addedPanel = Instantiate(EarthBenderAbilityPanel);
                break;
            case Bender.BenderTypes.Fire:
                addedPanel = Instantiate(FireBenderAbilityPanel);
                break;
            case Bender.BenderTypes.Water:
                addedPanel = Instantiate(WaterBenderAbilityPanel);
                break;
            default:
                break;
        }
        addedPanel.transform.parent = gameObject.transform;
        addedPanel.SetActive(true);
        panels.Add(key, addedPanel);
        foreach(AbilityIcon icon in addedPanel.GetComponentsInChildren<AbilityIcon>())
        {
            icon.BoundBender = toBeControlled;
        }
        return key;
    }

    public bool removeAbilityPanel(int key)
    {
        if(!panels.ContainsKey(key))
        {
            return false;
        }

        GameObject toRemove = panels[key];
        panels.Remove(key);
        Destroy(toRemove);
        return true;
    }
}
