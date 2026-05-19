using System;
using System.IO; // ファイル操作に必要
using System.Text; // エンコーディングに必要
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class speed6 : MonoBehaviour
{
    // インスペクターで確認できるように表示はするが、自動取得も行う
    [Header("Scripts (Auto-assigned if empty)")]
    public pitch0 scriptPitch0;
    public pitch3 scriptPitch3;
    public pitch5 scriptPitch5;
    // HMDの位置座標格納用
    public Vector3 HMDPosition;
    // HMDの回転座標格納用（クォータニオン）
    public Quaternion HMDRotationQ;
    // HMDの回転座標格納用（オイラー角）
    public Vector3 HMDRotation;

    // Inspectorから設定できるようにTransformを宣言
    public Transform targetTransform;

    // Z軸の回転角度を代入する変数
    private float gradient;

    // --- CSV出力用変数 ---
    private StreamWriter sw;
    private bool isRecording = false;
    private float startTime;
    // ---------------------

    void Start()
    {
        // 実行時に参照が空なら、シーン内から自動で探して設定する
        if (scriptPitch0 == null) scriptPitch0 = FindObjectOfType<pitch0>();
        if (scriptPitch3 == null) scriptPitch3 = FindObjectOfType<pitch3>();
        if (scriptPitch5 == null) scriptPitch5 = FindObjectOfType<pitch5>();

        // それでも見つからない場合の警告
        if (scriptPitch0 == null) Debug.LogWarning("pitch0 スクリプトがシーン内に見つかりません！");
        if (scriptPitch3 == null) Debug.LogWarning("pitch3 スクリプトがシーン内に見つかりません！");
        if (scriptPitch5 == null) Debug.LogWarning("pitch5 スクリプトがシーン内に見つかりません！");

        if (targetTransform == null)
        {
            GameObject planeObject = GameObject.Find("load");
            if (planeObject != null)
            {
                targetTransform = planeObject.transform;
            }
            else
            {
                Debug.LogError("'Plane (3)'が見つかりません。");
                enabled = false;
            }
        }
    }

    void Update()
    {
        // === HMDデータの取得 ===
        HMDPosition = InputTracking.GetLocalPosition(XRNode.Head);
        HMDRotationQ = InputTracking.GetLocalRotation(XRNode.Head);
        HMDRotation = HMDRotationQ.eulerAngles;

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Sキーが押されました！"); // ← これが出るか確認
        }

            // === Sキーで記録開始/停止 ===
            if (Input.GetKeyDown(KeyCode.S))
        {
            if (!isRecording)
            {
                StartRecording(); // 記録開始
            }
            else
            {
                StopRecording(); // 記録停止
            }
        }

        // === 記録中の処理（毎フレーム書き込み）===
        if (isRecording && sw != null)
        {
            float time = Time.time - startTime; // 経過時間
            float posX = HMDPosition.x;         // HMD X座標
            float rotX = HMDRotation.x;         // HMD 回転X

            // CSVデータを作成 (カンマ区切り)
            string[] s1 = { time.ToString(), posX.ToString(), rotX.ToString() };
            string s2 = string.Join(",", s1);

            // ファイルに書き込み
            sw.WriteLine(s2);
        }

        // === 以下、元の移動ロジック ===
        if (targetTransform != null)
        {
            Vector3 worldEuler = targetTransform.eulerAngles;
            gradient = (int)worldEuler.z;
        }
        float cos = (float)Math.Cos(gradient * (Math.PI / 180));

        // Raycast用
        RaycastHit hit;
        if (Physics.Raycast(HMDPosition, Vector3.down, out hit))
        {
            float groundY = hit.point.y;

            if (HMDPosition.x > 0)
            {
                Vector3 newPosition = new Vector3(HMDPosition.x * cos * 0.6f, HMDPosition.y + (groundY * cos * 0.6f), HMDPosition.z);
                this.transform.position = newPosition;
            }
            else
            {
                this.transform.position = HMDPosition;
            }
        }
    }

    // 記録開始処理
    void StartRecording()
    {
        int result = CheckActivePitch();
        // 1. スクリプト名を取得 ("speed6")
        //string scriptName = this.GetType().Name;

        // 2. ファイル名を生成: "gradient,スクリプト名.csv"
        // 例: gradientが30なら "30,speed6.csv" となります
        string fileName = $"{gradient},{result},0.6.csv";

        // 3. 保存パスを作成 (Assetsフォルダ直下)
        string filePath = Application.dataPath + "/" + fileName;

        filePath = GetUniqueFilePath(filePath);

        // false = 上書きモード (trueにすると追記モード)
        // Encoding.GetEncoding("UTF-8") でエンコード指定
        sw = new StreamWriter(filePath, false, Encoding.UTF8);

        // ヘッダー書き込み
        string[] s1 = { "Time", "Distance", "Rotation" };
        string s2 = string.Join(",", s1);
        sw.WriteLine(s2);

        startTime = Time.time;
        isRecording = true;
        Debug.Log("CSV記録開始: " + filePath);
    }

    // 記録停止処理
    void StopRecording()
    {
        if (sw != null)
        {
            sw.Flush(); // バッファを書き出し
            sw.Close(); // ファイルを閉じる
            sw = null;
        }
        isRecording = false;
        Debug.Log("CSV記録終了");
    }

    // ゲーム停止時などにファイルを確実に閉じる
    void OnApplicationQuit()
    {
        if (isRecording)
        {
            StopRecording();
        }
    }

    int CheckActivePitch()
    {
        // isActiveAndEnabled は「スクリプトのチェックON」かつ「GameObjectもON」の場合のみ true になります

        // 優先度 1: pitch0
        if (scriptPitch0 != null && scriptPitch0.isActiveAndEnabled)
        {
            return 0;
        }

        // 優先度 2: pitch3
        if (scriptPitch3 != null && scriptPitch3.isActiveAndEnabled)
        {
            return 3;
        }

        // 優先度 3: pitch5
        if (scriptPitch5 != null && scriptPitch5.isActiveAndEnabled)
        {
            return 5;
        }

        // どのスクリプトもアクティブでない、または見つからない場合
        return -1;
    }
    public string GetUniqueFilePath(string originalPath)
    {
        // ファイルが存在しなければ、そのままのパスを返す
        if (!File.Exists(originalPath))
        {
            return originalPath;
        }

        // パスから「ディレクトリ」「ファイル名（拡張子なし）」「拡張子」を分離
        string dir = Path.GetDirectoryName(originalPath);
        string fileName = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);

        int count = 1;
        string newPath = originalPath;

        // 重複しなくなるまでループして (1), (2)... を試す
        while (File.Exists(newPath))
        {
            // 新しいファイル名を生成: "Assets/Logs/data" + "(" + 1 + ")" + ".csv"
            string tempFileName = $"{fileName}({count}){extension}";
            newPath = Path.Combine(dir, tempFileName);
            count++;
        }

        return newPath;
    }
}