using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum ScoreType { Phrase, Metric, Rhyme };

    public static GameManager instance;

    public PhraseBook       phraseBook;
    public GameRules        gameRules;
    public TextOption[]     textOptions;
    public Poem             poem;
    public RectTransform    timerFill;
    public TextMeshProUGUI  scoreText;

    float                   passTimer;
    List<PhraseBook.Phrase> alreadyUsed = new List<PhraseBook.Phrase>();
    float                   score = 0;
    float                   bonus = 1.0f;

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

        UpdateScore();
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
                        p = phraseBook.GetRandomPhrase(poem.GetSyllableLengths(), poem.GetLastWords(), alreadyUsed);
                    }
                }
                else
                {
                    float pSyl = 0.0f;
                    float pRhyme = 0.0f;

                    if (gameRules.randomWithSameSyllables) pSyl = gameRules.randomFactorSyllables;
                    if (gameRules.randomWithRhyme) pRhyme = gameRules.randomFactorRhyme;

                    List<int> syllableLengths = null;
                    List<string> words = null;

                    if (pSyl > 0.0f) syllableLengths = poem.GetSyllableLengths();
                    if (pRhyme > 0.0f) words = poem.GetLastWords();

                    if ((pSyl > 0.0f) || (pRhyme > 0.0f))
                    {
                        p = phraseBook.GetRandomPhrase(pSyl, syllableLengths, pRhyme, words, alreadyUsed);
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

            if (gameRules.allowScorePhrase) ChangeScore(ScoreType.Phrase, gameRules.scorePhrase, 0.0f);

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

    public void ChangeScore(ScoreType type, float inScore, float inBonus)
    {
        float s = inScore * bonus;

        score += s;

        bonus += inBonus;

        Debug.Log("Add score " + s + "(" + type + ")");

        UpdateScore();
    }

    public void ResetBonus()
    {
        bonus = 1.0f;
    }

    void UpdateScore()
    {
        scoreText.text = phraseBook.scoreText + ": " + Mathf.FloorToInt(score);
    }
}
