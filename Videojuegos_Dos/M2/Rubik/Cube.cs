using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    List<GameObject> cubes;
    List<Vector3> positions;
    Vector3[] originals;
    List<Matrix4x4> matrices, originalMatrices;
    float rotZ, rotY;
    // Start is called before the first frame update
    void Start()
    {
        rotZ = 0;
        cubes = new List<GameObject>();
        positions = new List<Vector3>();
        originalMatrices = new List<Matrix4x4>();
        matrices = new List<Matrix4x4>();

        for(int i = 0; i < 8; i++)
        {
            cubes.Add(GameObject.CreatePrimitive(PrimitiveType.Cube));
        }
        
        float s = 1.0f;
        float h = s / 2.0f;
        

        positions.Add(new Vector3(-h,-h,h));
        positions.Add(new Vector3(h,-h,h));
        positions.Add(new Vector3(h,h,h));
        positions.Add(new Vector3(-h,h,h));
        positions.Add(new Vector3(-h,-h,-h));
        positions.Add(new Vector3(h,-h,-h));
        positions.Add(new Vector3(h,h,-h));
        positions.Add(new Vector3(-h,h,-h));

        originals = cubes[0].GetComponent<MeshFilter>().mesh.vertices;

        for(int i = 0; i < cubes.Count; i++)
        {
            originalMatrices.Add(Transformations.Translate(positions[i].x, positions[i].y, positions[i].z));
            matrices.Add(originalMatrices[i]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(rotZ < 360.0f)
        {
            rotZ += 0.1f;
            for(int i = 4; i < 8; i++)
            {
                matrices[i] = Transformations.RotateZ(rotZ) * originalMatrices[i];
            }
        }
        else if(rotY < 360.0f)
        {
            rotY += 0.1f;
            for(int i = 0; i < 8; i++)
            {
                if(i == 2 || i == 3 || i == 6 || i == 7)
                    matrices[i] = Transformations.RotateY(rotY) * originalMatrices[i];
            }
        }
        else
        {
            rotZ = 0f;
            rotY = 0f;
        }
        for(int i = 0; i < 8; i++)
        {
            cubes[i].GetComponent<MeshFilter>().mesh.vertices = Transformations.Transform(matrices[i], originals);

        }
    }
}
