using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{

    public GameObject Board;
    public GameObject Line;

    public float size = 0.45f;
    public float playerDistance = 2.37f;
    public float boardMaxDiameter = 0.451f;
    public float boardInsideRadius = 0.17f;
    public float bullsEyeDiameter = 0.032f;
    public float bullsInside = 0.0127f;
    public float insideRingRadius = 0.107f;
    public float tripleDouble = 0.008f;


    public List<int> points = new List<int> ();

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

        int layerMaskCombined = (1 << (int)Layers.Board);
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
        if (r < bullsInside / 2) {
            points = 50;
        } else if (bullsInside / 2 < r && r < bullsEyeDiameter / 2) {
            points = 25;
        } else if (insideRingRadius - tripleDouble < r && r < insideRingRadius) {
            points *= 3;
        } else if (boardInsideRadius - tripleDouble < r && r < boardInsideRadius) {
            points *= 2;
        } else if (boardInsideRadius < r) {
            points = 0;
        }
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

        int layerMaskCombined = 
              (1 << (int)Layers.UI) 
            | (1 << (int)Layers.Menu) 
            | (1 << (int)Layers.Board) 
            | (1 << (int)Layers.Dart);
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

            RaycastHit lineHit;
            Vector3 horizontalBoardNormal = new Vector3(hit.normal.x, 0f, hit.normal.z);
            horizontalBoardNormal = horizontalBoardNormal.normalized;
            Vector3 downDir = new Vector3(0f, -1f, 0f);
            if (Physics.Raycast(hit.point + playerDistance*horizontalBoardNormal, downDir, out lineHit, 10f, layerMaskCombined))
            {
                Line.transform.position = lineHit.point;
                Line.transform.forward = horizontalBoardNormal;
            }

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
   
    public int GetPoints()
    {
        int counter = 0;
        for(int i = 0; i<points.Count; i++)
        {
            counter += points[i];
        }
        return counter;

    }

    public void ResetPoints()
    {
        points = new List<int>();
    }

}
