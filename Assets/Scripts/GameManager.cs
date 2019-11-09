using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PhraseBook   phraseBook;
    public TextOption[] textOptions;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Invoke("SetPhrases", 0.5f);
    }

    void SetPhrases()
    {
        foreach (var to in textOptions)
        {
            if (to.phrase == "")
            {
                to.phrase = phraseBook.GetRandomPhrase();
                to.ForceUpdate();
            }
        }
    }
}
