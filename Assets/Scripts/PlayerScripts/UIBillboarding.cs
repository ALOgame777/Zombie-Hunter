using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIBillboarding : MonoBehaviour
{
    public Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
   


    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
