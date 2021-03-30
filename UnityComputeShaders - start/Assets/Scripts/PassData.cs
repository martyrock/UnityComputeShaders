using UnityEngine;
using System.Collections;

public class PassData : MonoBehaviour
{

    public ComputeShader shader;
    public int texResolution = 1024;

    Renderer rend;
    RenderTexture outputTexture;

    int circlesHandle;
    int baseHandle;

    public Color clearColor = new Color();
    public Color circleColor = new Color();

    // Use this for initialization
    void Start()
    {
        outputTexture = new RenderTexture(texResolution, texResolution, 0);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();

        rend = GetComponent<Renderer>();
        rend.enabled = true;

        InitShader();
    }

    private void InitShader()
    {
        circlesHandle = shader.FindKernel("Circles");
        baseHandle = shader.FindKernel("BaseColor");

        shader.SetInt( "texResolution", texResolution);
        shader.SetVector("ClearColor", clearColor);
        shader.SetVector("CircleColor", circleColor);
        
        shader.SetTexture(baseHandle, "Result", outputTexture);
        shader.SetTexture( circlesHandle, "Result", outputTexture);

        rend.material.SetTexture("_MainTex", outputTexture);
    }
 
    private void DispatchKernels(int count)
    {
        shader.Dispatch(baseHandle, texResolution / 8, texResolution / 8, 1);
        shader.SetFloat("time", Time.time);
        shader.Dispatch(circlesHandle, count, 1, 1);
    }

    void Update()
    {
        DispatchKernels(10);
    }
}

