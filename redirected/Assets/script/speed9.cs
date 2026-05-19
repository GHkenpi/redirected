using System;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;


public class speed9 : MonoBehaviour
{
    //HMDの位置座標格納用
    public Vector3 HMDPosition;
    //HMDの回転座標格納用（クォータニオン）
    //public Quaternion HMDRotationQ;
    //HMDの回転座標格納用（オイラー角）
    //public Vector3 HMDRotation;

    // Inspectorから設定できるようにTransformを宣言（Findを使うより高速で確実）
    public Transform targetTransform;

    // Z軸の回転角度を代入する変数
    private float gradient;

    // Start is called before the first frame update
    void Start()
    {
        // オブジェクトが未設定の場合、名前で検索を試みる
        if (targetTransform == null)
        {
            GameObject planeObject = GameObject.Find("Plane (3)");
            if (planeObject != null)
            {
                targetTransform = planeObject.transform;
            }
            else
            {
                Debug.LogError("'Plane (3)'が見つかりません。InspectorでTransformを設定してください。");
                enabled = false; // スクリプトのUpdateを停止
            }
        }
    }

    void Update()
    {
        if (targetTransform != null)
        {
            // 1. ワールド回転のオイラー角 (Vector3) を取得
            Vector3 worldEuler = targetTransform.eulerAngles;

            // 2. そのZ軸の値 (float) を取得し、int型にキャストして代入
            // 注意: eulerAnglesのZの値は0.0～360.0の範囲で変動します。
            gradient = (float)worldEuler.z;

            // デバッグ表示（確認用、不要なら削除可）
            Debug.Log("Current Gradient (Z-Rotation): " + gradient);
        }
        float cos = (float)Math.Cos(gradient * (Math.PI / 180)); //坂道の斜度

        /*InputTracking.GetLocalPosition(XRNode.機器名)で機器の位置や向きを呼び出せる*/

        //Head（ヘッドマウントディスプレイ）の情報を一時保管-----------
        //位置座標を取得
        HMDPosition = InputTracking.GetLocalPosition(XRNode.Head);
        //回転座標をクォータニオンで値を受け取る
        //HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
        //取得した値をクォータニオン → オイラー角に変換
        //HMDRotation = HMDRotationQ.eulerAngles;


        // ------------------------------------------------------------------
        // ここから追加コード
        // Raycastを飛ばすために、現在のHMDの位置を取得
        Vector3 currentHMDPos = this.transform.position;

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
                Vector3 newPosition = new Vector3(HMDPosition.x * cos * 0.9f, HMDPosition.y + groundY, HMDPosition.z);
                Debug.Log("HMDP:" + newPosition.x + ", " + newPosition.y + ", " + newPosition.z);
                // cameraの位置を更新
                this.transform.position = newPosition;
                //this.transform.rotation = HMDRotationQ;
            }
            else
            {
                this.transform.position = HMDPosition;
                //this.transform.rotation = HMDRotationQ;
            }
            Debug.Log("###" + this.transform.position.y);
            Debug.Log(this.name);
        }
        else
        {
            Debug.Log("#");
        }
    }
}