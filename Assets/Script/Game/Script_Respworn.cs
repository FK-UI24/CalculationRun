using UnityEngine;

public class Script_Respworn : MonoBehaviour
{
    [Header("リスポーン対象のプレイヤー")]
    [SerializeField] private GameObject Player;

    [Header("リスポーンにする高さ")]
    [SerializeField] private float DiePosY;

    //リスポーンする座標を格納する変数
    private Vector3 RespwornPos;


    private void Start()
    {
        //最初は初期位置をリスポーンエリアにする
        RespwornPos = new Vector3(0, 1, 0);
    }

    private void FixedUpdate()
    {
        if (Player.transform.position.y <= DiePosY) Player.transform.position = RespwornPos;
    }

}
