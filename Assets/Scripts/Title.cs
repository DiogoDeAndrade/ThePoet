using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SelectLanguage("PT");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Transition.HideGame("GameScene");
    }

    public void Exit()
    {
        Transition.HideGame("<QUIT>");
    }

    public void SelectLanguage(string str)
    {
        Transition.SetProperty("Language", str);
    }
}
