using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    public List<string> messages = new List<string>();
    public List<bool> hasBeenSeen = new List<bool>();

    public Text text;
    public GameObject scaler;
    public GameObject scalerPos;
    public int currentTutorial;

    public bool slingshotInsteadOfThrow;
    public bool tutorialEnd;

    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public static TutorialManager instance;
    void Awake()
    {
        if (TutorialManager.instance == null)
        {
            TutorialManager.instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    public int numberOfTimesThrown;

    public void SetTutorial(int index)
    {
        if (!tutorialEnd)
        {
            if (index == 1)
            {
                if (slingshotInsteadOfThrow)
                {
                    index = 8;
                }
                else
                {
                    numberOfTimesThrown += 1;

                    if (numberOfTimesThrown >= 3)
                    {
                        index = 2;
                    }
                }
            }

            if (!hasBeenSeen[index])
            {
                hasBeenSeen[index] = true;
                StartCoroutine(delayedNotification());
            }
            text.text = messages[index];
            currentTutorial = index;
        }
        else
        {
            text.text = messages[7];
            currentTutorial = 7;
        }
    }

    public IEnumerator delayedNotification()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.instance.TriggerClip(11);
        StartCoroutine(InputController.instance.FlashTut(15));
    }



    public void Update()
    {
        Vector3 targetPosition = scalerPos.transform.position;
        scaler.transform.position = Vector3.Lerp(scaler.transform.position, targetPosition, 0.3f);
        //scaler.transform.position = targetPosition;
        if (Vector3.Angle(InputController.instance.head.transform.forward, scaler.transform.position - InputController.instance.head.transform.position) < 30)
        {
            scaler.transform.localScale = new Vector3(2.5f,2.5f,2.5f);
        }
        else
        {
            scaler.transform.localScale = new Vector3(1, 1, 1);
        }

    }
}
