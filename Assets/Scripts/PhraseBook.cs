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
    public struct Phrase
    {
        public string phrase;
        public string splitText;
        public int nSyllables;
    }

    public TextAsset    originalText; 
    public List<Phrase> phrases;

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

            phrases.Add(phrase);
        }
    }
#endif
}
