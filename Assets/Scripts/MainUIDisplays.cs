using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUIDisplays : MonoBehaviour
{
    private TMP_Text currentEquipmentHeader;
    public static string[] Equipment = {"Sphygmomanometer", "Pulse Oximeter", "Intramuscular Injection", "Subcutaneous Injection", "Intradermal Injection", "Walker", "Axillary Crutch", "Forearm Crutch", "Cane"};

    void Awake(){
        currentEquipmentHeader = GameObject.Find("EquipmentHeader").GetComponent<TMP_Text>();
    }

    void Start(){
        currentEquipmentHeader.text = Equipment[ButtonGenerator.activeButtonIndex];
    }
}
