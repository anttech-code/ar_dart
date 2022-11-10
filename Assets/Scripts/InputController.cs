using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Microsoft;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
//using TMPro;

[AddComponentMenu("Scripts/InputController")]
public class InputController : MonoBehaviour, IMixedRealityGestureHandler<Vector3>
{

    [Header("Settings")]

    [SerializeField]
    private float interval = 100f;

    private Vector3 speed = Vector3.zero;
    private Queue<(Vector3, float)> track = new Queue<(Vector3, float)>();


    [Header("Gameobjects")]

    [SerializeField]
    private GameObject Board = null;

    [SerializeField]
    private GameObject DartPrefab = null;

    private GameObject Dart;

    [Header("Mapped gesture input actions")]

    [SerializeField]
    private MixedRealityInputAction holdAction = MixedRealityInputAction.None;

    [SerializeField]
    private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;

    [SerializeField]
    private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;

    [SerializeField]
    private MixedRealityInputAction tapAction = MixedRealityInputAction.None;


    [SerializeField]
    private Mode mode = Modes.idle;

    //private Modes modes = new Modes();
    class Mode
    {
        string mode;
        public Mode(string mode) { this.mode = mode; }
        public bool Equals(Mode other)
        {
            if (other == null)
                return false;
            return this.mode == other.mode;
        }
        public override bool Equals(object other) => this.Equals(other as Mode);
        public static bool operator ==(Mode lhs, Mode rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Mode lhs, Mode rhs) => !(lhs == rhs);
        public override int GetHashCode() => mode.GetHashCode();

    }
    class Modes
    {
        public static Mode board = new Mode("Board_mode");
        public static Mode dart = new Mode("Dart_mode");
        public static Mode idle = new Mode("Idle_mode");
    }

    void Start()
    {
        Debug.Log("Started");
        mode = Modes.dart;
        Dart = (GameObject)Instantiate(DartPrefab, transform.position, Quaternion.identity);
        Dart.GetComponent<DartHandler>().stopped = true;
        // StartCoroutine(waiter());
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
        if (mode == Modes.board)
        {
            moveBoard();
        } else if (mode == Modes.dart)
        {
            trackSpeed();
            moveDart();
        } else
        {

        }
    }

    private void trackSpeed()
    {
        float currentTime = Time.time * 1000f;

        Vector3 pos = Vector3.zero;
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose palm))
            pos = palm.Position;

        track.Enqueue((pos, currentTime));

        while (track.Peek().Item2 < currentTime - interval)
            track.Dequeue();

        speed = (pos - track.Peek().Item1) / (currentTime - track.Peek().Item2);
        speed *= 1000f;
    }

    private void throwDart()
    {
        DartHandler dart = Dart.GetComponent<DartHandler>();
        dart.velocity = speed;
        dart.stopped = false;
        //Debug.Log(speed);
        //Debug.Log(dart.velocity);
        mode = Modes.idle;
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
        if (!HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Right, out MixedRealityPose jointPose))
            return;
        
        if (Board.GetComponent<BoardHandler>().projectTo(jointPose.Position, jointPose.Forward))
        {
            //Debug.Log("Hit");
        }
        else
        {
            //Debug.Log("No Hit");
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
        Debug.Log($"OnGestureUpdated3 [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");
    }

    public void OnGestureCompleted(InputEventData eventData)
    {
        Debug.Log($"OnGestureCompleted [{Time.frameCount}]: {eventData.MixedRealityInputAction.Description}");

        MixedRealityInputAction action = eventData.MixedRealityInputAction;
        if (action == tapAction)
        {
            Debug.Log("test");
            if (mode == Modes.idle)
            {
                Debug.Log("New Dart");
                mode = Modes.dart;
                Dart = (GameObject)Instantiate(DartPrefab, transform.position, Quaternion.identity);
                Dart.GetComponent<DartHandler>().stopped = true;
            }
        }
    }

    public void OnGestureCompleted(InputEventData<Vector3> eventData)
    {
        Debug.Log($"OnGestureCompleted3 [{Time.frameCount}]: {eventData.MixedRealityInputAction}");

        MixedRealityInputAction action = eventData.MixedRealityInputAction;
        if (action == manipulationAction)
        {
            if (mode == Modes.dart)
            {
                throwDart();
            }
        }
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