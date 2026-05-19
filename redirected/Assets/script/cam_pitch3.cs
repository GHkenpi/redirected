using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;


public class cam_pitch3 : MonoBehaviour
{
    public Quaternion cameraRotationQ;

    public Vector3 cameraRotation;


    void Start()
    {
       
    }

    void Update()
    {
        pitch3 pitch3;
        GameObject obj = GameObject.Find("Camera"); //Playerっていうオブジェクトを探す
        pitch3 = obj.GetComponent<pitch3>();

        cameraRotationQ = this.transform.rotation;
        //取得した値をクォータニオン → オイラー角に変換
        cameraRotation = cameraRotationQ.eulerAngles;
        // cameraの位置を更新
        //this.transform.position = newPosition;
        Quaternion newRotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, -pitch3.addRotation);
        //Debug.Log("aftar:"+ newRotation.eulerAngles.x);
        this.transform.rotation = newRotation;
            
        
    }
}