using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Robot;

//Ian Seidman Sorsby A01028650

public class Arm : MonoBehaviour
{
    string side;
    public void Init(string _side, ref List<GameObject> objParts, ref List<Matrix4x4> mLocations, ref List<Matrix4x4> mScales)
    {
        if(_side == "LEFT")
        {
            side = "LEFT";
            //L_SHOULDER
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LSHOULDER, Color.red, "LSHOULDER", new Vector3(0.4f, 0.4f, 0.4f), new Vector3(-1.2f / 2f - 0.4f / 2f, 0f, 0f));
            //L_FOREARM
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LFOREARM, Color.white, "LFOREARM", new Vector3(0.4f, 0.5f, 0.4f), new Vector3(0f, -0.4f / 2f - 0.5f /2f, 0f));
            //L_ELBOW
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LELBOW, Color.blue, "LELBOW", new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0f, -0.5f / 2f - 0.4f / 2f, 0f));
            //L_ARM
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LARM, Color.red, "LARM", new Vector3(0.4f, 0.3f, 0.4f), new Vector3(0f, -0.4f / 2f - 0.3f / 2f, 0f));
            //L_HAND
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LHAND, Color.white, "LHAND", new Vector3(0.5f, 0.3f, 0.4f), new Vector3(0f, -0.3f / 2f - 0.3f / 2f, 0f));
        
        }
        else if(_side == "RIGHT")
        {
            side = "RIGHT";
            //R_SHOULDER
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RSHOULDER, Color.red, "RSHOULDER", new Vector3(0.4f, 0.4f, 0.4f), new Vector3(1.2f / 2f + 0.4f / 2f, 0f, 0f));
            //R_FOREARM
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RFOREARM, Color.white, "RFOREARM", new Vector3(0.4f, 0.5f, 0.4f), new Vector3(0f, -0.4f / 2f - 0.5f /2f, 0f));
            //R_ELBOW
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RELBOW, Color.blue, "RELBOW", new Vector3(0.4f, 0.4f, 0.4f), new Vector3(0f, -0.5f / 2f - 0.4f / 2f, 0f));
            //R_ARM
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RARM, Color.red, "RARM", new Vector3(0.4f, 0.3f, 0.4f), new Vector3(0f, -0.4f / 2f - 0.3f / 2f, 0f));
            //R_HAND
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RHAND, Color.white, "RHAND", new Vector3(0.5f, 0.3f, 0.4f), new Vector3(0f, -0.3f / 2f - 0.3f / 2f, 0f));
        
        }
    }

    public void Draw(ref Matrix4x4 chestMatrix, ref List<GameObject> objParts, List<Matrix4x4> mLocations, List<Matrix4x4> mScales, BACK_FORTH rX, Vector3[] vOriginals)
    {
        Matrix4x4 accumT = Matrix4x4.identity;
        if(side == "LEFT")
        {
            for(int i = (int)PARTS.RP_LSHOULDER; i <= (int)PARTS.RP_LHAND; i++)
            {
                Matrix4x4 m = accumT * mLocations[i] * mScales[i];
                if(i == (int)PARTS.RP_LSHOULDER)
                {
                    accumT = chestMatrix;
                    m = accumT * mLocations[i] * mScales[i];
                    accumT *= mLocations[i];
                }
                else if(i == (int)PARTS.RP_LELBOW)
                {
                    Matrix4x4 t1 = Transformations.Translate(0f, -0.5f / 2f, 0f);
                    Matrix4x4 t2 = Transformations.Translate(0f, -0.4f /2f, 0f);
                    Matrix4x4 r = Transformations.RotateX(-rX.val);
                    m = accumT * t1 * r * t2 * mScales[i];
                    accumT *= t1 * r * t2;
                }
                else
                    accumT *= mLocations[i];
                objParts[i].GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(m, vOriginals);
            }
        }
        else if(side == "RIGHT")
        {
            for(int i = (int)PARTS.RP_RSHOULDER; i <= (int)PARTS.RP_RHAND; i++)
            {
                Matrix4x4 m = accumT * mLocations[i] * mScales[i];
                if(i == (int)PARTS.RP_RSHOULDER)
                {
                    accumT = chestMatrix;
                    m = accumT * mLocations[i] * mScales[i];
                    accumT *= mLocations[i];
                }
                else if(i == (int)PARTS.RP_RELBOW)
                {
                    Matrix4x4 t1 = Transformations.Translate(0f, -0.5f / 2f, 0f);
                    Matrix4x4 t2 = Transformations.Translate(0f, -0.4f /2f, 0f);
                    Matrix4x4 r = Transformations.RotateX(rX.val);
                    m = accumT * t1 * r * t2 * mScales[i];
                    accumT *= t1 * r * t2;
                }
                else
                    accumT *= mLocations[i];
                objParts[i].GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(m, vOriginals);
            }
        }
    }
}
