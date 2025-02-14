﻿using UnityEngine;
using System.Collections;

public class LeapGestureConfig : MonoBehaviour {

    [SerializeField, Range(0.0f, 3.14f * 2), TooltipAttribute("認識できる範囲(円弧)")]
    float MIN_ARC = 3.14f * 2;

    [SerializeField, TooltipAttribute("認識できる範囲(半径)")]
    float MIN_RADIUS = 5.0f;

    [SerializeField, TooltipAttribute("スワイプする長さ")]
    float MIN_LENGTH = 150.0f;

    [SerializeField, TooltipAttribute("")]
    float MIN_VELOCITY = 1000.0f;

    public float MinArc { get { return MIN_ARC; } }
    public float MinRadius { get { return MIN_RADIUS; } }

    public float MinLength { get { return MIN_LENGTH; } }
    public float MinVelocity { get { return MIN_VELOCITY; } }
}
