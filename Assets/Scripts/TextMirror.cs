using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextMirror : MonoBehaviour
{
    private void Start()
    {
        TextMeshProUGUI[] temp = GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Log(temp.Length);
        foreach (TextMeshProUGUI t in temp)
        {
            t.rectTransform.Rotate(Vector3.up, 180f);
        }
    }
}
