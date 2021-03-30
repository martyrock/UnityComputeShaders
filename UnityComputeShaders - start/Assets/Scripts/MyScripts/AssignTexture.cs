using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTexture : MonoBehaviour
{
    public ComputeShader shader;
    public int texResolution = 256;
    public string kernelName = "SplitScreen";

    Renderer rend;
    RenderTexture outputTexture;
    int kernelHandle;

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
        kernelHandle = shader.FindKernel(kernelName);
        shader.SetTexture(kernelHandle, "Result", outputTexture);
        shader.SetInt("TexResolution", texResolution);
        shader.SetFloats("Color", new float[] { 1.0f, 0.0f, 0.0f, 1.0f});
        rend.material.SetTexture("_MainTex", outputTexture);
        DispatchShader(texResolution / 8, texResolution / 8);
    }

    private void DispatchShader(int x, int y)
    {
        shader.Dispatch(kernelHandle, x, y, 1);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            DispatchShader(texResolution / 8, texResolution / 8);
        }
    }
}
