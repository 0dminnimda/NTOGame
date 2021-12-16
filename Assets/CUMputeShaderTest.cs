using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUMputeShaderTest : MonoBehaviour
{
    public ComputeShader cum;
    public RenderTexture rt;

    [SerializeField]
    RadiationEmitter re;
    Vector2[] data = { };

    public int resolution = 256;
    public float scalar;

    public float low;
    public float high;

    public Color32 red = new Color32(255, 0, 0, 255);
    public Color32 defaultColor = new Color32(255, 255, 255, 255);

    /*void Start()
    {
        rt = new RenderTexture(256, 256, 24);
        rt.enableRandomWrite = true;
        rt.Create();

        cum.SetTexture(0, "Result", rt);
        cum.Dispatch(0, rt.width / 8, rt.height / 8, 1);
    }*/

    void Update()
    {
        if (re.dataHasChanged)
        {
            List<Vector2> points = new List<Vector2>();

            foreach ((Vector2, Vector2[]) item in re.data)
            {
                points.AddRange(item.Item2);
            }

            data = points.ToArray();
        }
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (rt == null)
        {
            rt = new RenderTexture(resolution, resolution, 24);
            rt.enableRandomWrite = true;
            rt.Create();
        }

        ComputeBuffer pointBuffer = new ComputeBuffer(data.Length, sizeof(float) * 2);
        pointBuffer.SetData(data);

        cum.SetBuffer(0, "points", pointBuffer);
        cum.SetTexture(0, "result", rt);

        cum.SetFloat("resolution", resolution);
        cum.SetFloat("scalar", scalar);

        cum.SetFloat("low", low);
        cum.SetFloat("high", high);

        cum.SetVector("red", (Color)red);
        cum.SetVector("defaultColor", (Color)defaultColor);

        cum.Dispatch(0, rt.width / 8, rt.height / 8, 1);
        pointBuffer.Dispose();

        Graphics.Blit(rt, destination);
    }
}
