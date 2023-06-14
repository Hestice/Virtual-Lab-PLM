using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionsHandler : MonoBehaviour
{
    public struct EquipmentSteps
    {
        public string[] header;
        public string[] body;
        public string objectives;
    }

    public static int currentIndex = 0;
    private int currentEquipmentIndex = ButtonGenerator.activeButtonIndex;
    private EquipmentSteps[] equipmentSteps;
    private EquipmentSteps currentEquipment;
    private TMP_Text headerText;
    private TMP_Text bodyText;
    private TMP_Text whatIs;
    private TMP_Text objectiveSub;

    private Animator descriptionAnimator;
    public Transform contentParent;

    private GameObject what;
    private GameObject objHeader;
    private GameObject objSub;


    void Awake(){
        what = GameObject.Find("What Is");
        objHeader = GameObject.Find("Objectives Header");
        objSub = GameObject.Find("Objectives Sub");
    }

    void Start()
    {
        headerText = GameObject.Find("Description Header").GetComponent<TMP_Text>();
        bodyText = GameObject.Find("Description Sub").GetComponent<TMP_Text>();
        whatIs = GameObject.Find("What Is").GetComponent<TMP_Text>();
        objectiveSub = GameObject.Find("Objectives Sub").GetComponent<TMP_Text>();

        whatIs.text = "What is "+MainUIDisplays.Equipment[ButtonGenerator.activeButtonIndex];
        descriptionAnimator = GetComponent<Animator>();

        InitializeEquipmentSteps();

        if (currentEquipmentIndex >= 0 && currentEquipmentIndex < equipmentSteps.Length)
        {
            currentEquipment = equipmentSteps[currentEquipmentIndex];
            currentIndex = 0; // Set currentIndex to 0 initially
            UpdateUI();
        }
        else
        {
            Debug.LogError("Invalid currentEquipmentIndex!");
        }
    }

    void InitializeEquipmentSteps()
    {
        equipmentSteps = new EquipmentSteps[9]; // Assuming you have 7 equipment items

        // Define the equipment steps in the desired order
        InitializeSphygmomanometerSteps();
        InitializePulseOximeterSteps();
        InitializeIntramuscularSteps();
        InitializeSubcutaneousSteps();
        InitializeIntradermalSteps();
        InitializeWalkerSteps();
        InitializeAxillaryCrutchSteps();
        InitializeForearmCrutchSteps();
        InitializeCaneSteps();
    }

    void InitializeSphygmomanometerSteps()
    {
        EquipmentSteps Sphygmomanometer = new EquipmentSteps();
        Sphygmomanometer.header = new string[] { "Description", "Step" };
        Sphygmomanometer.body = new string[]
        {
            "A Manual, or aneroid equipment includes a cuff, an attached pump or bulb, a gauge or pressure gauge and a stethoscope. When you're ready to take your blood pressure, sit quietly for three to five minutes beforehand. ",
            "To begin, place the cuff on your bare upper arm one inch above the bend of your elbow.",
            "Pull the end of the cuff so that it's evenly tight around your arm. You should place it tight enough so that you can only slip two fingertips under the top edge of the cuff. Make sure your skin doesn't pinch when the cuff inflates. ",
            "Once the cuff is on, place the disk of the stethoscope facedown under the cuff, just to the inner side of your upper arm. Place the stethoscope earpieces in your ears.",
            "Rest the gauge in the open palm of the hand of your cuffed arm so that you can clearly see it. Then, squeeze the pump rapidly until the gauge reads 30 points above your usual systolic pressure. (Be sure to inflate the cuff rapidly) ",
            "Stop squeezing. Turn the knob on the pump toward you (counterclockwise) to let the air out slowly. Let the pressure fall 2 millimeters, or lines on the dial, per second while listening for your heart sounds. Note the reading when you first hear a heartbeat, this is your systolic pressure. Note when you no longer hear the beating sounds, this is your diastolic pressure. Then, Rest and record the info."
        };
        Sphygmomanometer.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";

        equipmentSteps[0] = Sphygmomanometer;
    }

    void InitializePulseOximeterSteps()
    {
        EquipmentSteps pulseOximeter = new EquipmentSteps();
        pulseOximeter.header = new string[] { "Description", "Step" };
        pulseOximeter.body = new string[]
        {
            "Description of pulse oximeter step 1",
            "Description of pulse oximeter step 2",
            "Description of pulse oximeter step 3",
            "Description of pulse oximeter step 4",
            "Description of pulse oximeter step 5"
        };
        pulseOximeter.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";

        equipmentSteps[1] = pulseOximeter;
    }

    void InitializeIntramuscularSteps()
    {
        EquipmentSteps Intramuscular = new EquipmentSteps();
        Intramuscular.header = new string[] { "Description", "Step" };
        Intramuscular.body = new string[]
        {
            "Intramuscular (IM) injections are injections that are administered directly into a muscle using a needle and syringe.",
            "Select the appropriate injection site: The healthcare provider will select the appropriate muscle to use for the injection, which may include the upper arm, thigh, or buttocks.",
            "Draw up the medication: The medication will be drawn up into the syringe and the needle will be attached.",
            "Clean the area: The area around the injection site will be cleaned with an antiseptic solution to reduce the risk of infection. ",
            "Prepare the injection site: The healthcare provider may use their finger to locate the exact injection site and then stretch the skin to make the injection site taut.",
            "Insert the needle: The needle will be inserted into the muscle at a 90-degree angle, quickly and smoothly.",
            "Administer the medication: The plunger of the syringe will be slowly and steadily pushed to administer the medication.",
            "Remove the needle: The needle will be removed from the muscle quickly and at the same angle as it was inserted.",
            "Apply pressure: Pressure may be applied to the injection site with a cotton ball or gauze to help reduce bleeding. Discard the needle and syringe: The needle and syringe should be disposed of in an appropriate sharps container. "
        };
        Intramuscular.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[2] = Intramuscular;
        
    }

    void InitializeSubcutaneousSteps()
    {
        EquipmentSteps Subcutaneous = new EquipmentSteps();
        Subcutaneous.header = new string[] { "Description", "Step" };
        Subcutaneous.body = new string[]
        {
            "Subcutaneous (SC) injections are injections that are administered into the fatty tissue just below the skin using a needle and syringe.",
            "Select the appropriate injection site: The healthcare provider will select the appropriate site for the injection, which may include the abdomen, thigh, or upper arm.",
            "Draw up the medication: The medication will be drawn up into the syringe and the needle will be attached.",
            "Clean the area: The area around the injection site will be cleaned with an antiseptic solution to reduce the risk of infection. ",
            "Prepare the injection: The medication will be drawn up into the syringe and the needle will be attached. Pinch the skin: The healthcare provider may use their fingers to pinch the skin at the injection site to create a small fold of skin.",
            "Insert the needle: The needle will be inserted at a 45-degree angle into the fold of skin. ",
            "Administer the medication: The plunger of the syringe will be slowly and steadily pushed to administer the medication.",
            "Remove the needle: The needle will be removed from the skin quickly and at the same angle as it was inserted. Apply pressure: Pressure may be applied to the injection site with a cotton ball or gauze to help reduce bleeding. Discard the needle and syringe: The needle and syringe should be disposed of in an appropriate sharps container. "
        };
        Subcutaneous.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[3] = Subcutaneous;
    }

    void InitializeIntradermalSteps()
    {
        EquipmentSteps Intradermal = new EquipmentSteps();
        Intradermal.header = new string[] { "Description", "Step" };
        Intradermal.body = new string[]
        {
            "Intradermal (ID) injections are injections that are administered into the top layers of the skin using a very small needle.",
            "Select the appropriate injection site: The healthcare provider will select the appropriate site for the injection, which is usually on the forearm.",
            "Draw up the medication: The medication will be drawn up into the syringe and the needle will be attached.",
            "Clean the area: The area around the injection site will be cleaned with an antiseptic solution to reduce the risk of infection. ",
            "Stretch the skin: The healthcare provider will use their fingers to stretch the skin at the injection site to make it taut. ",
            "Insert the needle: The needle will be inserted at a 10-15 degree angle into the top layers of the skin, just deep enough to create a small bubble or bleb.",
            "Administer the medication: The plunger of the syringe will be slowly and steadily pushed to administer the medication.",
            "Remove the needle: The needle will be removed from the skin quickly and at the same angle as it was inserted. Apply pressure: Pressure may be applied to the injection site with a cotton ball or gauze to help reduce bleeding. Discard the needle and syringe: The needle and syringe should be disposed of in an appropriate sharps container. "
        };
        Intradermal.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[4] = Intradermal;
    }

    void InitializeWalkerSteps()
    {
        EquipmentSteps walker = new EquipmentSteps();
        walker.header = new string[] { "Description", "Step" };
        walker.body = new string[]
        {
            "Walkers provide great stability due to their wide base, so they are great for people who can bear weight on their feet but have trouble walking due to weakness of the legs or balance issues.",
            "The client should stand upright while holding the hand grips.",
            "When moving forward, they lift it up and move it another 6 to 10 inches in front of them and set it down.",
            "Using the walker as support, they should move one leg forward and then the other.",
            "Once balance is reestablished, repeat the process."
        };
        walker.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[5] = walker;
    }

    void InitializeAxillaryCrutchSteps()
    {
        EquipmentSteps AxilaryCrutch = new EquipmentSteps();
        AxilaryCrutch.header = new string[] { "Description", "Step" };
        AxilaryCrutch.body = new string[]
        {
            "Axillary crutches stretch from the armpits, or axilla, to the ground. Crutches should be squeezed between the arms and chest. To avoid damage to the nerves and blood vessels in your armpits, your weight should rest on your hands, not on the underarm supports. ",
            "Firmly hold the hand grip. Put the crutches ahead and to the side of your feet for the best balance.",
            "Both crutches move forward at the same time, supporting the weight of the body.",
            "Use the crutch as support, followed by the strong leg and then the weak leg",
            "Once balance is reestablished, repeat the process. Gently squeeze each crutch into your ribs. Put weight to your hands and keep your elbows straight."
        };
        AxilaryCrutch.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[6] = AxilaryCrutch;
    }

    void InitializeForearmCrutchSteps()
    {
        EquipmentSteps ForearmCrutch = new EquipmentSteps();
        ForearmCrutch.header = new string[] { "Description", "Step" };
        ForearmCrutch.body = new string[]
        {
            "Forearm crutches, also known as elbow crutch, are shorter, reaching from the elbow level to the ground. They have a hand grip and a cuff for the arm. ",
            "Hold the crutch with the hand on the opposite side of the weak leg, place the crutch on the side, in line with the legs and slightly to the sides.",
            "carefully put your weight through your hand onto the crutch for support, and move your leg and the crutch forward together,",
            "Move the good leg to be in line with the weak leg",
            "Repeat the action, to keep your head in posture upright to maintain balance."
        };
        ForearmCrutch.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[7] = ForearmCrutch;
    }

    void InitializeCaneSteps()
    {
        EquipmentSteps Cane = new EquipmentSteps();
        Cane.header = new string[] { "Description", "Step" };
        Cane.body = new string[]
        {
            "Canes are used by clients who could bear weight but have weakness in one of their legs, like a stroke patient or those with paralysis in one leg. Crutches and walkers are better for people with two weak legs.",
            "Hold it on the strong side, or the side without weakness. The cane tip should rest flat on the ground at the start.",
            "Lift and move the cane tip forward 6–10 inches before placing it flatly on the ground again. ",
            "Step forward with the weaker leg first, using the cane for support.",
            "After balance is established, move the stronger leg forward."
        };
        Cane.objectives = "\u2022 Objective 1 <br>\u2022 Objective 2<br>";
        equipmentSteps[8] = Cane;
    }

    void UpdateUI()
    {      
        if (currentIndex == 0)
        {
            headerText.text = currentEquipment.header[0];
            bodyText.text = currentEquipment.body[0];
            objectiveSub.text = currentEquipment.objectives;
            whatIs.text = "What is " + MainUIDisplays.Equipment[ButtonGenerator.activeButtonIndex] + "?";
            what.SetActive(true);
            objHeader.SetActive(true);
            objSub.SetActive(true);
            
        }
        else
        {
            headerText.text = currentEquipment.header[1] + " " + (currentIndex);
            what.SetActive(false);
            objHeader.SetActive(false);
            objSub.SetActive(false);
        }
        bodyText.text = currentEquipment.body[currentIndex];
        descriptionAnimator.SetTrigger("TextTransition");
    }

    public void Next()
    {
        if (currentIndex < currentEquipment.body.Length - 1)
        {
            currentIndex++;
            Debug.Log("Next index: "+currentIndex);
        }
        UpdateUI();
    }

    public void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            Debug.Log("Previous index: "+currentIndex);
        }
        UpdateUI();
    }

    public void SkipToEnd()
    {
        currentIndex = currentEquipment.body.Length - 1;
        Debug.Log("SKipToEnd index: "+currentIndex);
        what.SetActive(false);
        objHeader.SetActive(false);
        objSub.SetActive(false);
        UpdateUI();
    }

    public void BackToStart()
    {
        Debug.Log("BackToStart index: "+currentIndex);
        currentIndex = 0;
        what.SetActive(true);
        what.SetActive(true);
        objHeader.SetActive(true);
        objSub.SetActive(true);
        UpdateUI();
    }
}
