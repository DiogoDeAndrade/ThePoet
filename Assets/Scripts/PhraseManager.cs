using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhraseManager : MonoBehaviour
{
    public static PhraseManager instance;

    public PhraseBook   phraseBook;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

}
