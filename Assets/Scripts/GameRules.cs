using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
#endif
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ThePoet/GameRules")]
public class GameRules : ScriptableObject
{
    public bool     autoPass = true;
    [ShowIf("autoPass")]
    public float    autoPassTime = 5.0f;
    [ShowIf("autoPass")]
    public float    timePerSelectedPhrase = 2.5f;

    public bool     canRemovePhrase = true;
    [ShowIf("canRemovePhrase")]
    public float    removePhrasePenaltyTime = 1.0f;
}
