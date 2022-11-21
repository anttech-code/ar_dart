using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
//using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;


public enum Gesture
{
    None,
    Pinch,
    Grab,
}

public class GestureHandler : MonoBehaviour
{

    private Gesture currentGesture = Gesture.None;
    private Gesture fixedGesture = Gesture.None;

    private float timeNewGesture = 0.05f;

    private float gestureStart = 0.0f;

    [SerializeField]
    private Handedness handedness = Handedness.None;

    // Start is called before the first frame update
    void Start()
    {
    }

    public Gesture GetGesture()
    {
        return fixedGesture;
    }

    // Update is called once per frame
    void Update()
    {
        if (HandJointUtils.FindHand(handedness) is null)
            return;

        Gesture newGesture = Gesture.None;

        if (GestureUtils.IsPinching(handedness))
            newGesture = Gesture.Pinch;
        else if (GestureUtils.IsGrabbing(handedness))
            newGesture = Gesture.Grab;


        // Here to filter out intermediate gestures, i.e. a new gesture has to hold for a certain time
        if (newGesture != currentGesture)
        {
            gestureStart = Time.time;
            currentGesture = newGesture;
        }

        if (fixedGesture != newGesture)
        {
            if (Time.time - gestureStart > timeNewGesture)
            {
                fixedGesture = newGesture;
            }
        }
    }
}
