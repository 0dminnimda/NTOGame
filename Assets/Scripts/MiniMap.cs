using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Camera mapCam;
    public RawImage img;
    private RenderTexture tex;
    public float mul = 1;

    void Awake()
    {
        tex = new RenderTexture((int)(160 * mul), (int)(90 * mul), 24);
        img.texture = tex;
        mapCam.targetTexture = tex;
    }
}
