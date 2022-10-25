using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft;

public class InputController : MonoBehaviour
{

    public GameObject Board;


    void Start()
    {
        Debug.Log("Started");
    }



    void Update()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, Handedness.Right, out MixedRealityPose jointPose))
        {
            if (Board.GetComponent<BoardHandler>().projectTo(jointPose.Position, jointPose.Forward))
            {
                Debug.Log("Hit");
            }
            else
            {
                Debug.Log("No Hit");
            }
        }
    }
}
