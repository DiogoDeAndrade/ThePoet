using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
using NaughtyAttributes;
#endif

[CreateAssetMenu(menuName = "ThePoet/PhraseBook")]
public class PhraseBook : ScriptableObject
{
    [System.Serializable]
    public class Phrase
    {
        public string phrase;
        public string splitText;
        public int nSyllables;
        public string lastSyllable;
    }

    public TextAsset originalText;
    public List<Phrase> phrases;

    public Phrase GetRandomPhrase(List<Phrase> forbidden)
    {
        while (true)
        {
            int r = Random.Range(0, phrases.Count);

            if (forbidden.IndexOf(phrases[r]) == -1) return phrases[r];
        }
    }

    public Phrase GetRandomPhraseSyllables(List<int> validSyllableLength, List<Phrase> forbidden)
    {
        List<Phrase> tmp = new List<Phrase>();

        foreach (var p in phrases)
        {
            if (validSyllableLength.IndexOf(p.nSyllables) != -1)
            {
                if (forbidden.IndexOf(p) == -1)
                    tmp.Add(p);
            }
        }

        if (tmp.Count == 0) return null;

        return tmp[Random.Range(0, tmp.Count)];
    }

    public Phrase GetRandomPhraseRhyme(List<string> validSyllables, List<Phrase> forbidden)
    {
        List<Phrase> tmp = new List<Phrase>();

        foreach (var p in phrases)
        {
            if (IsRhyme(p, validSyllables))
            {
                if (forbidden.IndexOf(p) == -1)
                    tmp.Add(p);
            }
        }

        if (tmp.Count == 0) return null;

        return tmp[Random.Range(0, tmp.Count)];
    }

    public Phrase GetRandomPhrase(float probSyllables, List<int> validSyllableLength, float probRhyme, List<string> validSyllables, List<Phrase> forbidden)
    {
        float m = probSyllables + probRhyme;
        if (m < 1.0f) m = 1.0f;

        float p = Random.Range(0.0f, m);

        if ((p < probSyllables) && (validSyllableLength != null))
        {
            var tmp = GetRandomPhraseSyllables(validSyllableLength, forbidden);
            if (tmp != null)
            {
                return tmp;
            }
        }
        else if ((p < (probSyllables + probRhyme)) && (validSyllables != null))
        {
            var tmp = GetRandomPhraseRhyme(validSyllables, forbidden);
            if (tmp != null)
            {
                return tmp;
            }
        }

        while (true)
        {
            int r = Random.Range(0, phrases.Count);

            if (forbidden.IndexOf(phrases[r]) == -1) return phrases[r];
        }
    }

    public Phrase GetRandomPhrase(List<int> validSyllableLength, List<string> validSyllables, List<Phrase> forbidden, bool increase_range = true)
    {
        if ((validSyllableLength == null) ||
            (validSyllables == null))
        {
            return GetRandomPhrase(forbidden);
        }


        List<Phrase> tmp = new List<Phrase>();

        foreach (var p in phrases)
        {
            if (validSyllableLength.IndexOf(p.nSyllables) != -1)
            {
                if (IsRhyme(p, validSyllables))
                {
                    if ((forbidden == null) || (forbidden.IndexOf(p) == -1))
                        tmp.Add(p);
                }
            }
        }

        if (tmp.Count == 0)
        {
            if (increase_range)
            {
                List<int> vsl = new List<int>();

                foreach (var v in validSyllableLength)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        vsl.Add(v + i);
                    }
                }

                var ret = GetRandomPhrase(vsl, validSyllables, forbidden, false);

                if (ret != null) return ret;
            }
            else
            {
                return null;
            }
        }

        if (tmp.Count == 0)
        {
            // Try to get something with the same syllable length
            var ret = GetRandomPhraseSyllables(validSyllableLength, forbidden);

            if (ret != null) return ret;
        }

        if (tmp.Count == 0)
        {
            // Try to get something with the same syllable length
            var ret = GetRandomPhraseRhyme(validSyllables, forbidden);

            if (ret != null) return ret;

            return null;
        }

        return tmp[Random.Range(0, tmp.Count)];
    }

    bool IsRhyme(Phrase phrase, List<string> validSyllables)
    {
        foreach (var s in validSyllables)
        {
            if (IsRhyme(phrase, s))
            {
                return true;
            }
        }

        return false;
    }

    bool IsRhyme(Phrase phrase, string syllable)
    {
        var lastSyllable = phrase.lastSyllable;

        if (lastSyllable == syllable) return true;

        int match = 0;
        int s = Mathf.Min(syllable.Length, lastSyllable.Length);
        for (int i = 0; i < s; i++)
        {
            if (syllable[syllable.Length - i - 1] == lastSyllable[lastSyllable.Length - i - 1])
            {
                match++;
            }
            else break;
        }

        if (match >= 2) return true;

        return false;
    }

#if UNITY_EDITOR
    [Button("Import")]
    void Import()
    {
        phrases = new List<Phrase>();

        string fs = originalText.text;
        string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];

            if (valueLine == "") continue;

            Phrase phrase = new Phrase();

            int i1 = valueLine.IndexOf('(');
            int i2 = valueLine.IndexOf(')');

            phrase.phrase = valueLine.Substring(i2 + 1);

            string metadata = valueLine.Substring(i1 + 1, i2 - i1 - 1);

            i1 = metadata.IndexOf('\"');
            i2 = metadata.IndexOf('\"', i1 + 1);

            if ((i2 - i1 - 1) <= 0)
            {
                Debug.Log("Failed to parse: " + valueLine);
            }

            phrase.splitText = metadata.Substring(i1 + 1, i2 - i1 - 1);

            i1 = metadata.IndexOf(',', i2);

            phrase.nSyllables = int.Parse(metadata.Substring(i1 + 1));

            phrase.lastSyllable = GetLastSyllable(phrase.splitText);

            phrases.Add(phrase);
        }
    }

    static string GetLastSyllable(string splitText)
    {
        int index = splitText.LastIndexOf('-');
        if (index == -1) return RemovePunctuation(splitText);

        return RemovePunctuation(splitText.Substring(index + 1));
    }

    static readonly char[] Punctuation = { '.', '!', ',', '?', ' ' };

    static string RemovePunctuation(string sentence)
    {
        while (true)
        {
            int idx = sentence.IndexOfAny(Punctuation);
            if (idx != -1)
            {
                sentence = sentence.Remove(idx, 1);
            }
            else break;
        }

        return sentence;
    }
#endif
}
