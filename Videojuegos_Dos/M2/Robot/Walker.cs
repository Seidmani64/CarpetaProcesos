using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ian Seidman Sorsby A01028650

public class Walker : MonoBehaviour
{
    Robot r;
    // Start is called before the first frame update
    void Start()
    {
        r = gameObject.AddComponent<Robot>();
        r.Init();
    }

    // Update is called once per frame
    void Update()
    {
        r.Draw();
    }
}
