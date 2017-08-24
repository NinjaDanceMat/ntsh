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
    bool Blinking;
    Vector3 oldTransform;
    Vector3 oldoldTransform;
    public GameObject visual;

    public float speed;

    public void OnEnable()
    {
        Blinking = false;
        visual.transform.localScale = new Vector3(0,0,0);
    }
    public void OnDisable()
    {
        Blinking = false;
        visual.transform.localScale = new Vector3(0, 0, 0);
    }

    bool blinkingUp;
    public void Update()
    {

        if (Vector3.Distance(oldoldTransform, transform.position) < 0.02f)
        {
            Blinking = true;

        }
        else
        {
        }
        if (Blinking == true)
        {
            if (InputController.instance.recallingEye)
            {
                Blinking = false;
                visual.transform.localScale = new Vector3(0, 0, 0);
            }
        }
        if (Blinking)
        {
            if (visual.transform.localScale.magnitude > 3*Vector3.Distance(transform.position,InputController.instance.head.transform.position) && blinkingUp)
            {
                blinkingUp = false;
            }
            if (visual.transform.localScale.magnitude <= 1.5f && !blinkingUp)
            {
                blinkingUp = true;
            }
            float speedA = speed*Vector3.Distance(transform.position, InputController.instance.head.transform.position);
            if (blinkingUp)
            {
                visual.transform.localScale += new Vector3(speedA, speedA, speedA);
            }
            else
            {
                visual.transform.localScale -= new Vector3(speedA, speedA, speedA);
            }
        }
        oldoldTransform = oldTransform;
        oldTransform = transform.position;
    }
}
