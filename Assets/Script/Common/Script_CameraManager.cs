using UnityEditor.Rendering;
using UnityEngine;

/*
 * カメラの角度をTPSのようなマウス操作で回転させる方法
 * あくまでこのスクリプトで制御しているのは基準点（Pivot）であり、カメラそのものは操作していない
 * 
 * このスクリプトはキャラクターの子オブジェクトに空オブジェクトで
 * 基準を作り（キャラの中心から少し上くらい）、その基準にアタッチする
 * キャラクター
 * 　↳基準点
 * 　　↳メインカメラ
 * 　　
 * このような構成にすることで基準点の回転→基準点の子オブジェクトのカメラも移動する
 * という感じで動いている
 * 
 */


public class Script_CameraManager : MonoBehaviour
{
    //カメラが向く基準点の座標
    [Header("カメラピボット")]
    [SerializeField] private Transform cameraPivot;

    [Header("カメラ感度")]
    [SerializeField] private float cameraSensitivity;

    [Header("回転の滑らかさ")]
    [SerializeField] private float smoothSpeed;



    //上下の回転角度を保存（累積）する用の変数
    private float verticalRotation = 0f;

    //左右の回転角度を保存（累積）する用の変数
    private float horizontalRotation = 0f;


    //基準点の初期位置・角度保存用
    private Vector3 initialPosition;
    private Quaternion initialRotation;



    private void Start()
    {
        //マウスカーソルを非表示にして画面中央で固定する
        Cursor.lockState = CursorLockMode.Locked;


        //初期位置・回転を保存
        initialPosition = cameraPivot.localPosition;
        initialRotation = cameraPivot.localRotation;

    }


    //リセットとトリガー管理で切り替えれば、最初は視線固定、始まったら自由ができるかも
    private void Update()
    {
        cameraControllMouse();

    }

    //マウスでカメラ操作をする関数
    public void cameraControllMouse()
    {
        //マウスの左右移動量
        ///GetAxisは左：-1～0～1：右の範囲で返り値を返す
        ///CameraSensitivityをかけるのは「GetAxis」「Time.deltaTime」では値が小さすぎるので調整するため
        ///Time.deltaTimeをかけることでPC性能でそう度が変わらないようにする
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;

        //マウスの上下移動量
        ///GetAxisは下：-1～0～1：上の範囲で返り値を返す
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

        //上下回転（カメラの上下向き）
        //マウスを上に動かしたら上を見るためにマイナスにしている
        //UnityではX回転（縦）が増えると下を向き、減ると上を向く。そのため直感的に操作できるようにマイナスにしている
        //気になるならシーンビューから回転モードを選んで回してみるとわかる
        verticalRotation -= mouseY;

        //上下の向きを制限する
        //今回の場合マイナス方向に大きくなればどんどん上を向き、プラス方向に大きくなるとどんどん下を向く
        //そのためここではマイナス方向の制限→上方向の制限、プラス方向への制限→下方向への制限となる
        verticalRotation = Mathf.Clamp(verticalRotation, -40f, 20f);

        //左右回転（カメラの左右向き）
        //上下と違い左右は特に変える必要なし
        horizontalRotation += mouseX;

        //目標回転を作る
        //ここではカメラが最終的に向く角度を生成している
        //X軸回転はverticalRotation、Y軸回転はhorizontalRotation、Z軸は回転しない←これを回転目標にしている
        ///Quaternionは回転の情報（向き）を表すデータ型→(x,y,z,w)のように４つの値で回転を表している
        ///EulerはX軸Y軸Z軸のオイラー角を指定して、その回転におけるQuaternionを作成する
        ///→Quaternionで定義されているメソッド
        Quaternion targetRotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);

        //スムーズに補完する
        ///localRotationは親オブジェクトから見た回転、rotationはワールド空間での回転が基準となる
        ///今回の場合の親オブジェクトはキャラクターとなる
        ///Slerpは回転aから回転bへ「滑らかに途中の回転」を返す
        ///第一引数は「現在の回転」、第二引数は「目標の回転（行きたい方向）」
        ///第三引数は「目標にどれくらいの割合近づくか」
        cameraPivot.localRotation =
            Quaternion.Slerp(cameraPivot.localRotation, targetRotation, smoothSpeed * Time.deltaTime);
    }


    //基準点の位置を初期状態に戻す関数（カメラの位置を初期位置に戻す関数）
    public void ResetPivot()
    {
        //初期位置に戻す
        cameraPivot.localPosition = initialPosition;
        cameraPivot.localRotation = initialRotation;

        //変数を直す
        //回転を角度に直す
        Vector3 angles = initialRotation.eulerAngles;

        //左右角度を反映し、横回転を戻す
        horizontalRotation = angles.y;

        //上下回転を反映し。縦回転を戻す
        verticalRotation = angles.x;

        //もし上下回転の角度が180を越えていたら
        if (verticalRotation > 180f)
        {
            //その状態から-360して-180～180の間に直す
            //こうしないとClampや回転処理で不具合が出るかも
            verticalRotation -= 360f;
        }
    }

}
