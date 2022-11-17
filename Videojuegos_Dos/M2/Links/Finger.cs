using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ian Seidman Sorsby A01028650

public class Finger : MonoBehaviour
{
    GameObject link1, link2, link3;
    Vector3[] vLink1, vLink2, vLink3;
    float rotZ;
    bool isUp, isDown;
    // Start is called before the first frame update
    void Start()
    {
        isUp = true;
        isDown = false;
        rotZ = 0f;
        link1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        link2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        link3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vLink1 = link1.GetComponent<MeshFilter>().mesh.vertices;
        vLink2 = link2.GetComponent<MeshFilter>().mesh.vertices;
        vLink3 = link3.GetComponent<MeshFilter>().mesh.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        if(isUp)
        {
            rotZ += 0.1f;
            if(rotZ >= 45.0f)
            {
                isUp = false;
                isDown = true;
            }
        }
        else if(isDown)
        {
            rotZ -= 0.1f;
            if(rotZ <= -45.0f)
            {
                isUp = true;
                isDown = false;
            }
        }
        Matrix4x4 rotate = Transformations.RotateZ(rotZ);
        Matrix4x4 link1Pos = Transformations.Translate(0.5f,0,0);
        Matrix4x4 scale = Transformations.Scale(1.0f,0.5f,0.5f);

        Matrix4x4 tMinusP = Transformations.Translate(-0.5f,0,0);
        Matrix4x4 tPlusP = link1Pos;
        Matrix4x4 rotatePivot1 = tPlusP * rotate * tMinusP * link1Pos * link1Pos;

        Matrix4x4 tMinusP2 = Transformations.Translate(-0.5f,0,0);
        Matrix4x4 tPlusP2 = Transformations.Translate(0.5f,0,0);
        Matrix4x4 rotatePivot2 =  tPlusP2 * rotate * tMinusP2 * link1Pos * link1Pos;

        Matrix4x4 tMinusP3 = Transformations.Translate(-0.5f,0,0);
        Matrix4x4 tPlusP3 = Transformations.Translate(0.5f,0,0);
        Matrix4x4 rotatePivot3 =  tPlusP3 * rotate * tMinusP3 * link1Pos * link1Pos;

        link1.GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(rotatePivot1 * scale, vLink1);
        link2.GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(rotatePivot1 * rotatePivot2 * scale, vLink2);
        link3.GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(rotatePivot1 * rotatePivot2 * rotatePivot3 * scale, vLink3);
        
    }
}
