using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class RadiationVisualization : MonoBehaviour
{
    SpriteRenderer sr;
    Sprite mySprite;

    [SerializeField]
    RadiationEmitter re;

    [SerializeField]
    int resolution;
    Texture2D tex;

    [SerializeField]
    float low;
    [SerializeField]
    float high;

    [SerializeField]
    float scalar = 10f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        tex = new Texture2D(resolution, resolution);
        mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        sr.sprite = mySprite;

        /*Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = tex;*/
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
        List<Vector2> points = new List<Vector2>();

        foreach ((Vector2, Vector2[]) item in re.data)
        {
            points.AddRange(item.Item2);
        }

        // RGBA32 texture format data layout exactly matches Color32 struct
        var data = tex.GetRawTextureData<Color32>();

        // fill texture data with a simple pattern
        var red = new Color32(255, 0, 0, 255);
        var defaultColor = new Color32(255, 255, 255, 255);
        Color32 color;

        int index = 0;
        var coord = new Vector2();
        for (int y = 0; y < resolution; y++)
        {
            coord.y = y - resolution / 2f + 0.5f;
            coord.y /= scalar;

            for (int x = 0; x < resolution; x++)
            {
                coord.x = x - resolution / 2f + 0.5f;
                coord.x /= scalar;

                var min = points.Select(x => (coord - x).sqrMagnitude).Min();

                //Debug.LogFormat("{0}, {1}", coord, min);
                if (low <= min && min <= high)
                {
                    color = red;
                }
                else
                    color = defaultColor;

                data[index++] = color;
            }
        }

        // upload to the GPU
        tex.Apply();
    }
}
