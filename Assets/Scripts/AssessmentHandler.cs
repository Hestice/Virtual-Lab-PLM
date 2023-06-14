using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Proyecto26;
using Newtonsoft.Json;
using FullSerializer;

public class AssessmentHandler : MonoBehaviour
{     
    public TMP_Text detail_classCode;
    public TMP_Text detail_faculty;
    public TMP_Text detail_datePosted;
    public TMP_Text detail_dueDate;
    public TMP_Text detail_closingTime;
    public TMP_Text detail_description;
    public TMP_Text detail_equipment;
 
    public TMP_Text Instructions;

    public GameObject questionTemplate1;
    public GameObject questionTemplate2;
    public GameObject questionTemplate3;
    public GameObject questionTemplate4;
    public Transform parentTransform;

    private int numbering = 1;
    public TMP_Text testTitle;

    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";

    private void Start()
    {
        // Retrieve data from Firebase for the specified identifier
        RetrieveEquipmentAssessmentData(ButtonGeneratorScores.equipmentAssessment);
    }
    
    private void RetrieveEquipmentAssessmentData(string assessmentIndex)
    {
        RestClient.Get(databaseURL + "equipmentAssessments/" + assessmentIndex + ".json").Then(response =>
        {
            var equipmentAssessment = JsonConvert.DeserializeObject<EquipmentAssessmentData.EquipmentAssessment>(response.Text);

            if (equipmentAssessment != null)
            {
                RestClient.Get(databaseURL + "equipmentAssessments/" + assessmentIndex + "/questions.json").Then(questionsResponse =>
                {
                    var questions = JsonConvert.DeserializeObject<List<EquipmentAssessmentData.Question>>(questionsResponse.Text);

                    if (questions != null)
                    {
                        // Log assessment details
                        testTitle.text = equipmentAssessment.equipment;
                        Instructions.text = equipmentAssessment.instructions;
                        Debug.Log("Assessment Identifier: " + assessmentIndex);
                        Debug.Log("Class Code: " + equipmentAssessment.classCode);
                        Debug.Log("Closing Date Time: " + equipmentAssessment.closingDateTime);
                        detail_classCode.text = equipmentAssessment.classCode;
                        detail_equipment.text = equipmentAssessment.equipment;
                        detail_description.text = equipmentAssessment.description;
                        detail_faculty.text = equipmentAssessment.faculty;
                        detail_dueDate.text = equipmentAssessment.dueDateTime.ToString();
                        detail_datePosted.text = equipmentAssessment.datePosted.ToString();
                        detail_closingTime.text = equipmentAssessment.closingDateTime.ToString();

                        // Log question count
                        Debug.Log("Question Count: " + questions.Count);

                        foreach (var question in questions)
                        {
                            // Instantiate question template based on question type
                            GameObject questionTemplate = null;

                            switch (question.questionType)
                            {
                                case 1:
                                    questionTemplate = Instantiate(questionTemplate1, parentTransform);
                                    break;
                                case 2:
                                    questionTemplate = Instantiate(questionTemplate2, parentTransform);
                                    break;
                                case 3:
                                    questionTemplate = Instantiate(questionTemplate3, parentTransform);
                                    break;
                                case 4:
                                    questionTemplate = Instantiate(questionTemplate4, parentTransform);
                                    break;
                                default:
                                    Debug.LogWarning("Unknown question type: " + question.questionType);
                                    break;
                            }

                            // Customize the instantiated question template based on the question data
                            if (questionTemplate != null)
                            {
                                switch (question.questionType)
                                {
                                    case 1: //Essay
                                        questionTemplate.transform.Find("Question/Heading").GetComponent<TMP_Text>().text = numbering + ". " + question.question;
                                        break;
                                    case 2: //Identification
                                        questionTemplate.transform.Find("Question/Heading").GetComponent<TMP_Text>().text = numbering + ". " + question.question;
                                        // Customize question template for question type 2
                                        break;
                                    case 3: // MultipleChoice
                                    questionTemplate.transform.Find("Container/Question/Heading").GetComponent<TMP_Text>().text = numbering + ". " + question.question;

                                    RestClient.Get(databaseURL + "equipmentAssessments/" + assessmentIndex + "/questions/" + (numbering - 1).ToString() + "/multipleChoice/options.json").Then(optionsResponse =>
                                    {
                                        Debug.Log(optionsResponse.Text);
                                        var options = JsonConvert.DeserializeObject<List<string>>(optionsResponse.Text);
                                        Debug.Log(options);

                                        if (options != null && options.Count > 0)
                                        {
                                            for (int i = 0; i < options.Count; i++)
                                            {
                                                Transform optionTransform = questionTemplate.transform.Find("Container/MultipleChoice")
                                                    .GetChild(i)
                                                    .Find("Heading");

                                                if (optionTransform != null)
                                                {
                                                    TMP_Text optionText = optionTransform.GetComponent<TMP_Text>();
                                                    if (optionText != null)
                                                    {
                                                        optionText.text = options[i];
                                                    }
                                                    else
                                                    {
                                                        Debug.LogWarning("Option text component not found for option " + (i + 1));
                                                    }
                                                }
                                                else
                                                {
                                                    Debug.LogWarning("Option " + (i + 1) + " transform not found");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogWarning("No options found for the multiple-choice question");
                                        }
                                    }).Catch(error =>
                                    {
                                        Debug.LogError("Failed to fetch options: " + error.Message);
                                    });

                                    break;

                                    case 4: //TrueOrFalse
                                        questionTemplate.transform.Find("Container/Question/Heading").GetComponent<TMP_Text>().text = numbering + ". " + question.question;
                                        break;
                                    default:
                                        Debug.LogWarning("Unknown question type: " + question.question);
                                        break;
                                }
                                numbering++;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No questions found for the assessment: " + assessmentIndex);
                    }
                }).Catch(error =>
                {
                    Debug.LogError("Failed to fetch questions: " + error.Message);
                });
            }
            else
            {
                Debug.LogWarning("Invalid assessment index: " + assessmentIndex);
            }
        }).Catch(error =>
        {
            Debug.LogError("Failed to fetch equipment assessment: " + error.Message);
        });
    }

}




// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Firebase;
// using Firebase.Database;
// using TMPro;
// using System.Threading.Tasks;
// using System.Linq;

// public class AssessmentHandler : MonoBehaviour
// {     
//     private DatabaseReference dbReference;
    
//     public TMP_Text detail_classCode;
//     public TMP_Text detail_faculty;
//     public TMP_Text detail_datePosted;
//     public TMP_Text detail_dueDate;
//     public TMP_Text detail_closingTime;
//     public TMP_Text detail_description;
//     public TMP_Text detail_equipment;
 
//     public TMP_Text Instructions;

//     public GameObject questionTemplate1;
//     public GameObject questionTemplate2;
//     public GameObject questionTemplate3;
//     public GameObject questionTemplate4;
//     public Transform parentTransform;

//     private int numbering = 1;
//     public TMP_Text testTitle;

//     void Awake()
//     {
//         dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                
//     }


//     private void Start()
//     {
//         // Retrieve data from Firebase for the specified identifier
//         RetrieveEquipmentAssessmentData(ButtonGeneratorScores.equipmentAssessment);
        
//     }

//     private async void RetrieveEquipmentAssessmentData(string assessmentIndex)
//     {
//         DataSnapshot assessmentSnapshot = await dbReference.Child("equipmentAssessments").Child(ButtonGeneratorScores.equipmentAssessment).GetValueAsync();

//         if (assessmentSnapshot != null && assessmentSnapshot.Exists)
//         {
//             EquipmentAssessmentData.EquipmentAssessment equipmentAssessment =
//                 JsonUtility.FromJson<EquipmentAssessmentData.EquipmentAssessment>(assessmentSnapshot.GetRawJsonValue());

//             DataSnapshot questionsSnapshot = await dbReference.Child("equipmentAssessments").Child(ButtonGeneratorScores.equipmentAssessment).Child("questions").GetValueAsync();

//             if (questionsSnapshot != null && questionsSnapshot.Exists)
//             {
//                 IEnumerable<DataSnapshot> questionSnapshots = questionsSnapshot.Children;

//                 List<EquipmentAssessmentData.Question> questions = new List<EquipmentAssessmentData.Question>();

//                 foreach (DataSnapshot questionSnapshot in questionSnapshots)
//                 {
//                     EquipmentAssessmentData.Question question = JsonUtility.FromJson<EquipmentAssessmentData.Question>(questionSnapshot.GetRawJsonValue());
//                     questions.Add(question);
//                 }

//                 // Log assessment details
//                 testTitle.text = equipmentAssessment.equipment;
//                 Instructions.text = equipmentAssessment.instructions;
//                 Debug.Log("Assessment Identifier: " + ButtonGeneratorScores.equipmentAssessment);
//                 Debug.Log("Class Code: " + equipmentAssessment.classCode);
//                 Debug.Log("Closing Date Time: " + equipmentAssessment.closingDateTime);
//                 detail_classCode.text = equipmentAssessment.classCode;
//                 detail_equipment.text = equipmentAssessment.equipment;
//                 detail_description.text = equipmentAssessment.description;
//                 detail_faculty.text = equipmentAssessment.faculty;
//                 detail_dueDate.text = equipmentAssessment.dueDateTime.ToString();
//                 detail_datePosted.text = equipmentAssessment.datePosted.ToString();
//                 detail_closingTime.text = equipmentAssessment.closingDateTime.ToString();

//                 // Log question count
//                 Debug.Log("Question Count: " + questions.Count);
//                 foreach (EquipmentAssessmentData.Question question in questions)
//                 {
//                     // Instantiate question template based on question type
//                     GameObject questionTemplate = null;

//                     switch (question.questionType)
//                     {
//                         case 1:
//                             questionTemplate = Instantiate(questionTemplate1, parentTransform);
//                             break;
//                         case 2:
//                             questionTemplate = Instantiate(questionTemplate2, parentTransform);
//                             break;
//                         case 3:
//                             questionTemplate = Instantiate(questionTemplate3, parentTransform);
//                             break;
//                         case 4:
//                             questionTemplate = Instantiate(questionTemplate4, parentTransform);
//                             break;
//                         default:
//                             Debug.LogWarning("Unknown question type: " + question.questionType);
//                             break;
//                     }
//                     // Customize the instantiated question template based on the question data
//                     if (questionTemplate != null)
//                     {
//                         switch (question.questionType)
//                         {
//                             case 1: //Essay
//                                 questionTemplate.transform.Find("Question/Heading").GetComponent<TMP_Text>().text = numbering+". "+ question.question;
//                                 break;
//                             case 2: //Identification
//                                 questionTemplate.transform.Find("Question/Heading").GetComponent<TMP_Text>().text = numbering+". "+ question.question;
//                                 // Customize question template for question type 2
//                                 break;
//                             case 3: // MultipleChoice
//                                 questionTemplate.transform.Find("Container/Question/Heading").GetComponent<TMP_Text>().text = numbering+". "+ question.question;
//                             // Retrieve the options snapshot
//                                 DataSnapshot optionsSnapshots = await dbReference.Child("equipmentAssessments")
//                                     .Child(ButtonGeneratorScores.equipmentAssessment)
//                                     .Child("questions")
//                                     .Child((numbering - 1).ToString())
//                                     .Child("multipleChoice")
//                                     .Child("options")
//                                     .GetValueAsync();

//                                 IEnumerable<DataSnapshot> optionSnapshots = optionsSnapshots.Children;

//                                 // Check if the options snapshot exists
//                                 if (optionSnapshots != null && optionSnapshots.Any())
//                                 {
//                                     // Retrieve the option values
//                                     List<string> options = new List<string>();
//                                     foreach (DataSnapshot optionSnapshot in optionSnapshots)
//                                     {
//                                         string option = optionSnapshot.GetRawJsonValue();
//                                         options.Add(option);
//                                         Debug.Log("Option" + option);
//                                     }

//                                     // Customize the multiple choice question template
//                                     for (int i = 0; i < options.Count; i++)
//                                     {
//                                         questionTemplate.transform.Find("Container/MultipleChoice")
//                                             .GetChild(i).GetChild(0)
//                                             .GetComponent<TMP_Text>()
//                                             .text = options[i];
//                                     }
//                                 }
//                                 else
//                                 {
//                                     Debug.LogWarning("No options found for the multiple choice question");
//                                 }
//                                 break;
//                             case 4: //TrueOrFalse
//                                 questionTemplate.transform.Find("Container/Question/Heading").GetComponent<TMP_Text>().text = numbering+". "+ question.question;
//                                 break;
//                             default:
//                                 Debug.LogWarning("Unknown question type: " + question.question);
//                                 break;
//                         }
//                         numbering++;
//                     }
//                 }
//             }
//             else
//             {
//                 Debug.LogWarning("No questions found for the assessment: " + ButtonGeneratorScores.equipmentAssessment);
//             }
//         }
//         else
//         {
//             Debug.LogWarning("Invalid assessment index: " + assessmentIndex);
//         }
//     }

// }