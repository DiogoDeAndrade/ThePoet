using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Poem : MonoBehaviour
{
    public TextMeshProUGUI  textPrefab;
    public float            scrollSpeed = 100.0f;
    public float            scrollTime = 0.5f;
    public float            alphaSpeed = 1.0f;
    public float            alphaFadeLimit = 950.0f;
    public Sprite[]         poetSprites;
    public Image            poetImage;
            
    List<int>       valid_syllable_lengths;
    List<string>    valid_last_words;
    GameRules       gameRules;
    PhraseBook      phraseBook;
    float           spriteSwitchTime;

    struct PoemPhrase
    {
        public PhraseBook.Phrase phrase;
        public RectTransform displayRectTransform;
        public TextMeshProUGUI displayText;
    }

    List<PoemPhrase>    poem = new List<PoemPhrase>();

    float scrollTimer = 0;
    float midPointY;

    void Start()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        midPointY = rect.center.y;

        gameRules = GameManager.instance.gameRules;
        phraseBook = GameManager.instance.currentPhraseBook;
    }

    void Update()
    {
        scrollTimer -= Time.deltaTime;
        if (scrollTimer > 0)
        {
            // Scroll text
            foreach (var phrase in poem)
            {
                Vector2 pos = phrase.displayRectTransform.anchoredPosition;
                pos.y += Time.deltaTime * scrollSpeed;
                phrase.displayRectTransform.anchoredPosition = pos;

                Color c = phrase.displayText.color;
                if ((c.a < 1) && (pos.y < alphaFadeLimit))
                {
                    c.a = Mathf.Clamp01(c.a + alphaSpeed * Time.deltaTime);
                    phrase.displayText.color = c;
                }
                else if ((c.a > 0) && (pos.y > alphaFadeLimit))
                {
                    c.a = Mathf.Clamp01(c.a - alphaSpeed * Time.deltaTime);
                    phrase.displayText.color = c;
                }
            }

            spriteSwitchTime -= Time.deltaTime;
            if (spriteSwitchTime < 0.0f)
            {
                poetImage.sprite = poetSprites[Random.Range(0, poetSprites.Length)];
                spriteSwitchTime = 0.1f;
            }
        }
        else
        {
            scrollTimer = 0.0f;
            poetImage.sprite = poetSprites[0];
        }
    }

    public void AddPhrase(PhraseBook.Phrase phrase)
    {
        if (phrase == null)
        {
            if (poem.Count == 0)
            {
                GameManager.instance.LooseLife();
                return;
            }

            var lastPhrase = poem[poem.Count - 1];
            if (lastPhrase.phrase == null)
            {
                GameManager.instance.LooseLife();
                return;
            }
        }

        PoemPhrase pp = new PoemPhrase();
        pp.displayText = Instantiate(textPrefab, transform);
        pp.displayRectTransform = pp.displayText.GetComponent<RectTransform>();

        float posY = 0;
        if (poem.Count > 0)
        {
            posY = poem[poem.Count - 1].displayRectTransform.anchoredPosition.y - scrollSpeed * scrollTime;
        }

        pp.displayRectTransform.anchoredPosition = new Vector2(0, posY);

        Color c = pp.displayText.color;
        pp.displayText.color = new Color(c.r, c.g, c.b, 0.0f);
        pp.displayText.text = (phrase != null)?(phrase.phrase):("");

        pp.phrase = phrase;

        poem.Add(pp);

        scrollTimer += scrollTime;

        if (phrase != null)
        {
            if (valid_syllable_lengths == null) valid_syllable_lengths = new List<int>();

            if (valid_syllable_lengths.IndexOf(phrase.nSyllables) == -1)
            {
                valid_syllable_lengths.Add(phrase.nSyllables);
            }

            if (valid_last_words == null) valid_last_words = new List<string>();

            if (valid_last_words.IndexOf(phrase.lastWord) == -1)
            {
                valid_last_words.Add(phrase.lastWord);
            }

            var stanza = GetCurrentStanza();

            float rhymeScore = 0;
            if (stanza.Count >= 2)
            {
                string currentLastWord = stanza[0].lastWord;

                // Get first rhyme
                int count = -1;
                for (int i = 1; i < stanza.Count; i++)
                {
                    if (phraseBook.IsRhyme(stanza[i], currentLastWord))
                    {
                        count = i;
                        break;
                    }
                }

                if (count != -1)
                {
                    float   currentBonus = 1.0f;

                    // Compute rhyme bonus
                    for (int i = count; i < stanza.Count; i++)
                    {
                        if (phraseBook.IsRhyme(stanza[i], currentLastWord))
                        {
                            rhymeScore = gameRules.scoreRhyme * currentBonus * count;
                            currentBonus += gameRules.bonusRhyme;
                        }
                        else
                        {
                            rhymeScore = 0;
                            break;
                        }
                    }

                    if (rhymeScore > 0)
                    {
                        if (gameRules.allowScoreRhyme) GameManager.instance.ChangeScore(GameManager.ScoreType.Rhyme, rhymeScore, 0.0f);
                    }
                }

            }

            float metricScore = 0.0f;

            if (stanza.Count >= 2)
            {
                int currentSylCount = stanza[0].nSyllables;

                // Get first rhyme
                int count = -1;
                for (int i = 1; i < stanza.Count; i++)
                {
                    if (Mathf.Abs(stanza[i].nSyllables - currentSylCount) <= 1)
                    {
                        count = i;
                        break;
                    }
                }

                if (count != -1)
                {
                    float currentBonus = 1.0f;

                    // Compute rhyme bonus
                    for (int i = count; i < stanza.Count; i++)
                    {
                        if (Mathf.Abs(stanza[i].nSyllables - currentSylCount) <= 1)
                        {
                            metricScore = gameRules.scoreMetric * currentBonus * count;
                            currentBonus += gameRules.bonusMetric;
                        }
                        else
                        {
                            metricScore = 0;
                            break;
                        }
                    }

                    if (metricScore > 0)
                    {
                        if (gameRules.allowScoreMetric) GameManager.instance.ChangeScore(GameManager.ScoreType.Metric, metricScore, 0.0f);                            
                    }
                }
            }

            if ((metricScore == 0.0f) && (rhymeScore == 0.0f) && (stanza.Count > 2))
            {
                if (gameRules.failOnNoMetricAndNoRhyme)
                {
                    GameManager.instance.LooseLife();
                }
            }
            else
            {
                GameManager.instance.Clap();
            }
        }
    }

    public List<PhraseBook.Phrase> GetCurrentStanza()
    {
        List<PhraseBook.Phrase> ret = new List<PhraseBook.Phrase>();

        for (int i = poem.Count - 1; i >= 0; i--)
        {
            if (poem[i].phrase == null) break;

            ret.Add(poem[i].phrase);
        }

        return ret;
    }

    public List<int> GetSyllableLengths()
    {
        return valid_syllable_lengths;
    }

    public List<string> GetLastWords()
    {
        return valid_last_words;
    }
}
