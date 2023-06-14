using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClassCodeSubmitted : MonoBehaviour
{
    public TMP_Text classHeader;
    private string classCode;

    void Start()
    {
        classCode = StudentGenerator.ClassCode;
        classHeader.text = "You are now part of class "+classCode+"!";
    }
}
