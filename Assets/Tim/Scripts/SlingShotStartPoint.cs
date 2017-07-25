using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShotStartPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 11)
        {
            if (InputController.instance.clutchingEye)
            {
                InputController.instance.inSlingShot();
            }
        }
    }
}