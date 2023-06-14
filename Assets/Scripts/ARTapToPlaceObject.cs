using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using System;

//[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;

    private GameObject spawnedObject;
    private XROrigin arOrigin;
    private ARRaycastManager rayCastMgr;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    Quaternion targetRotation;
    private Vector3 objectToPlace_FinalPosition;
    private float rotateStep = 30f;

    void Start()
    {
        targetRotation = placementIndicator.transform.rotation;
        arOrigin = FindObjectOfType<XROrigin>();
        rayCastMgr = FindObjectOfType<ARRaycastManager>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    public void PlaceObject()
    {
        if(spawnedObject == null){
            spawnedObject = Instantiate(objectToPlace, placementPose.position, targetRotation);
            spawnedObject.tag = "SpawnedEquipment";
        }
        else{
            Destroy (GameObject.FindWithTag("SpawnedEquipment"));
            spawnedObject = null;
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && spawnedObject == null){
            placementIndicator.SetActive(true);
            // Smoothly interpolate the rotation of the placement indicator
            placementIndicator.transform.rotation = Quaternion.Lerp(placementIndicator.transform.rotation, targetRotation, Time.deltaTime * 10f);
            // Update the placement indicator position
            placementIndicator.transform.position = placementPose.position;
            objectToPlace_FinalPosition = new Vector3(placementPose.position.x, placementPose.position.y, placementPose.position.z + 0.1612f);
        }      
        else {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        if(spawnedObject == null){
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        rayCastMgr.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid){
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
        Debug.Log(placementPose.rotation);

    }

    public void RotateToRight()
    {
        targetRotation *= Quaternion.Euler(0, rotateStep, 0);
        placementPose.rotation = Quaternion.RotateTowards(placementPose.rotation, targetRotation, rotateStep * Time.deltaTime);
        Debug.Log(placementPose.rotation);
    }
}
