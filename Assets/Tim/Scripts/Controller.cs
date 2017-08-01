using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public SteamVR_TrackedController controller;

    public bool isLeft;

    private void Awake()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        controller.TriggerClicked += HandleTriggerClicked;
        controller.TriggerUnclicked += HandleTriggerReleased;
        controller.PadClicked += HandlePadClicked;

        if (isLeft)
        {
            InputController.instance.leftHandInputController = this;
        }
        else
        {

            InputController.instance.rightHandInputController = this;
        }
    }

    public void Update()
    {
        if (controller.triggerPressed)
        {
            HandleTriggerHeld();
        }
    }

    public void HandleTriggerReleased(object sender, ClickedEventArgs e)
    {
        if (!isLeft && !GameVariables.instance.leftHandMode ||
            (isLeft && GameVariables.instance.leftHandMode))
        {
            InputController.instance.TriggerReleased();
        }
       
    }

    public void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (!isLeft && !GameVariables.instance.leftHandMode ||
            (isLeft && GameVariables.instance.leftHandMode))
        {
            InputController.instance.TriggerShoot();
        }
    }

    public void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        if (!isLeft && !GameVariables.instance.leftHandMode ||
             (isLeft && GameVariables.instance.leftHandMode))
        {
            InputController.instance.ButtonBoop();
        }
    }
    public void HandleTriggerHeld()
    {
        if (!isLeft && !GameVariables.instance.leftHandMode ||
            (isLeft && GameVariables.instance.leftHandMode))
        {
            InputController.instance.TriggerHeld();
        }
    }
}