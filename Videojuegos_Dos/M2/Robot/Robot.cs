using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//Ian Seidman Sorsby A01028650

public class Robot : MonoBehaviour
{

    public enum PARTS
    {

        RP_HIP, RP_TORSO, RP_CHEST, RP_NECK, RP_HEAD,
        RP_RSHOULDER, RP_RFOREARM, RP_RELBOW, RP_RARM, RP_RHAND,
        RP_LSHOULDER, RP_LFOREARM, RP_LELBOW, RP_LARM, RP_LHAND,
        RP_RTHIGH, RP_RLEG, RP_RFOOT,
        RP_LTHIGH, RP_LLEG, RP_LFOOT
    };

    public struct BACK_FORTH
    {
        public float delta, dir, val, min, max;
        public BACK_FORTH(float _delta, float _dir, float _val, float _min, float _max)
        {
            delta = _delta;
            dir = _dir;
            val = _val;
            min = _min;
            max = _max;
        }

        public void Update()
        {
            val += delta * dir;
            if(val <= min || val >= max)
                dir = -dir;
        }
    };

    public static void INIT_PART(
                            ref List<GameObject> parts,
                            ref List<Matrix4x4> locations,
                            ref List<Matrix4x4> scales,
                            PrimitiveType type,
                            int index,
                            Color color,
                            string name,
                            Vector3 scale,
                            Vector3 position)
    {
        parts.Add(GameObject.CreatePrimitive(type));
        parts[index].GetComponent<MeshRenderer>().material.SetColor("_Color", color);
        parts[index].name = name;
        parts[index].GetComponent<BoxCollider>().enabled = false;
        scales.Add(Transformations.Scale(scale.x, scale.y, scale.z));
        locations.Add(Transformations.Translate(position.x, position.y, position.z));
    } 

    List<GameObject> objParts;
    List<Matrix4x4> mLocations;
    List<Matrix4x4> mScales;
    List<Matrix4x4> mRotations;
    Vector3[] vOriginals;
    BACK_FORTH rX;
    BACK_FORTH rY;
    BACK_FORTH jump;

    Arm leftArm;
    Arm rightArm;
    Leg leftLeg;
    Leg rightLeg;

    public void Init()
    {
        leftArm = gameObject.AddComponent<Arm>();
        rightArm = gameObject.AddComponent<Arm>();
        leftLeg = gameObject.AddComponent<Leg>();
        rightLeg = gameObject.AddComponent<Leg>();
        rX = new BACK_FORTH(0.02f, 1f, 0f, 0f, 8f);
        rY = new BACK_FORTH(0.02f, 1f, 0f, -3f, 3f);
        jump = new BACK_FORTH(0.0005f, 1f, 0f, -0.05f, 0.05f);
        objParts = new List<GameObject>();
        mLocations = new List<Matrix4x4>();
        mScales = new List<Matrix4x4>();

        //HIP
        INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_HIP, Color.gray, "HIP", new Vector3(1f, 0.5f, 1f), new Vector3(0f, 0f, 0f));
        vOriginals = objParts[(int)PARTS.RP_HIP].GetComponent<MeshFilter>().mesh.vertices;
        //TORSO
        INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_TORSO, Color.white, "TORSO", new Vector3(1f, 0.75f, 1f), new Vector3(0f, 0.25f + 0.75f/2f, 0f));
        //CHEST
        INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_CHEST, Color.red, "CHEST", new Vector3(1.2f, 0.4f, 1.2f), new Vector3(0f, 0.75f/2f + 0.2f, 0f));
        //NECK
        INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_NECK, Color.white, "NECK", new Vector3(0.2f, 0.1f, 0.2f), new Vector3(0f, 0.2f + 0.1f/2f, 0f));
        //HEAD
        INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_HEAD, Color.blue, "HEAD", new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0f, 0.1f/2f + 0.5f/2f, 0f));

        rightArm.Init("RIGHT", ref objParts, ref mLocations, ref mScales);
        leftArm.Init("LEFT", ref objParts, ref mLocations, ref mScales);
        rightLeg.Init("RIGHT", ref objParts, ref mLocations, ref mScales);
        leftLeg.Init("LEFT", ref objParts, ref mLocations, ref mScales);
        
      
    }

    public void Draw()
    {
        rX.Update();
        rY.Update();
        jump.Update();
        Matrix4x4 accumT = Matrix4x4.identity;
        Matrix4x4 accumChest = Matrix4x4.identity;
        Matrix4x4 accumHip = Matrix4x4.identity;
        for(int i = (int)PARTS.RP_HIP; i <= (int)PARTS.RP_HEAD; i++)
        {
            Matrix4x4 m = accumT * mLocations[i] * mScales[i];
            if(i == (int)PARTS.RP_HIP)
            {
                Matrix4x4 t = Transformations.Translate(0f, jump.val, 0f);
                m = accumT * mLocations[i] * t * mScales[i];
                accumT *= mLocations[i] * t;
                accumHip = accumT;
                rightLeg.Draw(ref accumHip, ref objParts, mLocations, mScales, rX, vOriginals);
                leftLeg.Draw(ref accumHip, ref objParts, mLocations, mScales, rX, vOriginals);
            }
            else if(i == (int)PARTS.RP_CHEST)
            {
                Matrix4x4 r = Transformations.RotateY(rY.val);
                m = accumT * mLocations[i] * r * mScales[i];
                accumT *= mLocations[i] * r;
                accumChest = accumT;
                rightArm.Draw(ref accumChest, ref objParts, mLocations, mScales, rX, vOriginals);
                leftArm.Draw(ref accumChest, ref objParts, mLocations, mScales, rX, vOriginals);
            }
            else if(i == (int)PARTS.RP_NECK)
            {
                Matrix4x4 r = Transformations.RotateY(-rY.val);
                m = accumT * mLocations[i] * r * mScales[i];
                accumT *= mLocations[i] * r;
            }
            else
                accumT *= mLocations[i];
            objParts[i].GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(m, vOriginals);
            
        }
    }

}