using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GravityField : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private Vector3 gravity = Vector3.zero;

    public Vector3 getGravity()
    {
        return gravity;
    }
    
}
