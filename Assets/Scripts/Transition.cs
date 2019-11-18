using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Image[]  curtains;
    public float    speed = 0.1f;

    float                       inc = 0.0f;
    float                       val = 1.0f;
    string                      nextScreen;
    Dictionary<string, string>  properties;

    static Transition instance;
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        nextScreen = "";

        if (SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene("Title", LoadSceneMode.Additive);
            ShowGame();
        }
        else
        {
            ShowGame();
        }
    }

    void Update()
    {
        if (inc != 0.0f)
        {
            val = Mathf.Clamp01(val + inc * Time.deltaTime * speed);
            if (((val <= 0.0f) && (inc < 0.0f)) ||
                ((val >= 1.0f) && (inc > 0.0f)))
            {
                inc = 0.0f;

                if (val >= 1.0f)
                {
                    if (nextScreen != "")
                    {
                        // Close all scenes except this one
                        StartCoroutine(ChangeScreenCR());
                    }
                }
            }

            foreach (var c in curtains)
            {
                var rt = c.GetComponent<RectTransform>();

                float alpha = 2.0f;
                float s = Mathf.Pow(val, alpha);
                s = s / (s + Mathf.Pow(1 - val, alpha));
                
                rt.localScale = new Vector2(s, 1.0f);
            }
        }        
    }

    IEnumerator ChangeScreenCR()
    {
        while (SceneManager.sceneCount > 1)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name != "Transition")
                {
                    SceneManager.UnloadSceneAsync(scene);
                    break;
                }
            }
            yield return null;
        }        

        yield return new WaitForSeconds(1.0f);

        if (nextScreen == "<QUIT>")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(nextScreen, LoadSceneMode.Additive);
            ShowGame();

            nextScreen = "";
        }
    }

    public static void ShowGame()
    {
        instance.inc = -1.0f;
    }

    public static void HideGame(string nextScreen)
    {
        instance.inc = 1.0f;
        instance.nextScreen = nextScreen;
    }

    public static void SetProperty(string key, string val)
    {
        if (instance.properties == null) instance.properties = new Dictionary<string, string>();

        if (instance.properties.ContainsKey(key))
        {
            instance.properties[key] = val;
        }
        else
        {
            instance.properties.Add(key, val);
        }
    }

    public static string GetProperty(string key)
    {
        if (instance.properties == null) return "";

        if (instance.properties.ContainsKey(key))
            return instance.properties[key];

        return "";
    }
}
