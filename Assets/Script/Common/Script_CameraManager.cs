using UnityEngine;

public class Script_CameraManager : MonoBehaviour
{
    [Header("カメラのターゲットなるオブジェクト")]
    [SerializeField] private GameObject targetObject;

    [Header("カメラの移動速度")]
    [SerializeField] private float cameraSpeed;

    [Header("カメラの上下方向の最小値")]
    [SerializeField] private float cameraMin;

    [Header("カメラの上下方向の最大値")]
    [SerializeField] private float cameraMax;

    //ターゲットのポジションを格納する用変数
    private Vector3 targetPos;

    //上下方向の制限をするために、上下回転の合計角度を記録する用変数
    private float verticalAngle = 0f;

    private void Start()
    {
        //ターゲットの座標を格納する
        targetPos = targetObject.transform.position;

        //マウスカーソルを非表示にして画面中央で固定する
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        //ターゲットの移動量分、カメラも追従する
        ///ターゲットの「前フレームからの移動量」を求め、その分だけカメラも移動させる。こうすることでカメラとターゲットの距離を一定に保つ
        ///引かないと毎フレーム元の座標が足され続け、爆速でどこかにフライアウェイする
        Camera.main.transform.position += targetObject.transform.position - targetPos;

        //ターゲットの座標を更新する
        targetPos = targetObject.transform.position;

        //マウスの移動量を格納する
        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");

        //ターゲットの位置のY軸を中心に回転（公転）する
        ///RotateAround(中心、軸、角度)で指定した点を中心に円を描くように回転する
        ///Vector3.upは(0,1,0)であるため、縦方向を軸に回る=横にぐるっとまわる
        Camera.main.transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * cameraSpeed);

        //今のフレームで回そうとしている角度（上下）
        float nowFrameY = mouseInputY * Time.deltaTime * cameraSpeed;

        // 前の角度保存
        float prevAngle = verticalAngle;

        // 角度更新
        verticalAngle += nowFrameY;

        // Clampで制限
        verticalAngle = Mathf.Clamp(verticalAngle, cameraMin, cameraMax);

        // 実際に回す量（差分）
        float delta = verticalAngle - prevAngle;

        //ターゲットの上下で回転させる
        ///Camera.main.transform.rightはカメラの右ベクトルであり、これが軸となる
        ///→その場で回転しないか？→Roateは自分中心で回るのでその場回転するが、RotateAroundは指定した場所を中心に回るのでその場回転しない
        Camera.main.transform.RotateAround(targetPos, Camera.main.transform.right, delta);

    }

}
