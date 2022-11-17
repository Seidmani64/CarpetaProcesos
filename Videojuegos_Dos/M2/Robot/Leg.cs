using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Robot;

//Ian Seidman Sorsby A01028650

public class Leg : MonoBehaviour
{
    string side;
    public void Init(string _side, ref List<GameObject> objParts, ref List<Matrix4x4> mLocations, ref List<Matrix4x4> mScales)
    {
        if(_side == "LEFT")
        {
            side = "LEFT";
            //L_THIGH
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LTHIGH, Color.white, "LTHIGH", new Vector3(0.4f, 0.6f, 0.4f), new Vector3(- 1f / 2f + 0.4f /2f, -0.5f /2f - 0.6f / 2f, 0f));
            //L_LEG
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LLEG, Color.blue, "LLEG", new Vector3(0.4f, 0.6f, 0.4f), new Vector3(0f, -0.6f / 2f - 0.6f / 2f, 0f));
            //L_FOOT
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_LFOOT, Color.blue, "LFOOT", new Vector3(0.4f, 0.2f, 0.6f), new Vector3(0f, -0.6f / 2f - 0.2f / 2f, 0.4f / 2f - 0.6f / 2f));
            
        
        }
        else if(_side == "RIGHT")
        {
            side = "RIGHT";
            //R_THIGH
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RTHIGH, Color.white, "RTHIGH", new Vector3(0.4f, 0.6f, 0.4f), new Vector3(1f / 2f - 0.4f /2f, -0.5f /2f - 0.6f / 2f, 0f));
            //R_LEG
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RLEG, Color.blue, "RLEG", new Vector3(0.4f, 0.6f, 0.4f), new Vector3(0f, -0.6f / 2f - 0.6f / 2f - 2f, 0f));
            //R_FOOT
            INIT_PART(ref objParts, ref mLocations, ref mScales, PrimitiveType.Cube, (int)PARTS.RP_RFOOT, Color.blue, "RFOOT", new Vector3(0.4f, 0.2f, 0.6f), new Vector3(0f, -0.6f / 2f - 0.2f / 2f, 0.4f / 2f - 0.6f / 2f));
        }
    }

    public void Draw(ref Matrix4x4 hipMatrix, ref List<GameObject> objParts, List<Matrix4x4> mLocations, List<Matrix4x4> mScales, BACK_FORTH rX, Vector3[] vOriginals)
    {
        Matrix4x4 accumT = Matrix4x4.identity;
        if(side == "LEFT")
        {
            for(int i = (int)PARTS.RP_LTHIGH; i <= (int)PARTS.RP_LFOOT; i++)
            {
                Matrix4x4 m = accumT * mLocations[i] * mScales[i];
                if(i == (int)PARTS.RP_LTHIGH)
                {
                    accumT = hipMatrix;
                    m = accumT * mLocations[i] * mScales[i];
                    accumT *= mLocations[i];
                }
                else if(i == (int)PARTS.RP_LLEG)
                {
                    Matrix4x4 t1 = Transformations.Translate(0f, -0.6f / 2f, 0f);
                    Matrix4x4 t2 = Transformations.Translate(0f, -0.6f /2f, 0f);
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
            for(int i = (int)PARTS.RP_RTHIGH; i <= (int)PARTS.RP_RFOOT; i++)
            {
                Matrix4x4 m = accumT * mLocations[i] * mScales[i];
                if(i == (int)PARTS.RP_RTHIGH)
                {
                    accumT = hipMatrix;
                    m = accumT * mLocations[i] * mScales[i];
                    accumT *= mLocations[i];
                }
                else if(i == (int)PARTS.RP_RLEG)
                {
                    Matrix4x4 t1 = Transformations.Translate(0f, -0.6f / 2f, 0f);
                    Matrix4x4 t2 = Transformations.Translate(0f, -0.6f /2f, 0f);
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
