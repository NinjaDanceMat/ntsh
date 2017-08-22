using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    public List<string> messages = new List<string>();
    public List<bool> hasBeenSeen = new List<bool>();

    public Text text;

    public int currentTutorial;

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
        if (index == 1)
        {
            numberOfTimesThrown += 1;

            if (numberOfTimesThrown >= 3)
            {
                index = 2;
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

    public IEnumerator delayedNotification()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.instance.TriggerClip(11);
    }
}
