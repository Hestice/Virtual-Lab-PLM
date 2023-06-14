using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestCompleted : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text testSubmittedSub;
    private Animator BGanimator;

    void Awake(){
        testSubmittedSub.text = "You have completed the quick test on "+ButtonGeneratorScores.equipmentAssessment +" and have submitted on time, Well done!";
    }
}
