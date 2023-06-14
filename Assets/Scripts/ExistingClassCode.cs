using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExistingClassCode : MonoBehaviour
{
    public TMP_Text classHeader;
    private string classCode;

    void Start()
    {
        classCode = StudentGenerator.ClassCode;
        classHeader.text = "You’re already part of Class "+classCode+", would you like to enter a new Class code?";
    }
}
