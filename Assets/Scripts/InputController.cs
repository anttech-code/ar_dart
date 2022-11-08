using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft;
using TMPro;

public class InputController : MonoBehaviour, IMixedRealityGestureHandler<Vector3>
{

    public GameObject Board;
    public GameObject DartPrefab;

    private GameObject Dart;

    private string mode = "";

    void Start()
    {
        Debug.Log("Started");
        // StartCoroutine(waiter());

        mode = "Dart";
        Dart = (GameObject)Instantiate(DartPrefab, transform.position, Quaternion.identity);
        Dart.GetComponent<DartHandler>().stopped = true;



    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(0.05f);
        int nmb = 50;
        for (int i = 0; i < nmb; i++)
        {
            float x = Random.Range(-0.07f, 0.07f);
            float y = Random.Range(-0.07f, 0.07f);
            GameObject dart = (GameObject)Instantiate(DartPrefab, new Vector3(x, y, 0), Quaternion.identity);
            dart.GetComponent<DartHandler>().velocity = new Vector3(0, 5, 7);
            if (i != nmb - 1)
                yield return new WaitForSeconds(0.05f);
        }
    }



    void Update()
    {
        switch(mode)
        {
            case "Board":
                moveBoard();
                break;
            case "Dart":
                moveDart();
                break;
            default:
                break;
        }
    }

    private void moveDart()
    {

        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexKnuckle, Handedness.Right, out MixedRealityPose index_back))
            return;
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose index_front))
            return;
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbProximalJoint, Handedness.Right, out MixedRealityPose thumb_back))
            return;
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.ThumbTip, Handedness.Right, out MixedRealityPose thumb_front))
            return;
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose palm))
            return;

            Vector3 mid_front = midpoint(index_front.Position, thumb_front.Position, 0.4f);
        Vector3 mid_back = midpoint(index_back.Position, thumb_back.Position, 0.6f);
        Vector3 dir = (mid_front - mid_back);


        Dart.transform.position = mid_front + dir * 0.07f - palm.Right * 0.015f + palm.Up * 0.015f;
        Dart.transform.forward = dir;
            

    }
    private Vector3 midpoint(Vector3 a, Vector3 b)
    {
        return midpoint(a, b, 0.5f);
    }

    private Vector3 midpoint(Vector3 a, Vector3 b, float p)
    {
        Vector3 temp = new Vector3();
        temp.x = a.x + p * (b.x - a.x);
        temp.y = a.y + p * (b.y - a.y);
        temp.z = a.z + p * (b.z - a.z);
        return temp;
    }

    private void moveBoard()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose jointPose))
        {
            if (Board.GetComponent<BoardHandler>().projectTo(jointPose.Position, jointPose.Forward))
            {
                //Debug.Log("Hit");
            }
            else
            {
                //Debug.Log("No Hit");
            }
        }
    }
    

    public void OnGestureStarted(InputEventData eventData)
    {
        Debug.Log($"OnGestureStarted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

    }

    public void OnGestureUpdated(InputEventData eventData)
    {
        Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    public void OnGestureUpdated(InputEventData<Vector3> eventData)
    {
        Debug.Log($"OnGestureUpdated [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    public void OnGestureCompleted(InputEventData<Vector3> eventData)
    {
        Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    public void OnGestureCanceled(InputEventData eventData)
    {
        Debug.Log($"OnGestureCanceled [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    void Awake()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler<Vector3>>(this);
    }
}