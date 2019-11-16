using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum ScoreType { Phrase, Metric, Rhyme };

    public static GameManager instance;

    public PhraseBook[]     phraseBooks;
    public PhraseBook       currentPhraseBook;
    public GameRules        gameRules;
    public TextOption[]     textOptions;
    public Poem             poem;
    public RectTransform    timerFill;
    public TextMeshProUGUI  scoreText;
    public Balloon[]        rhymeScore;
    public Balloon[]        metricScore;
    public Balloon[]        fail;
    public Image            poetImage;
    public Sprite           poetDejected;
    public AudioClip        clapAudio;
    public float            clapProb = 0.1f;
    public AudioClip        smallBoo;
    public float            booProb = 0.5f;
    public AudioClip        bigBoo;

    float                   passTimer;
    List<PhraseBook.Phrase> alreadyUsed = new List<PhraseBook.Phrase>();
    float                   score = 0;
    float                   bonus = 1.0f;
    int                     fails = 0;
    Coroutine               gameOverCR;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        string language = Transition.GetProperty("Language");
        foreach (var pb in phraseBooks)
        {
            if (pb.name == language)
            {
                currentPhraseBook = pb;
                break;
            }
        }

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
                        p = currentPhraseBook.GetRandomPhrase(poem.GetSyllableLengths(), poem.GetLastWords(), alreadyUsed);
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
                        p = currentPhraseBook.GetRandomPhrase(pSyl, syllableLengths, pRhyme, words, alreadyUsed);
                    }
                }

                if (p == null)
                {
                    p = currentPhraseBook.GetRandomPhrase(alreadyUsed);
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
        if (gameOverCR != null) return;

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

        switch (type)
        {
            case ScoreType.Rhyme:
                {
                    // Choose random rhyme balloon
                    Balloon balloon = rhymeScore[Random.Range(0, rhymeScore.Length)];
                    balloon.SetText(currentPhraseBook.GetRandomRhymePraise(Mathf.FloorToInt(s)));
                }
                break;
            case ScoreType.Metric:
                {
                    // Choose random rhyme balloon
                    Balloon balloon = metricScore[Random.Range(0, metricScore.Length)];
                    balloon.SetText(currentPhraseBook.GetRandomMetricPraise(Mathf.FloorToInt(s)));
                }
                break;
        }

        UpdateScore();
    }

    public void ResetBonus()
    {
        bonus = 1.0f;
    }

    void UpdateScore()
    {
        scoreText.text = currentPhraseBook.scoreText + ": " + Mathf.FloorToInt(score);
    }

    public void LooseLife()
    {
        if (fails >= currentPhraseBook.GetFailCount())
        {
            // Game Over!
            poem.gameObject.SetActive(false);
            poetImage.sprite = poetDejected;

            SoundManager.PlaySound(SoundManager.SoundType.SoundFX, bigBoo);

            gameOverCR = StartCoroutine(GameOverCR());
        }
        else
        {
            Balloon balloon = fail[Random.Range(0, fail.Length)];
            balloon.SetText(currentPhraseBook.GetFailText(fails));

            if (Random.Range(0.0f, 1.0f) < booProb)
            {
                SoundManager.PlaySound(SoundManager.SoundType.SoundFX, smallBoo);
            }

            fails++;
        }
    }

    public void Clap()
    {
        if (Random.Range(0.0f, 1.0f) < clapProb)
        {
            SoundManager.PlaySound(SoundManager.SoundType.SoundFX, clapAudio, Random.Range(0.8f, 1.0f), Random.Range(0.8f, 1.2f));
        }
    }

    IEnumerator GameOverCR()
    {
        yield return new WaitForSeconds(1.0f);

        float tElapsed = 0.0f;
        var   rt = poetImage.GetComponent<RectTransform>();
        var   startPos = rt.anchoredPosition;
        var   pos = startPos;

        while (tElapsed < 3.0f)
        {
            if ((tElapsed < 2.0f) && ((tElapsed + Time.deltaTime) > 2))
            {
                Transition.HideGame("Title");
            }
            tElapsed += Time.deltaTime;

            float s = Mathf.Abs(Mathf.Sin(tElapsed * 12.0f));
            pos.x -= 500.0f * Time.deltaTime;
            pos.y = startPos.y + 30.0f * s;
            rt.anchoredPosition = pos;

            yield return null;
        }
    }
}
