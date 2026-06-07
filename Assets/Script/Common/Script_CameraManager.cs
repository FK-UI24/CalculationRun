using UnityEngine;

public class Script_CameraManager : MonoBehaviour
{
    //カメラが向く基準点の座標
    [Header("カメラピボット")]
    [SerializeField] private Transform cameraPivot;

    [Header("カメラ感度")]
    [SerializeField] private float cameraSensitivity;

    //上下の回転角度を保存（累積）する用の変数
    private float verticalRotation = 0f;

    private void Start()
    {
        //マウスカーソルを非表示にして画面中央で固定する
        Cursor.lockState = CursorLockMode.Locked;
    }

}
