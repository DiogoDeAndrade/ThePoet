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

    public bool     randomWithSameSyllablesAndRhyme = true;
    [ShowIf("randomWithSameSyllablesAndRhyme")]
    public float    randomFactorSyllableAndRhyme = 0.5f;

    [HideIf("randomWithSameSyllablesAndRhyme")]
    public bool     randomWithSameSyllables = true;
    [ShowIf(ConditionOperator.And, "randomWithSameSyllables", "notRandomWithSameSyllablesAndRhyme")]
    public float    randomFactorSyllables = 0.5f;   

    [HideIf("randomWithSameSyllablesAndRhyme")]
    public bool     randomWithRhyme = true;
    [ShowIf(ConditionOperator.And, "randomWithRhyme", "notRandomWithSameSyllablesAndRhyme")]
    public float    randomFactorRhyme = 0.5f;

    public bool     avoidDuplicates = true;

    public bool notRandomWithSameSyllablesAndRhyme()
    {
        return !randomWithSameSyllablesAndRhyme;
    }
}
