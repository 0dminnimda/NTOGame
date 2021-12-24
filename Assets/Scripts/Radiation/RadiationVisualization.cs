using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Consts;
using System;

namespace Radiation
{
    public struct RayData
    {
        public Vector2 v;
        public int ptsUsed;

        public RayData(Vector2 v, int l)
        {
            this.v = v;
            ptsUsed = Math.Min(l, LEN);
        }
    };

    public struct Point
    {
        public Vector2 v;
        public float radiationLvl;

        public Point(Vector2 v, float radiationLvl)
        {
            this.v = v;
            this.radiationLvl = radiationLvl;
        }
    };

    [RequireComponent(typeof(SpriteRenderer))]
    public class RadiationVisualization : MonoBehaviour
    {
        SpriteRenderer sr;
        //Sprite mySprite;
        //Texture2D tex;

        [SerializeField] RadiationEmitter re;

        [SerializeField] ComputeShader cum;

        [SerializeField] int resolution = 1024;
        [SerializeField] float scalar;

        [SerializeField] Color32 filling = new Color32(255, 0, 0, 255);

        // [SerializeField]
        // Color32 defaultColor = new Color32(255, 255, 255, 255);
        [SerializeField] Color32 transparent = new Color32(0, 0, 0, 0);

        [SerializeField] float minAmountOfAlpha = 0.03f;

        RenderTexture rt;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            // cum = (ComputeShader)Instantiate(Resources.Load(@"C:\C#\projects\NTOGame\Assets\Shaders\ShadowEffect.compute"));
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

            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = rt;

            cum.SetTexture(0, "result", rt);

            cum.SetFloat("resolution", resolution);
            cum.SetFloat("scalar", scalar);

            cum.SetVector("filling", (Color) filling);
            cum.SetVector("defaultColor", (Color) filling);
            cum.SetVector("transparent", (Color) transparent);

            cum.SetFloat("maxDistance", re.maxDistance * re.maxDistance);
            cum.SetFloat("initRadLvl", re.basicRadiationLevel);
            cum.SetFloat("minAmountOfAlpha", minAmountOfAlpha);

            //tex = new Texture2D(resolution, resolution);

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
            // var data = tex.GetRawTextureData<Color32>();

            ComputeBuffer rayBuffer = new ComputeBuffer(re.data.Count, sizeof(float) * 2 + sizeof(int));
            rayBuffer.SetData(re.data);
            cum.SetBuffer(0, "rays", rayBuffer);

            ComputeBuffer pointBuffer = new ComputeBuffer(re.pointData.Count, sizeof(float) * (2 + 1));
            pointBuffer.SetData(re.pointData);
            cum.SetBuffer(0, "points", pointBuffer);

            cum.SetVector("filling", (Color) filling);
            cum.SetVector("defaultColor", (Color) filling);
            cum.SetVector("transparent", (Color) transparent);

            cum.Dispatch(0, rt.width / 8, rt.height / 8, 1);

            rayBuffer.Release();
            pointBuffer.Release();

            // upload to the GPU
            //tex.Apply();
        }
    }
}
