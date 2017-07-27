using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    private SteamVR_TrackedController _controller;

    public bool isLeft;

    private void Awake()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += HandleTriggerClicked;
        _controller.TriggerUnclicked += HandleTriggerReleased;
        _controller.PadClicked += HandlePadClicked;
    }

    public void Update()
    {
        if (_controller.triggerPressed)
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