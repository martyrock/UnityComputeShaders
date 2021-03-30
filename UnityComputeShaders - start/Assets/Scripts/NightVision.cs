﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : BaseCompletePP
{
    [Range(0.0f, 100.0f)]
    public float radius = 70;
    [Range(0.0f, 1.0f)]
    public float tintStrength = 0.7f;
    [Range(0.0f, 100.0f)]
    public float softenEdge = 3;
    public Color tint = Color.green;
    [Range(2, 500)]
    public int lines = 100;

    public Transform target;

    private void OnValidate()
    {
        if(!init)
            Init();
           
        SetProperties();
    }

    protected void SetProperties()
    {
        float rad = (radius / 100.0f) * texSize.y;
        shader.SetFloat("radius", rad);
        shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
        shader.SetVector("tintColor", tint);
        shader.SetFloat("tintStrength", tintStrength);
        shader.SetInt("lines", lines);
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shader.SetFloat("time", Time.time);
        Vector2 pos = Camera.main.WorldToScreenPoint(target.position);
        Vector4 center = new Vector4(pos.x, pos.y, 0.0f, 0.0f);
        shader.SetVector("center", center);
        base.OnRenderImage(source, destination);
    }
}
