using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;


public class pitch0 : MonoBehaviour
{
    //HMDの回転座標格納用（クォータニオン）
    public Quaternion HMDRotationQ;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Update()
    {
        HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
        this.transform.rotation = HMDRotationQ;
    }
}