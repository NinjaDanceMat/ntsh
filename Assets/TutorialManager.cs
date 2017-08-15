using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public List<string> messages = new List<string>();

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
        text.text = messages[index];
        currentTutorial = index;
    }
}
