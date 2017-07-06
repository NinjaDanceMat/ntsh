using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSource : MonoBehaviour
{
    public SteamVR_TrackedObject trackedObj;

    public SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        //InputController.instance.AddInputControllerDevice(Controller, trackedObj.index);
    }
}
