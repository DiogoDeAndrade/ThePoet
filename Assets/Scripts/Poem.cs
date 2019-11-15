using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Poem : MonoBehaviour
{
    public TextMeshProUGUI  textPrefab;
    public float            scrollSpeed = 100.0f;
    public float            scrollTime = 0.5f;
    public float            alphaSpeed = 1.0f;
    public float            alphaFadeLimit = 950.0f;

    List<int>       valid_syllable_lengths;
    List<string>    valid_last_syllables;

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
        }
        else
        {
            scrollTimer = 0.0f;
        }
    }

    public void AddPhrase(PhraseBook.Phrase phrase)
    {
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

        if (valid_syllable_lengths == null) valid_syllable_lengths = new List<int>();

        if (valid_syllable_lengths.IndexOf(phrase.nSyllables) == -1)
        {
            valid_syllable_lengths.Add(phrase.nSyllables);
        }

        if (valid_last_syllables == null) valid_last_syllables = new List<string>();

        if (valid_last_syllables.IndexOf(phrase.lastSyllable) == -1)
        {
            valid_last_syllables.Add(phrase.lastSyllable);
        }
    }

    public List<int> GetSyllableLengths()
    {
        return valid_syllable_lengths;
    }

    public List<string> GetLastSyllables()
    {
        return valid_last_syllables;
    }
}
