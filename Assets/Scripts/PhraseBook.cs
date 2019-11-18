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
        public string   phrase;
        public string   splitText;
        public int      nSyllables;
        public string   lastSyllable;
        public string   lastWord;
    }

    [System.Serializable]
    public class Rhyme
    {
        public string       word;
        public List<string> words;
    }

    public string       scoreText = "Pontos";
    public string[]     goodRhymeText;
    public string[]     goodMetricText;
    public string[]     failText;

    public TextAsset                    phraseDB;
    public TextAsset                    rhymeDB;
    public List<Phrase>                 phrases;
    public List<Rhyme>                  rhymes;

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

    public Phrase GetRandomPhraseRhyme(List<string> validWords, List<Phrase> forbidden)
    {
        List<Phrase> tmp = new List<Phrase>();

        foreach (var p in phrases)
        {
            if (IsRhyme(p, validWords))
            {
                if (forbidden.IndexOf(p) == -1)
                    tmp.Add(p);
            }
        }

        if (tmp.Count == 0) return null;

        return tmp[Random.Range(0, tmp.Count)];
    }

    public Phrase GetRandomPhrase(float probSyllables, List<int> validSyllableLength, float probRhyme, List<string> validWords, List<Phrase> forbidden)
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
        else if ((p < (probSyllables + probRhyme)) && (validWords != null))
        {
            var tmp = GetRandomPhraseRhyme(validWords, forbidden);
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

    public Phrase GetRandomPhrase(List<int> validSyllableLength, List<string> validWords, List<Phrase> forbidden, int increase_range = 2)
    {
        if ((validSyllableLength == null) ||
            (validWords == null))
        {
            return GetRandomPhrase(forbidden);
        }


        List<Phrase> tmp = new List<Phrase>();

        foreach (var p in phrases)
        {
            if (validSyllableLength.IndexOf(p.nSyllables) != -1)
            {
                if (IsRhyme(p, validWords))
                {
                    if ((forbidden == null) || (forbidden.IndexOf(p) == -1))
                        tmp.Add(p);
                }
            }
        }

        if (tmp.Count == 0)
        {
            if (increase_range > 0)
            {
                List<int> vsl = new List<int>();

                foreach (var v in validSyllableLength)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        if (!vsl.Contains(i))
                            vsl.Add(v + i);
                    }
                }

                var ret = GetRandomPhrase(vsl, validWords, forbidden, increase_range - 1);

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
            var ret = GetRandomPhraseRhyme(validWords, forbidden);

            if (ret != null) return ret;

            return null;
        }

        return tmp[Random.Range(0, tmp.Count)];
    }

    List<string> GetRhymes(string word)
    {
        foreach (var r in rhymes)
        {
            if (r.word == word) return r.words;
        }

        return null;
    }

    bool IsRhyme(Phrase phrase, List<string> validWords)
    {
        string lastWord = phrase.lastWord;

        var potentialRhymes = GetRhymes(lastWord.ToLower());

        if (potentialRhymes == null) return false;

        foreach (var s in validWords)
        {
            if (potentialRhymes.IndexOf(s.ToLower()) != -1)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsRhyme(Phrase phrase, string word)
    {
        string lastWord = phrase.lastWord;

        var potentialRhymes = GetRhymes(lastWord.ToLower());

        if (potentialRhymes.IndexOf(word.ToLower()) != -1)
        {
            return true;
        }

        return false;
    }

    public string GetRandomRhymePraise(int score)
    {
        int r = Random.Range(0, goodRhymeText.Length);

        return string.Format(goodRhymeText[r], score);
    }

    public string GetRandomMetricPraise(int score)
    {
        int r = Random.Range(0, goodMetricText.Length);

        return string.Format(goodMetricText[r], score);
    }

    public string GetFailText(int r)
    {
        return failText[r];
    }

    public int GetFailCount()
    {
        return failText.Length;
    }

#if UNITY_EDITOR
    [Button("Import")]
    void Import()
    {
        phrases = new List<Phrase>();
        rhymes = new List<Rhyme>();

        string fs = phraseDB.text;
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

            i1 = metadata.IndexOf('\"', i2 + 1);
            i2 = metadata.IndexOf('\"', i1 + 1);

            if ((i2 - i1 - 1) <= 0)
            {
                Debug.Log("Failed to parse: " + valueLine);
            }

            phrase.lastSyllable = metadata.Substring(i1 + 1, i2 - i1 - 1);

            phrase.splitText = metadata.Substring(i1 + 1, i2 - i1 - 1);

            i1 = metadata.IndexOf('\"', i2 + 1);
            i2 = metadata.IndexOf('\"', i1 + 1);

            if ((i2 - i1 - 1) <= 0)
            {
                Debug.Log("Failed to parse: " + valueLine);
            }

            phrase.lastWord = metadata.Substring(i1 + 1, i2 - i1 - 1);

            i1 = metadata.IndexOf(',', i2);

            phrase.nSyllables = int.Parse(metadata.Substring(i1 + 1));

            phrases.Add(phrase);
        }

        fs = rhymeDB.text;
        fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            string valueLine = fLines[i];

            if (valueLine == "") continue;

            var tmp = valueLine.Split(':');

            string word = tmp[0];
            string rWordsString = tmp[1].Substring(1, tmp[1].Length - 2);
            
            Rhyme rtmp = new Rhyme();
            rtmp.word = word.ToLower();
            rtmp.words = new List<string>();

            if (rWordsString != "")
            {
                var rWords = rWordsString.Split(',');

                foreach (var rWord in rWords)
                {
                    int i1a = rWord.IndexOf('\'');
                    int i1b = rWord.IndexOf('\"');
                    int i1;
                    if (i1a != -1)
                    {
                        if (i1b != -1) i1 = Mathf.Min(i1a, i1b);
                        else i1 = i1a;
                    }
                    else i1 = i1b;

                    int i2a = rWord.LastIndexOf('\'');
                    int i2b = rWord.LastIndexOf('\"');
                    int i2;
                    if (i2a != -1)
                    {
                        if (i2b != -1) i2 = Mathf.Min(i2a, i2b);
                        else i2 = i2a;
                    }
                    else i2 = i2b;

                    if ((i2 - i1 - 1) > 0)
                    {
                        string rhyme = rWord.Substring(i1 + 1, i2 - i1 - 1);
                        rtmp.words.Add(rhyme.ToLower());
                    }
                    else
                    {
                        Debug.LogError("Can't parse " + valueLine + "!");
                        continue;
                    }
                }
            }

            rhymes.Add(rtmp);
        }
    }
    
    static readonly char[] Punctuation = { '.', '!', ',', '?', ' ', '_' };

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
