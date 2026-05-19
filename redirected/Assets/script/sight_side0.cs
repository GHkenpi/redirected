using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class sight_side0 : MonoBehaviour
{
    //HMDの位置座標格納用
    public Vector3 HMDPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float cos = (float)Math.Cos(10 * (Math.PI / 180)); //坂道の斜度

        /*InputTracking.GetLocalPosition(XRNode.機器名)で機器の位置や向きを呼び出せる*/

        //Head（ヘッドマウントディスプレイ）の情報を一時保管-----------
        //位置座標を取得
        HMDPosition = InputTracking.GetLocalPosition(XRNode.Head);

        // 下向きにRaycastを飛ばす。Physics.Raycast(始点, 方向, out RaycastHit hitInfo)
        RaycastHit hit;
        if (Physics.Raycast(HMDPosition, Vector3.down, out hit))
        {
            // Rayが何かに当たった場合
            // 当たった地点（地面）のY座標を取得
            float groundY = hit.point.y;// HMDのY座標を地面のY座標に変更
            Debug.Log(groundY);

            if (HMDPosition.x > 0)
            {
                // 新しい位置を計算
                Vector3 newPosition = new Vector3(HMDPosition.x * cos, HMDPosition.y + groundY, HMDPosition.z + 2);
                this.transform.position = newPosition;
            }
            else
            {
                Vector3 newPosition = new Vector3(HMDPosition.x, HMDPosition.y, HMDPosition.z + 2);
                this.transform.position = newPosition;
            }
        }
    }
}
