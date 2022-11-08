using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{

    public GameObject Board;
    public float size = 0.45f;

    private List<int> points = new List<int> ();

    // Start is called before the first frame update
    void Start()
    {
        projectTo(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {
        //projectTo(new Vector3(-3.0f, -3.0f, 0.0f), new Vector3(0.7f, 0.7f, 1.0f));
    }



    public void hit(GameObject obj)
    {
        int layerA = 11;

        int layerMaskCombined = (1 << layerA); // | (1 << layerB);
        RaycastHit hit;
        //if (Physics.Raycast(from, transform.TransformDirection(dir), out hit, Mathf.Infinity, layerMask))
        Vector3 dir = obj.transform.forward;
        Vector3 from = obj.transform.position - dir * 2.0f;

        if (Physics.Raycast(from, dir, out hit, 10, layerMaskCombined))
        {
            Vector3 brd = transform.InverseTransformPoint(hit.point);
            Vector3 scl = transform.localScale;
            brd = new Vector3(brd.x / scl.x, brd.y / scl.y, brd.z / scl.z);
            //Debug.Log($"{-brd.x}, {brd.y}");
            points.Add(calculatePoints(-brd.x, brd.y));
            Debug.Log(points.Count);
        } else
        {
            Debug.Log("Ray did not hit board");
        }
    }

    private int calculatePoints(float x, float y)
    {
        return calculatePoints(cartesianToPolar(x, y));
    }

    private int calculatePoints(PolarCoordinate pos)
    {
        float r = pos.r;
        float phi = pos.phi;
        int[] pointsBoard = new int[] { 6, 13, 4, 18, 1, 20, 5, 12, 9, 14, 11, 8, 16, 7, 19, 3, 17, 2, 15, 10 };
        int points = pointsBoard[Mathf.FloorToInt(((phi + 9) % 360) / 18)];
        return points;
    }

    struct PolarCoordinate
    {
        public float r;
        public float phi;

        public PolarCoordinate(float r, float phi)
        {
            this.r = r;
            this.phi = phi;
        }
    }

    private PolarCoordinate cartesianToPolar(float x, float y)
    {
        float r = Mathf.Sqrt(x * x + y * y);
        float phi = 0f;
        if (x == 0)
        {
            if (y > 0)
            {
                phi = 90f;
            } else
            {
                phi = 270f;
            } 
        } else
        {
            phi = Mathf.Rad2Deg * Mathf.Atan(y / x);
            if (x < 0)
                phi += 180;
            else if (y < 0)
                phi += 360;
        }
        return new PolarCoordinate(r, phi);

    }

    public bool projectTo(Vector3 from, Vector3 dir)
    {
        int layerA = 11; // Board
        int layerB = 12; // Dart
        int layerC = 8; // Player

        int layerMaskCombined = (1 << layerA) | (1 << layerB) | (1 << layerC);
        layerMaskCombined = ~layerMaskCombined; // invert so it does not hit player, board, and dart

        RaycastHit hit;
        //if (Physics.Raycast(from, transform.TransformDirection(dir), out hit, Mathf.Infinity, layerMask))
        if (Physics.Raycast(from, dir, out hit, 10f, layerMaskCombined))
        {
            //Debug.DrawRay(from, transform.TransformDirection(dir) * hit.distance, Color.yellow);
            //Debug.DrawRay(from, dir * hit.distance, Color.green);
            //Debug.Log($"({from}, {transform.TransformDirection(dir) * hit.distance})");

            //Debug.Log("Did Hit");
            Board.transform.position = hit.point;
            Board.transform.forward = hit.normal.normalized;
            return true;
        }
        else
        {
            //Debug.DrawRay(from, transform.TransformDirection(dir) * 1000, Color.red);
            Board.transform.position = dir * 10;
            Board.transform.forward = -dir;
            //Debug.Log("Did not Hit");
            return false;
        }
    }
   
}
