﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOption : MonoBehaviour
{
    public string   phrase;
    public Material normalMaterial;
    public Material highlightMaterial;

    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        UpdateMaterial(false);
    }

    void UpdateMaterial(bool highlight)
    {
        Material material = (highlight) ? (highlightMaterial) : (normalMaterial);
        text.text = "<material=\"" + material.name + "\">" + phrase;
    }

    public void OnEnter()
    {
        UpdateMaterial(true);
    }

    public void OnExit()
    {
        UpdateMaterial(false);
    }
}