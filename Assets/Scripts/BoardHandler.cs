using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{

    public GameObject Board;

    // Start is called before the first frame update
    void Start()
    {
        //projectTo(new Vector3(-3.0f, -3.0f, 0.0f), new Vector3(0.7f, 0.7f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        //projectTo(new Vector3(-3.0f, -3.0f, 0.0f), new Vector3(0.7f, 0.7f, 1.0f));
    }

    public bool projectTo(Vector3 from, Vector3 dir)
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        //if (Physics.Raycast(from, transform.TransformDirection(dir), out hit, Mathf.Infinity, layerMask))
        if (Physics.Raycast(from, dir, out hit, Mathf.Infinity,layerMask))
        {
            //Debug.DrawRay(from, transform.TransformDirection(dir) * hit.distance, Color.yellow);
            Debug.DrawRay(from, dir * hit.distance, Color.green);
            //Debug.Log($"({from}, {transform.TransformDirection(dir) * hit.distance})");

            //Debug.Log("Did Hit");
            Board.transform.position = hit.point;
            Board.transform.rotation = Quaternion.LookRotation(hit.normal) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);
            return true;
        }
        else
        {
            Debug.DrawRay(from, transform.TransformDirection(dir) * 1000, Color.red);
            //Debug.Log("Did not Hit");
            return false;
        }
    }
}
