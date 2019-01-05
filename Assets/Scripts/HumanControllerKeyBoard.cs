using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanControllerKeyBoard : HumanController
{
    public float speed = 3.0f;
    public float rotationSpeed = 100f;
    public Bender bender;

    private int abilityPanelKey = 0;

    void Start()
    {
        base.Start();
        bender.Owner = this;


        abilityPanelKey = Panels.GetComponent<Panels>().addAbilityPanel(bender);
    }


    // Update is called once per frame
    void Update () {
        bender.SpeedInput = Input.GetAxisRaw("KeyboardVertical");
        bender.RotationSpeedInput = Input.GetAxisRaw("KeyboardHorizontal");
        if (Input.GetKey(KeyCode.W))
        {
            bender.transform.position += bender.transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            bender.transform.position -= bender.transform.forward * Time.deltaTime * speed;
        }
    }
}
