using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PhraseBook       phraseBook;
    public GameRules        gameRules;
    public TextOption[]     textOptions;
    public Poem             poem;
    public RectTransform    timerFill;

    float passTimer;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Invoke("SetPhrases", 0.1f);

        if (gameRules.autoPass)
        {
            passTimer = gameRules.autoPassTime;
        }
        else
        {
            passTimer = 0;
        }
    }

    void Update()
    {
        if (passTimer > 0)
        {
            ChangeTime(-Time.deltaTime);
        }
    }

    void ChangeTime(float t)
    {
        passTimer += t;
        if (passTimer <= 0)
        {
            passTimer += gameRules.autoPassTime;
            OptionSelected(null);
        }

        timerFill.localScale = new Vector2(passTimer / gameRules.autoPassTime, 1.0f);
    }

    void SetPhrases()
    {
        foreach (var to in textOptions)
        {
            if (!to.HasPhrase())
            {
                to.SetPhrase(phraseBook.GetRandomPhrase());
            }
        }
    }

    public void OptionSelected(TextOption option)
    {
        if (option != null)
        {
            poem.AddPhrase(option.GetPhrase());
            option.SetPhrase(null);

            if (gameRules.autoPass)
            {
                passTimer += gameRules.timePerSelectedPhrase;

                if (passTimer > gameRules.autoPassTime) passTimer = gameRules.autoPassTime;
            }
            Invoke("SetPhrases", 1.0f);
        }
        else
        {
            poem.AddPhrase(null);
        }
    }

    public void OptionClear(TextOption option)
    {
        if (gameRules.canRemovePhrase)
        {
            option.SetPhrase(null);
            Invoke("SetPhrases", 0.5f);

            ChangeTime(-gameRules.removePhrasePenaltyTime);
        }
    }
}
