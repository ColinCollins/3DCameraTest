using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum Direction { 
    X,
    Y,
    Z
}

public class GizmosObj : MonoBehaviour
{
    public Direction Dir;
    [HideInInspector]
    public Material Mat;
    private Color selfColor;

    public void Init()
    {
        Mat = GetComponentInChildren<Renderer>().material;
        selfColor = Mat.color;
    }

    // Update is called once per frame

    public void SwtichMatColor(bool isSelected)
    {
        Mat.color = isSelected ? Color.yellow : selfColor;
    }
}
