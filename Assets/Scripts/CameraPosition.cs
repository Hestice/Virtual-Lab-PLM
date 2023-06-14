using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraPosition : MonoBehaviour
{
    private CinemachineFreeLook cinemachineFreeLook;
    private GameObject CMFreeLookObject;

    public static Vector3 lookPos, nextPos;
    public static bool isChangingFocus;
    private GameObject newParent; 
    public static int index;
    private Touch touch1, touch2;
    private float verticalInput, horizontalInput;
    
    private float speedModifier = 0.1f;

    void Awake(){
        CMFreeLookObject = GameObject.Find("CM FreeLook1");
    }
    void Start(){
        cinemachineFreeLook = CMFreeLookObject.GetComponent<CinemachineFreeLook>();
        
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isChangingFocus){
            transform.localPosition = Vector3.Lerp( transform.localPosition, nextPos, 0.2f);   
        }
        Panning();
        Invoke("setFocus",0.01f);  
        // highlightFocus(); 
       
    }
    //this gets called by the TouchField
    private void Panning(){
        if (Input.touchCount > 0)
            isChangingFocus = false;
        if (Input.touchCount > 1 ){
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
            touch1 = Input.GetTouch(0);
            touch2 = Input.GetTouch(1);
            
            if ((touch1.phase == TouchPhase.Moved) && (touch2.phase == TouchPhase.Moved)){
                // var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
                // var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

                verticalInput = touch1.deltaPosition.y;
                horizontalInput = touch1.deltaPosition.x;

                Vector3 forward = Camera.main.transform.forward;
                Vector3 right = Camera.main.transform.right;

                forward.y = 0;
                right.y = 0;
                forward = forward.normalized;
                right = right.normalized;

                Vector3 forwardRelativeVerticalInput = verticalInput * forward * -1;
                Vector3 rightRelativeHorizontalInput = horizontalInput * right * -1;


                Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeHorizontalInput;

                transform.Translate(cameraRelativeMovement * speedModifier, Space.World);
                
            } else {
                cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
                cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
            }
        } 

    }
    

    //this gets called by the resetbutton
    public void ResetCamPosition(){
        nextPos = new Vector3(0, 0, 0);
        isChangingFocus = true;         
    }

    // private void setFocus(){
    //     string facilityObject;
    //     facility tmpFac = DB.facilityList[index];
    //     // Debug.Log(FacilitiesList.isOnFirstFac + "; " + FacilitiesList.isOnSecondFac + "; " + FacilitiesList.noFac);
    //         if (FacilitiesList.isOnFirstFac == true){
    //             facilityObject = FacilitiesList.firstFacBuilding;
    //             newParent = GameObject.Find("/Buildings/"+ facilityObject);
    //             transform.SetParent(newParent.transform, true);
    //             ResetCamPosition();
    //             FacilitiesList.isOnFirstFac = false;
    //         } else if (FacilitiesList.isOnSecondFac == true) { 
    //             facilityObject = FacilitiesList.secondFac;
    //             newParent = GameObject.Find("/Buildings/"+ tmpFac.Building+"/" +facilityObject);
    //             Debug.Log("facilityObject: " + facilityObject);
    //             transform.SetParent(newParent.transform, true);
    //             ResetCamPosition();
    //             FacilitiesList.isOnSecondFac = false;
    //         } else if (FacilitiesList.noFac == true) {
    //             newParent = GameObject.Find("/Buildings/Sibuyas");
    //             transform.SetParent(newParent.transform, true);
    //             ResetCamPosition();
    //             FacilitiesList.noFac = false;
    //         }
    // }
}
