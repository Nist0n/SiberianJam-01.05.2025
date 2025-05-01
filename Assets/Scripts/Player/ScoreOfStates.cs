using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreOfStates : MonoBehaviour
{
    [SerializeField] private Text _textHapiness;
    [SerializeField] private Text _textLearning;
    
    public int _hapinessScore;
    public int _learningScore;
    
    private void Update()
    {
        _textHapiness.text = $"{PlayerPrefs.GetInt("hapinessScore")}";
        _textLearning.text = $"{PlayerPrefs.GetInt("learningScore")}";
    }
}
