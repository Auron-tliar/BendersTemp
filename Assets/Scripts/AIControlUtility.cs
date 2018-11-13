using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControlUtility : MonoBehaviour
{
    private Bender _bender;

    private void Start()
    {
        _bender = GetComponent<Bender>();
    }

    private void Update()
    {
        _bender.SpeedInput = Input.GetAxisRaw("AIVertical");
        _bender.RotationSpeedInput = Input.GetAxisRaw("AIHorizontal");
        if (Input.GetButtonDown("Ability1"))
        {
            _bender.AbilitySelector = 0;
        }
        else if (Input.GetButtonDown("Ability2"))
        {

            _bender.AbilitySelector = 1;
        }
        else if(Input.GetButtonDown("Ability3"))
        {

            _bender.AbilitySelector = 2;
        }
    }
}
