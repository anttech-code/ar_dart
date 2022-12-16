using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GestureUtils
{
    private const float PinchThresholdPlacement = 0.7f;
    private const float PinchThresholdThrowing = 0.2f;
    private const float GrabThreshold = 0.4f;

    public static bool IsPinching(Handedness trackedHand)
    {
        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        GameObject InputController = null;

        foreach (GameObject go in rootObjects)
        {
            if (go.name == "Constants")
            {
                InputController = go.GetComponent<ConstantsScript>().InputController;
            }
        }

        float currentThreshold = PinchThresholdPlacement;
        if (InputController.GetComponent<InputController>().mode == Modes.Dart || InputController.GetComponent<InputController>().mode == Modes.Idle)
        {
            currentThreshold = PinchThresholdThrowing;
        }

        //Debug.Log($"{currentThreshold}");


        return HandPoseUtils.CalculateIndexPinch(trackedHand) > currentThreshold;
    }

    public static bool IsGrabbing(Handedness trackedHand)
    {

        return !IsPinching(trackedHand) &&
               HandPoseUtils.MiddleFingerCurl(trackedHand) > GrabThreshold &&
               HandPoseUtils.RingFingerCurl(trackedHand) > GrabThreshold &&
               HandPoseUtils.PinkyFingerCurl(trackedHand) > GrabThreshold &&
               HandPoseUtils.ThumbFingerCurl(trackedHand) > GrabThreshold;
    }
}