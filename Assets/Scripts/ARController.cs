using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.XR.ARSubsystems;

public class ARController : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public GameObject objectPlaced;

    public ARSessionOrigin arOrigin;
    public ARRaycastManager arRaycastManager;
    private Pose placementPose;
    public bool placementPoseIsValid = false;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public LayerMask layerMask;
    public GameObject[] testGround;



    private void Awake()
    {
#if UNITY_EDITOR
        for (int i = 0; i < testGround.Length; i++)
        {
            testGround[i].SetActive(true);
        }
        var cam = Camera.main.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
#else
        for (int i = 0; i < testGround.Length; i++)
        {
            testGround[i].SetActive(false);
        }
#endif
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif
        touchPosition = default;
        return false;
    }
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // we actually ignore touchPosition, and always use screenCenter for the ray

        if (placementPoseIsValid)
        {
            PlaceObject();
        }

    }

    private void PlaceObject()
    {
        if(objectPlaced == null)
        {
            objectPlaced = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        }
        else
        {
            Debug.Log("Object Has Been Place");
        }
       
    }

    private void UpdatePlacementIndicator()
    {
        if(placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose(Vector3 hitPoint)
    {
        placementPose.position = hitPoint;
        var cameraForward = Camera.main.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
        placementPose.rotation = Quaternion.LookRotation(cameraBearing);
    }

    private void UpdatePlacementPose()
    {
#if UNITY_EDITOR
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f, 0f));
        RaycastHit hit;

        placementPoseIsValid = Physics.Raycast(ray, out hit, 500f, layerMask);
        if (placementPoseIsValid)
        {
            UpdatePlacementPose(hit.point);
        }
#else
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        placementPoseIsValid = hits.Count > 0;
        if(placementPoseIsValid)
        {
            UpdatePlacementPose(hits[0].pose.position);
        }
#endif

    }
}
