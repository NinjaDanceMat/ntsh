using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eye : MonoBehaviour
{
    public LayerMask validWall;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 8)
        {
            InputController.instance.eyeCollidedWithValidWall();
            SoundManager.instance.TriggerClip(2);
        }
        else
        {           
            InputController.instance.eyeCollidedWithNonValidWall();
            if (col.gameObject.layer != 2)
            {
                if (col.gameObject.layer == 9)
                {
                    SoundManager.instance.TriggerClip(1);
                }
                else
                {
                    SoundManager.instance.TriggerClip(0);
                }
            }
        }
    }

}
