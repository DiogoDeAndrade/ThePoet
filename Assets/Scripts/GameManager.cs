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

    float                   passTimer;
    List<PhraseBook.Phrase> alreadyUsed = new List<PhraseBook.Phrase>();

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
                PhraseBook.Phrase p = null;

                if (gameRules.randomWithSameSyllablesAndRhyme)
                {
                    float r = Random.Range(0.0f, 1.0f);
                    if (r <= gameRules.randomFactorSyllableAndRhyme)
                    {
                        p = phraseBook.GetRandomPhrase(poem.GetSyllableLengths(), poem.GetLastSyllables(), alreadyUsed);
                    }
                }
                else
                {
                    float pSyl = 0.0f;
                    float pRhyme = 0.0f;

                    if (gameRules.randomWithSameSyllables) pSyl = gameRules.randomFactorSyllables;
                    if (gameRules.randomWithRhyme) pRhyme = gameRules.randomFactorRhyme;

                    List<int> syllableLengths = null;
                    List<string> syllables = null;

                    if (pSyl > 0.0f) syllableLengths = poem.GetSyllableLengths();
                    if (pRhyme > 0.0f) syllables = poem.GetLastSyllables();

                    if ((pSyl > 0.0f) || (pRhyme > 0.0f))
                    {
                        p = phraseBook.GetRandomPhrase(pSyl, syllableLengths, pRhyme, syllables, alreadyUsed);
                    }
                }

                if (p == null)
                {
                    p = phraseBook.GetRandomPhrase(alreadyUsed);
                }

                if (gameRules.avoidDuplicates)
                {
                    alreadyUsed.Add(p);
                }

                to.SetPhrase(p);
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
