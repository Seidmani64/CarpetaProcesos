using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Script de Ian Seidman y Carlos Cordoba

public class RayCaster : MonoBehaviour
{
    public int spheres;
    public Vector3 cameraLocation;
    public Vector3 spotLight;
    public Vector2 cameraSize;
    public Vector3 Ia;
    public Vector3 Id;
    public Vector3 Is;
    public List<Vector3> kd;
    public List<Vector3> ka;
    public List<Vector3> ks;
    public List<float> alphas;
    public List<float> radiuses;
    public List<Vector3> centers;
    public Vector3 PoI;
    public float SR;
    public Vector3 SC;

    public Vector3 contact;

    private Camera mainCam;
    public Vector3 n;
    public Texture2D background;
    public Renderer renderer;
    public GameObject plane;

    // Start is called before the first frame update
    void Start()
    {
        Ia = new Vector3(0.7f, 0.7f, 0.7f);
        Id = new Vector3(0.8f, 0.8f, 1f);
        Is = new Vector3(1f, 1f, 1f);

        for(int j = 0; j < spheres; j++)
        {
            kd.Add(new Vector3(Random.Range(0.5f, 1f), (Random.Range(0.5f, 1f)), Random.Range(0.5f, 1f)));
            ka.Add(new Vector3(kd[j].x/5f, kd[j].y/5f, kd[j].z/5f));
            ks.Add(new Vector3(kd[j].x/3f, kd[j].y/3f, kd[j].z/3f));
            alphas.Add(Random.Range(500.0f, 600.0f));
            radiuses.Add(Random.Range(0.1f, 0.35f));
            centers.Add(new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(2.0f, 6.0f), Random.Range(8.0f, 10.0f)));

        }
        
        //Order by centers


        mainCam = Camera.main;
        //Vector3 i = Illumination(PoI, 0);
        
        // Create sphere
        for(int j = 0; j < spheres; j++)
        {
            GameObject sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sph.transform.position = centers[j];
            SR = radiuses[j];
            sph.transform.localScale = new Vector3(SR*2f, SR*2f, SR*2f);
            Renderer rend = sph.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            rend.material.SetColor("_Color", new Color(kd[j].x, kd[j].y, kd[j].z));
            rend.material.SetColor("_SpecColor", new Color(ks[j].x, ks[j].y, ks[j].z));
        }
        
        mainCam.transform.position = cameraLocation;
        //mainCam.transform.LookAt(SC);

        // Create pointLight
        GameObject pointLight = new GameObject("ThePointLight");
        Light lightComp = pointLight.AddComponent<Light>();
        lightComp.type = LightType.Point;
        lightComp.color = new Color(Id.x, Id.y, Id.z);
        lightComp.intensity = 20;
        pointLight.transform.localPosition = spotLight;


        float frusttumHeight = 2.0f * mainCam.nearClipPlane * Mathf.Tan(mainCam.fieldOfView * Mathf.Deg2Rad);
        float frustumWidth = frusttumHeight * mainCam.aspect;
        var texture = new Texture2D(640, 480, TextureFormat.ARGB32, false);

        for (int x = 0; x < cameraSize.x; x++)
        {
            for (int y = 0; y < cameraSize.y; y++)
            {
                Color bg = background.GetPixelBilinear(x/640f, y/480f);
                texture.SetPixel(x, y, bg);
            }
        }
        texture.Apply();
        
            for (int x = 0; x < cameraSize.x; x++)
            {
            for (int y = 0; y < cameraSize.y; y++)
                {
                    for(int j = 0; j < spheres; j++)
                    {
                        if(Cast(new Vector3(x, y, 0f), j))
                        {
                            Color color = GetPixel(new Vector3(x, y, 0f), j);
                            texture.SetPixel(x, 480-y, color);
                        }
                    }
                    
                }
            }
        
        
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        renderer = plane.GetComponent<Renderer>();
        Shader shader = Shader.Find("Unlit/Texture");
        renderer.material.shader = shader;
        renderer.material.mainTexture = texture;
        byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../render.png", bytes);
    }

    Vector3 Illumination(Vector3 PoI, int idx)
    {
        Debug.Log(PoI.ToString("F5"));
        Vector3 A = new Vector3(ka[idx].x * Ia.x, ka[idx].y * Ia.y, ka[idx].z * Ia.z);
        Vector3 D = new Vector3(kd[idx].x * Id.x, kd[idx].y * Id.y, kd[idx].z * Id.z);
        Vector3 S = new Vector3(ks[idx].x * Is.x, ks[idx].y * Is.y, ks[idx].z * Is.z);

        Vector3 l = spotLight - PoI;
        Vector3 v = cameraLocation - PoI;
        n = PoI - centers[idx];
        float dotNuLu = Vector3.Dot(n.normalized, l.normalized);
        float dotNuL = Vector3.Dot(n.normalized, l);

        Vector3 lp = n.normalized * dotNuL;
        Vector3 lo = l - lp;
        Vector3 r = lp-lo;
        D *= dotNuLu;
        float sCalc = Mathf.Pow(Vector3.Dot(v.normalized,r.normalized),alphas[idx]);
        if(sCalc is float.NaN){sCalc = 0f;}
        S *= sCalc;
        
        return A + D + S;
    }

    Vector3 FindTopLeftFrusrtumNear()
    {
        //localizar camara
        Vector3 o = mainCam.transform.position;
        //mover hacia adelante
        Vector3 p = o + mainCam.transform.forward * mainCam.nearClipPlane;
        //obtener dimenciones del frustum
        float frusttumHeight = 2.0f * mainCam.nearClipPlane * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frusttumHeight * mainCam.aspect;
        //mover hacia arriba, media altura
        p += mainCam.transform.up * frusttumHeight / 2.0f;
        //mover a la izquierda, medio ancho
        p -= mainCam.transform.right * frustumWidth / 2.0f;
        return p;

    }

    bool Cast(Vector3 coords, int idx)
    {
        float frusttumHeight = 2.0f * mainCam.nearClipPlane * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frusttumHeight * mainCam.aspect;
        float pixelWidth = frustumWidth / cameraSize.x;
        float pixelHeight = frusttumHeight / cameraSize.y;
        Vector3 center = FindTopLeftFrusrtumNear();
        center += +(pixelWidth / 2f) * mainCam.transform.right;
        center -= (pixelHeight / 2f) * mainCam.transform.up;
        center += +(pixelWidth) * mainCam.transform.right * coords.x;
        center -= (pixelHeight) * mainCam.transform.up * coords.y;


        Vector3 u = (center - mainCam.transform.position).normalized;
        Vector3 oc = mainCam.transform.position - centers[idx];

        float delta = (Mathf.Pow(Vector3.Dot(u, oc), 2) - (Mathf.Pow(oc.magnitude, 2) - Mathf.Pow(radiuses[idx], 2)));
        if(delta < 0)
        {
            return false;
        }
        return true;
    }
    Color GetPixel(Vector3 coords, int idx)
    {

        float frusttumHeight = 2.0f * mainCam.nearClipPlane * Mathf.Tan(mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frusttumHeight * mainCam.aspect;
        float pixelWidth = frustumWidth / cameraSize.x;
        float pixelHeight = frusttumHeight / cameraSize.y;
        Vector3 center = FindTopLeftFrusrtumNear();
        center += +(pixelWidth / 2f) * mainCam.transform.right;
        center -= (pixelHeight / 2f) * mainCam.transform.up;
        center += +(pixelWidth) * mainCam.transform.right * coords.x;
        center -= (pixelHeight) * mainCam.transform.up * coords.y;


        Vector3 u = (center - mainCam.transform.position).normalized;
        Vector3 oc = mainCam.transform.position - centers[idx];

        float delta = (Mathf.Pow(Vector3.Dot(u, oc), 2) - (Mathf.Pow(oc.magnitude, 2) - Mathf.Pow(radiuses[idx], 2)));
        if(delta < 0)
        {
            return Color.white;
        }

        
        float d1 = -Vector3.Dot(u, oc) + Mathf.Sqrt(delta);
        float d2 = -Vector3.Dot(u, oc) - Mathf.Sqrt(delta);

        float d = 0;
        if(Mathf.Abs(d1) < Mathf.Abs(d2))
            d = d1;
        else
            d = d2;
        
        Vector3 color = Illumination(mainCam.transform.position + (d * u), idx);

        return new Color(color.x, color.y, color.z);
    }

}
