using UnityEngine;

//これが同じオブジェクトにアタッチされていないとエラーになる
[RequireComponent(typeof(Animator))]

public class Script_UnityChanManager : MonoBehaviour
{
    [Header("移動の速さ")]
    [SerializeField]private float moveSpeed = 5f;

    [Header("回転速度")]
    [SerializeField]private float rotateSpeed = 5f;

}
