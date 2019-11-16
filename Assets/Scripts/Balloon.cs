using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Balloon : MonoBehaviour
{
    CanvasGroup     cg;
    TextMeshProUGUI text;
    float           duration;
    float           fadeInc;

    // Start is called before the first frame update
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0.0f;
        text = GetComponentInChildren<TextMeshProUGUI>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeInc > 0.0f)
        {
            cg.alpha = Mathf.Clamp01(cg.alpha + Time.deltaTime * 4.0f);
            if (cg.alpha >= 1.0f)
            {
                fadeInc = 0.0f;
            }
        }
        else if (fadeInc < 0.0f)
        {
            cg.alpha = Mathf.Clamp01(cg.alpha - Time.deltaTime * 4.0f);
            if (cg.alpha <= 0.0f)
            {
                fadeInc = 0.0f;
            }
        }
        else
        {
            duration -= Time.deltaTime;
            if ((duration < 0.0f) && (cg.alpha > 0.0f))
            {
                fadeInc = -1.0f;
            }
        }
    }

    public void SetText(string inText)
    {
        text.text = inText;
        fadeInc = 1.0f;
        duration = 1.0f;
    }
}
