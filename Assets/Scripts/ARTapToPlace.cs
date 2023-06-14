using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using System;

//[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    private Transform objectToPlace; // Changed GameObject to Transform
    public GameObject placementIndicator;
    public GameObject equipmentInstances;
    private int equipmentToPlace = ButtonGenerator.activeButtonIndex;
    private Animator ActiveEquipment_Animator;

    [Header("UI Buttons:")]
    public GameObject uiPlaceButton;
    public GameObject uiRemoveButton;
    public GameObject uiRotateLeft;
    public GameObject uiRotateRight;
    [Space(10)]
    

    private GameObject spawnedObject;
    private XROrigin arOrigin;
    private ARRaycastManager rayCastMgr;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    Quaternion targetRotation;
    private Vector3 objectToPlace_FinalPosition;
    private float rotateStep = 30f;

    //Reference to Placement Indicator
    //Other equipment are hidden
    void Awake()
    {
        placementIndicator = GameObject.Find("/Placement Indicator");
        // Initialize objectToPlace with the prefab variant
        if (equipmentToPlace == -1)
            equipmentToPlace = 0;
        Debug.Log("equipmentToPlace == " + equipmentToPlace);

        objectToPlace = equipmentInstances.transform.GetChild(equipmentToPlace);
        spawnedObject = Instantiate(objectToPlace.gameObject, placementPose.position, targetRotation);
        spawnedObject.tag = "SpawnedEquipment";
        DestroyAllChildren();
    }

    void Start()
    {
        targetRotation = placementIndicator.transform.rotation;
        arOrigin = FindObjectOfType<XROrigin>();
        rayCastMgr = FindObjectOfType<ARRaycastManager>();
        uiRemoveButton.SetActive(false);
        spawnedObject.SetActive(false);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        
    }

    void DestroyAllChildren()
    {
        Transform parent = equipmentInstances.transform; // Update the parent transform to equipmentInstances
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    //Spawn the equipment
    //Delete the equipment instances
    public void PlaceObject()
    {
        if(!spawnedObject.activeSelf){
            spawnedObject.SetActive(true); // Show the spawned object

            uiRemoveButton.SetActive(true);
            uiPlaceButton.SetActive(false);
            uiRotateLeft.SetActive(false);
            uiRotateRight.SetActive(false);

        }
        else{
            spawnedObject.SetActive(false);

            uiRemoveButton.SetActive(false);
            uiPlaceButton.SetActive(true);
            uiRotateLeft.SetActive(true);
            uiRotateRight.SetActive(true);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !spawnedObject.activeSelf){
            placementIndicator.SetActive(true);
            // Smoothly interpolate the rotation of the placement indicator
            placementIndicator.transform.rotation = Quaternion.Lerp(placementIndicator.transform.rotation, targetRotation, Time.deltaTime * 10f);
            // Update the placement indicator position
            placementIndicator.transform.position = placementPose.position;
            objectToPlace_FinalPosition = new Vector3(placementPose.position.x, placementPose.position.y, placementPose.position.z + 0.1612f);

            spawnedObject.transform.position = placementPose.position;
        }      
        else {
            placementIndicator.SetActive(false);
        }
        Debug.Log("spawned object rotation: "+ spawnedObject.transform.rotation);
        Debug.Log("placement pose rotation: "+ placementPose.rotation);
    }

    private void UpdatePlacementPose()
{
    if (!spawnedObject.activeSelf)
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.65f));
        var hits = new List<ARRaycastHit>();
        rayCastMgr.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.RotateTowards(placementPose.rotation, targetRotation, rotateStep * Time.deltaTime);
        }
    }
}
    public void RotateToLeft()
    {
        targetRotation *= Quaternion.Euler(0, -rotateStep, 0);
        placementPose.rotation = Quaternion.RotateTowards(placementPose.rotation, targetRotation, rotateStep * Time.deltaTime);       
        spawnedObject.transform.rotation = targetRotation;
        Debug.Log("target rotation: "+ targetRotation);
    }

    public void RotateToRight()
    {
        targetRotation *= Quaternion.Euler(0, rotateStep, 0);
        placementPose.rotation = Quaternion.RotateTowards(placementPose.rotation, targetRotation, rotateStep * Time.deltaTime);
        spawnedObject.transform.rotation = targetRotation;
        Debug.Log("target rotation: "+ targetRotation);
        Debug.Log("spawned object rotation: "+ spawnedObject.transform.rotation);
        Debug.Log("placement pose rotation: "+ placementPose.rotation);
    }
}

