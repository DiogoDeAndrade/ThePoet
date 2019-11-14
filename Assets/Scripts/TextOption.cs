using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextOption : MonoBehaviour, IPointerClickHandler
{
    public Material normalMaterial;
    public Material highlightMaterial;

    TextMeshProUGUI text;
    bool highlight;
    PhraseBook.Phrase phrase;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        UpdateMaterial(false);
    }

    void UpdateMaterial(bool highlight)
    {
        string s = (phrase != null) ? (phrase.phrase) : ("");

        Material material = (highlight) ? (highlightMaterial) : (normalMaterial);
        text.text = "<material=\"" + material.name + "\">" + s;
        this.highlight = highlight;
    }

    public void OnEnter()
    {
        UpdateMaterial(true);
    }

    public void OnExit()
    {
        UpdateMaterial(false);
    }

    void ForceUpdate()
    {
        UpdateMaterial(highlight);
    }

    public void SetPhrase(PhraseBook.Phrase phrase)
    {
        this.phrase = phrase;
        ForceUpdate();
    }

    public bool HasPhrase()
    {
        return phrase != null;
    }

    public PhraseBook.Phrase GetPhrase()
    {
        return phrase;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Detected right click on this, call the GameManager
        GameManager.instance.OptionClear(this);
    }
}
