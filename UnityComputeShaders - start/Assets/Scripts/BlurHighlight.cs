﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurHighlight : BaseCompletePP
{
    [Range(0, 50)]
    public int blurRadius = 20;
    [Range(0.0f, 100.0f)]
    public float radius = 10;
    [Range(0.0f, 100.0f)]
    public float softenEdge = 30;
    [Range(0.0f, 1.0f)]
    public float shade = 0.5f;
    public Transform trackedObject;

    Vector4 center;
    RenderTexture horzOutput = null;
    RenderTexture vertOutput = null;
    int kernelHorzPassID;
    int kernelVertPassID;

    protected override void Init()
    {
        center = new Vector4();
        kernelName = "Highlight";
        base.Init();
        kernelHorzPassID = shader.FindKernel("HorzPass");
        kernelVertPassID = shader.FindKernel("VertPass");
    }

    protected override void CreateTextures()
    {
        base.CreateTextures();
        CreateTexture(ref horzOutput);
        CreateTexture(ref vertOutput);
        shader.SetTexture(kernelHorzPassID, "source", renderedSource);
        shader.SetTexture(kernelHorzPassID, "horzOutput", horzOutput);
        shader.SetTexture(kernelVertPassID, "horzOutput", horzOutput);
        shader.SetTexture(kernelVertPassID, "vertOutput", vertOutput);
        shader.SetTexture(kernelHandle, "vertOutput", vertOutput);
    }

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
        shader.SetFloat("shade", shade);
        shader.SetInt("blurRadius", blurRadius);
    }

    protected override void DispatchWithSource(ref RenderTexture source, ref RenderTexture destination)
    {
        if (!init) return;

        Graphics.Blit(source, renderedSource);
        shader.Dispatch(kernelHorzPassID, groupSize.x, groupSize.y, 1);
        shader.Dispatch(kernelVertPassID, groupSize.x, groupSize.y, 1);
        shader.Dispatch(kernelHandle, groupSize.x, groupSize.y, 1);
        Graphics.Blit(output, destination);
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            if (trackedObject && thisCamera)
            {
                Vector3 pos = thisCamera.WorldToScreenPoint(trackedObject.position);
                center.x = pos.x;
                center.y = pos.y;
                shader.SetVector("center", center);
            }
            bool resChange = false;
            CheckResolution(out resChange);
            if (resChange) SetProperties();
            DispatchWithSource(ref source, ref destination);
        }
    }

}
