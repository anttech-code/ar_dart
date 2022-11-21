using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartHandler : MonoBehaviour
{

    public GameObject Dart;
    public float timestep = 0.1f;

    public Vector3 velocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    public bool stopped = false;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0, 3f, 5f);

    }

    public void setVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopped)
        {

            acceleration = Vector3.zero;
            acceleration += new Vector3(0, -9.81f, 0);
            velocity += acceleration * Time.deltaTime;

            int layerA = 8; // Player
            int layerB = 12; // Darts
            int layerMaskCombined = (1 << layerA) | (1 << layerB);
            layerMaskCombined = ~layerMaskCombined;
            Vector3 dir = velocity.normalized;
            Vector3 from = transform.position + dir * 0.05f;
            RaycastHit hit;
            if (Physics.Raycast(from, dir, out hit, (velocity * Time.deltaTime).magnitude, layerMaskCombined))
            {
                if (hit.collider.gameObject.name != "Dartboard")
                    Debug.Log(hit.collider.gameObject.name);
                transform.position = hit.point;
                velocity = Vector3.zero;
                stopped = true;
            } else
            {
                transform.position += velocity * Time.deltaTime;
            }


            if (velocity.magnitude > 0.01)
                transform.forward = velocity.normalized;
        }
    }

    void OnTriggerEnter(Collider collision)
    {

        velocity = Vector3.zero;
        if (collision.tag == "Board")
        {
            transform.parent = collision.gameObject.transform;
            //transform.localPosition = Vector3.zero;
            //transform.localRotation = Quaternion.identity;
            BoardHandler board = collision.gameObject.GetComponent<BoardHandler>();
            board.hit(Dart);

        }
    }
}
