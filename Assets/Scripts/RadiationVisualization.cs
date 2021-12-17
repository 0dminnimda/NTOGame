using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class RadiationVisualization : MonoBehaviour
{
    SpriteRenderer sr;
    //Sprite mySprite;
    //Texture2D tex;

    [SerializeField]
    RadiationEmitter re;

    [SerializeField]
    ComputeShader cum;

    [SerializeField]
    int resolution;
    [SerializeField]
    float scalar;

    [SerializeField]
    float low;
    [SerializeField]
    float high;

    [SerializeField]
    Color32 filling = new Color32(255, 0, 0, 255);
    [SerializeField]
    Color32 defaultColor = new Color32(255, 255, 255, 255);
    [SerializeField]
    Color32 transparent = new Color32(0, 0, 0, 0);

    RenderTexture rt;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Vector3 newVal = transform.localScale;
        newVal.x = re.maxDistance * 2;
        newVal.y = re.maxDistance * 2;
        transform.localScale = newVal;

        scalar = resolution / re.maxDistance / 2;

        // shader setup
        rt = new RenderTexture(resolution, resolution, 32);
        rt.enableRandomWrite = true;
        rt.Create();

        //tex = new Texture2D(resolution, resolution);

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = rt;

        cum.SetTexture(0, "result", rt);

        cum.SetFloat("resolution", resolution);
        cum.SetFloat("scalar", scalar);

        cum.SetFloat("low", low);
        cum.SetFloat("high", high);

        cum.SetVector("red", (Color)filling);
        cum.SetVector("defaultColor", (Color)defaultColor);
        cum.SetVector("defaultColor", (Color)transparent);

        cum.SetFloat("maxDistance", re.maxDistance * re.maxDistance);

        /*mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        sr.sprite = mySprite;*/
    }

    void Update()
    {
        if (re.dataHasChanged)
        {
            Redraw();
        }
    }

    void Redraw()
    {
        // RGBA32 texture format data layout exactly matches Color32 struct
        //var data = tex.GetRawTextureData<Color32>();

        ComputeBuffer pointBuffer = new ComputeBuffer(re.data.Count, sizeof(float) * 2);
        pointBuffer.SetData(re.data);

        cum.SetFloat("scalar", scalar);

        cum.SetFloat("low", low);
        cum.SetFloat("high", high);

        cum.SetVector("red", (Color)filling);
        cum.SetVector("defaultColor", (Color)defaultColor);

        cum.SetFloat("maxDistance", re.maxDistance * re.maxDistance);

        cum.SetBuffer(0, "points", pointBuffer);

        cum.Dispatch(0, rt.width / 8, rt.height / 8, 1);
        pointBuffer.Dispose();

        // upload to the GPU
        //rt.Apply();
    }
}
