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
            
        }
        else
        {
            InputController.instance.eyeCollidedWithNonValidWall();
        }
    }

}
