using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextMirror : MonoBehaviour
{
    private void Start()
    {
        //TextMeshProUGUI[] temp;
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<TextMeshProUGUI>().rectTransform.Rotate(Vector3.up, 180f);
            //temp = GetComponentsInChildren<TextMeshProUGUI>();
            //foreach (TextMeshProUGUI t in temp)
            //{
            //    t.rectTransform.Rotate(Vector3.up, 180f);
            //}
        }
    }
}
