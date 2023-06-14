using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InitializeCamera : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _freeLookCamera;
    private float _xAxisValue;
    private float _yAxisValue = 0.45f;

    private float _topRigValue_H;
    private float _middleRigValue_H;
    private float _bottomRigValue_H;
    private float _topRigValue_R;
    private float _middleRigValue_R;
    private float _bottomRigValue_R;
    private int equipmentIndex;

    private GameObject focus;
    private GameObject cameraSystem;

    private void Awake(){
        equipmentIndex = ButtonGenerator.activeButtonIndex;
        Debug.Log("equipment Index: " +equipmentIndex);
        if (equipmentIndex == -1)
            equipmentIndex = 0;

          // Set the initial camera position based on the switch case
        switch (equipmentIndex)
        {
            case 0: //BPM
                _xAxisValue = -45f;
                _yAxisValue = 0.75f;
                _topRigValue_H = 1.09f;
                _middleRigValue_H = 0.31f;
                _bottomRigValue_H = -0.62f;

                _topRigValue_R = 0.63f;
                _middleRigValue_R = 1.48f;
                _bottomRigValue_R = -1.4f;
                focus = GameObject.FindWithTag("BP_Focus");
                break;

            case 1://Pulse Ox
                _xAxisValue = -45f;
                _yAxisValue = 0.75f;
                _topRigValue_H = 0.41f;
                _middleRigValue_H = 0.07f;
                _bottomRigValue_H = -0.06f;
                
                _topRigValue_R = 0.27f;
                _middleRigValue_R = 0.39f;
                _bottomRigValue_R = 0.16f;

                focus = GameObject.FindWithTag("Pulse_Focus");
                break;

            case 2: //IM
                _xAxisValue = -14f;
                _yAxisValue = 0.75f;
                _topRigValue_H = 0.2f;
                _middleRigValue_H = 0.02f;
                _bottomRigValue_H = -0.17f;
                
                _topRigValue_R = 0.26f;
                _middleRigValue_R = 0.42f;
                _bottomRigValue_R = 0.22f;

                focus = GameObject.FindWithTag("IM_Focus");
                break;

            case 3: //SUB
                _xAxisValue = -14f;
                _yAxisValue = 0.75f;
                _topRigValue_H = 0.1f;
                _middleRigValue_H = 0.02f;
                _bottomRigValue_H = -0.17f;
                
                _topRigValue_R = 0.26f;
                _middleRigValue_R = 0.42f;
                _bottomRigValue_R = 0.22f;

                focus = GameObject.FindWithTag("SUB_Focus");
                break;

            case 4: //ID
                _xAxisValue = -14f;
                _yAxisValue = 0.75f;
                _topRigValue_H = 0.1f;
                _middleRigValue_H = 0.02f;
                _bottomRigValue_H = -0.17f;
                
                _topRigValue_R = 0.13f;
                _middleRigValue_R = 0.21f;
                _bottomRigValue_R = 0.11f;
                focus = GameObject.FindWithTag("ID_Focus");
                break;
                
            case 5://Walker
                _xAxisValue = -45f;
                _topRigValue_H = 4.8f;
                _middleRigValue_H = 2.13f;
                _bottomRigValue_H = -1.04f;

                _topRigValue_R = 2f;
                _middleRigValue_R = 4.27f;
                _bottomRigValue_R = 3.25f;
                focus = GameObject.FindWithTag("Walker_Focus");
                break;

            case 6://AxCrutch
                _xAxisValue = -45f;  //the values are additive.
                _topRigValue_H = 4.8f;
                _middleRigValue_H = 2.13f;
                _bottomRigValue_H = -1.04f;

                _topRigValue_R = 2f;
                _middleRigValue_R = 4.27f;
                _bottomRigValue_R = 3.25f;
                focus = GameObject.FindWithTag("ACrutch_Focus");
                break;

            case 7://FCrutch or elbow crutch
                _xAxisValue = -45f;
                _topRigValue_H = 4.8f;
                _middleRigValue_H = 2.13f;
                _bottomRigValue_H = -1.04f;

                _topRigValue_R = 2f;
                _middleRigValue_R = 4.27f;
                _bottomRigValue_R = 3.25f;

                focus = GameObject.FindWithTag("FCrutch_Focus");
                break;
            
            case 8://Cane
                _xAxisValue = -45f;
                _topRigValue_H = 4.8f;
                _middleRigValue_H = 2.13f;
                _bottomRigValue_H = -1.04f;

                _topRigValue_R = 2f;
                _middleRigValue_R = 4.27f;
                _bottomRigValue_R = 3.25f;

                focus = GameObject.FindWithTag("Cane_Focus");
                break;
        }

        // Set the camera values based on the initialization values
        _freeLookCamera.m_XAxis.Value = _xAxisValue;
        _freeLookCamera.m_YAxis.Value = _yAxisValue;

        _freeLookCamera.m_Orbits[0].m_Height = _topRigValue_H;
        _freeLookCamera.m_Orbits[1].m_Height = _middleRigValue_H;
        _freeLookCamera.m_Orbits[2].m_Height = _bottomRigValue_H;

        _freeLookCamera.m_Orbits[0].m_Radius = _topRigValue_R;
        _freeLookCamera.m_Orbits[1].m_Radius = _middleRigValue_R;
        _freeLookCamera.m_Orbits[2].m_Radius = _bottomRigValue_R;

        cameraSystem = GameObject.FindWithTag("CameraSystem");
        if (focus != null)
            cameraSystem.transform.parent = focus.transform;   
    }      

    void Start(){
        cameraSystem.transform.position = Vector3.zero;
        // Debug.Log(cameraSystem.transform.position);
    }
    
    void Update(){
        // Debug.Log(" _freeLookCamera.m_XAxis.Value: "+  _freeLookCamera.m_XAxis.Value);
        // Debug.Log(" _freeLookCamera.m_YAxis.Value: "+  _freeLookCamera.m_YAxis.Value);
    }
}
