using NUnit.Framework.Constraints;
using UnityEngine;

public class Script_PlayerCameraMove : MonoBehaviour
{
    [Header("プレイヤーの移動速度")]
    [SerializeField]private float moveSpeed;

    [Header("プレイヤーの回転速度")]
    [SerializeField] private float rotateSpeed;

    [Header("プレイヤー")]
    [SerializeField] private GameObject player;

    [Header("ジャンプ力")]
    [SerializeField] private float jumpForce;


    //プレイヤーのRigidBodyを扱う用変数
    private Rigidbody playerRb;

    //キーボードの入力を格納する用変数
    //横方向
    private float inputHorizontal;
    //縦方向
    private float inputVertical;

    //プレイヤーのAnimatorを格納する用変数
    private Animator playerAnimator;


    private void Start()
    {
        //プレイヤーのRigidBodyを取得する
        playerRb=player.GetComponent<Rigidbody>();

        //プレイヤーのanimatorを取得する
        playerAnimator = player.GetComponent<Animator>();

        //プレイヤーのanimatorを初期化する
        playerAnimator.SetBool("Next", false);
        playerAnimator.SetBool("Back", false);
        playerAnimator.SetBool("Jump", false);


    }

    private void Update()
    {
        //WASDの入力を取得している
        ///GetAxisは滑らかに変化させる（徐々に変化させる）
        ///GetAxisRawは即座に-1/0/1を返す（カクっと動く）
        //A:-1～1:Dの範囲で徐々に変化させる
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        //W:1～-1:Sの範囲で徐々に変化させる
        inputVertical = Input.GetAxisRaw("Vertical");

        MoveAnimationController();

    }

    ///Updateは毎フレームの実行（例：60FPSなら1秒間に約60回実行する）
    ///→入力処理やUI更新、軽い処理など人の操作に関わる処理を行う
    ///FixedUpdateは一定時間ごとに実行される（0.02秒ごとなど、1秒間に約50回がデフォルト）
    ///→物理演算(RigidBody)や力を加える処理、移動(物理ベース)などの安定した計算が必要な処理を行う
    private void FixedUpdate()
    {
        //カメラの方向から、XZ平面の単位ベクトルを取得する
        ///Vector3.Scaleはベクトルの各成分を掛け算する。今回はXとZはそのままでYだけ0にする→水平面だけの前方向にする
        ///Camera.maini.transform.forwardはカメラが向いている前方向(3次元ベクトル、Y成分も含む)→例：上を向いている(0,0.7,0.7)
        ///normalizedはベクトルの長さを１にする(正規化)→例：方向はそのままで、大きさを一定にする(2,0,2)→(0.7,0,0.7)
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        //方向キーの入力値とカメラの向きから、移動方向を決定
        //カメラの向きに応じたベクトルを足している→Camera.Forward(0.7,0,0.7)+Camera.Right(0.7,0,-0.7)=(1.4,0,0)となったのを式化している
        Vector3 moveForward = (cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal).normalized;

        //移動方向にスピードをかける。ジャンプや落下がる場合は、別途Y軸方向のベクトルを足す
        //Y軸は固定するとジャンプ、落下ができなくなるので上下の動きはそのまま維持している
        playerRb.linearVelocity = moveForward * moveSpeed + new Vector3(0, playerRb.linearVelocity.y, 0);

        //キャラクターの向きを進行方向にする
        //もし移動方向がゼロじゃなかったら
        if (moveForward != Vector3.zero)
        {
            ///Quaternion.LookRotationは指定した方向を向く回転を作る→その方向を見るようにする
            Quaternion targetRotation = Quaternion.LookRotation(moveForward);

            //今の向き→目標の向きに滑らかに回す
            ///Quaternion.Slerp(現在の向き、目標の向き、どれくらい進むか(0～1))は現在の向きから目標の向きに雨らかに移動する関数
            ///どれくらい進むか(0～1)→0はそのまま動いていない、0.5は目標まであと半分くらい、1は目標に到達
            ///→例：rotateSpeed=10,deltaTime=0.16の場合、t=0.16となり毎フレーム「0.16%」ずつ近づく
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        }
    }

    private void Jump()
    {
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, jumpForce, playerRb.linearVelocity.z);
    }

    private void MoveAnimationController()
    {
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        playerAnimator.SetBool("Next", false);
        playerAnimator.SetBool("Back", false);
        playerAnimator.SetBool("Jump", false);


        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            playerAnimator.SetBool("Next", true);
        }
        else playerAnimator.SetBool("Back", true);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnimator.SetBool("Jump", true);

            Jump();

        }

    }

}