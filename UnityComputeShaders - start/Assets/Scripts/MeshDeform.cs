using UnityEngine;
using System.Collections;


public class MeshDeform : MonoBehaviour
{
    public ComputeShader shader;
    [Range(0.5f, 2.0f)]
	public float radius;
	
    int kernelHandle;
    Mesh mesh;

    public struct Vertex
    {
        public Vector3 position;
        public Vector3 normal;

        public Vertex(Vector3 position, Vector3 normal)
        {
            this.position.x = position.x;
            this.position.y = position.y;
            this.position.z = position.z;

            this.normal.x = normal.x;
            this.normal.y = normal.y;
            this.normal.z = normal.z;
        }
    }

    Vertex[] vertexArray;
    Vertex[] initialArray;
    ComputeBuffer vertexBuffer;
    ComputeBuffer initialBuffer;

    void Start()
    {
    
        if (InitData())
        {
            InitShader();
        }
    }

    private bool InitData()
    {
        kernelHandle = shader.FindKernel("CSMain");

        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf == null)
        {
            Debug.Log("No MeshFilter found");
            return false;
        }

        InitVertexArrays(mf.mesh);
        InitGPUBuffers();

        mesh = mf.mesh;

        return true;
    }

    private void InitShader()
    {
        shader.SetFloat("radius", radius);

    }

    private void InitVertexArrays(Mesh mesh)
    {
        vertexArray = new Vertex[mesh.vertices.Length];
        initialArray = new Vertex[mesh.vertices.Length];

        for(int i = 0; i < vertexArray.Length; i++)
        {
            Vertex v1 = new Vertex(mesh.vertices[i], mesh.normals[i]);
            vertexArray[i] = v1;
            Vertex v2 = new Vertex(mesh.vertices[i], mesh.normals[i]);
            initialArray[i] = v2;
        }
    }

    private void InitGPUBuffers()
    {
        vertexBuffer= new ComputeBuffer(vertexArray.Length, sizeof(float) * 6);
        vertexBuffer.SetData(vertexArray);

        initialBuffer = new ComputeBuffer(vertexArray.Length, sizeof(float) * 6);
        initialBuffer.SetData(initialArray);

        shader.SetBuffer(kernelHandle, "vertexBuffer", vertexBuffer);
        shader.SetBuffer(kernelHandle, "initialBuffer", initialBuffer);
    }
    
    void GetVerticesFromGPU()
    {
        vertexBuffer.GetData(vertexArray);
        Vector3[] vertices = new Vector3[vertexArray.Length];
        Vector3[] normals = new Vector3[vertexArray.Length];

        for(int i = 0; i < vertexArray.Length; i++)
        {
            vertices[i] = vertexArray[i].position;
            normals[i] = vertexArray[i].normal;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }

    void Update(){
        if (shader)
        {
        	shader.SetFloat("radius", radius);
            float delta = (Mathf.Sin(Time.time) + 1)/ 2;
            shader.SetFloat("delta", delta);
            shader.Dispatch(kernelHandle, vertexArray.Length, 1, 1);
            
            GetVerticesFromGPU();
        }
    }

    void OnDestroy()
    {
        
    }
}

