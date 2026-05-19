using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; // XRNodeやInputTrackingを使うために必要

public class ShowEvaluationImage : MonoBehaviour
{
    // 表示・非表示を切り替えたい画像（またはCanvas）のオブジェクトをここで指定
    public GameObject evaluationObject;

    // 現在表示されているかどうかのフラグ
    private bool isVisible = false;

    void Start()
    {
        // ゲーム開始時は非表示にしておく（必要なければコメントアウトしてください）
        if (evaluationObject != null)
        {
            evaluationObject.SetActive(false);
        }
    }

    void Update()
    {
        // キーボードのAキーが押された瞬間を検知
        if (Input.GetKeyDown(KeyCode.A))
        {
            ToggleImage();
        }
    }

    void ToggleImage()
    {
        if (evaluationObject == null) return;

        // 表示状態を反転させる（trueならfalseへ、falseならtrueへ）
        isVisible = !isVisible;

        if (isVisible)
        {
            // 1. HMDの位置と回転（向き）を取得
            Vector3 hmdPos = InputTracking.GetLocalPosition(XRNode.Head);
            Quaternion hmdRot = InputTracking.GetLocalRotation(XRNode.Head);

            // 2. 目の前1メートルの位置を計算
            // hmdRot * Vector3.forward で「HMDが向いている前方ベクトル」が取れます
            Vector3 targetPos = hmdPos + (hmdRot * Vector3.forward * 1.5f);

            // 3. 画像オブジェクトの位置を更新
            evaluationObject.transform.position = targetPos;

            // 4. 画像をHMDの方に向ける（ユーザーに正対させる）
            // そのままHMDと同じ回転を与えれば、ユーザーから見て真正面になります
            evaluationObject.transform.rotation = hmdRot;

            // 5. 表示する
            evaluationObject.SetActive(true);
        }
        else
        {
            // 6. 非表示にする
            evaluationObject.SetActive(false);
        }
    }
}
