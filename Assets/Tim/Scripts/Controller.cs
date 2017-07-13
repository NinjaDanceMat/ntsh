using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    private SteamVR_TrackedController _controller;

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
        InputController.instance.TriggerReleased();
    }

    public void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        InputController.instance.TriggerShoot();
    }

    public void HandlePadClicked(object sender, ClickedEventArgs e)
    {
        InputController.instance.ButtonBoop();
    }
    public void HandleTriggerHeld()
    {
        InputController.instance.TriggerHeld();
    }

}
