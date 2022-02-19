using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
using System;

public class TileController : MonoBehaviour
{
    [SerializeField] private Vector3 Coordinates;
    [SerializeField] private float Cost;
    [SerializeField] private Material Material;
    
    public float GetCost()
    {
        return Cost;
    }
    public void Init()
    {
        GetComponent<MeshRenderer>().material = Material;
    }
}
