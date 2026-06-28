using NUnit.Framework.Constraints;
using UnityEngine;

//これはSDUnityちゃんに直接アタッチするスクリプト

public class Script_PlayerCameraMove : MonoBehaviour
{
    [Header("プレイヤーの移動速度")]
    [SerializeField]private float moveSpeed;

    [Header("プレイヤーの回転速度")]
    [SerializeField] private float rotateSpeed;

    [Header("ジャンプ力")]
    [SerializeField] private float jumpForce;

    [Header("プレイヤーの着地アニメーション判定に使用するRayの長さ")]
    [SerializeField] private float rayLength;


    //プレイヤーのRigidBodyを扱う用変数
    private Rigidbody playerRb;

    //キーボードの入力を格納する用変数
    //横方向
    private float inputHorizontal;
    //縦方向
    private float inputVertical;

    //プレイヤーのAnimatorを格納する用変数
    private Animator playerAnimator;

    //ジャンプ用地面接触フラグ（ジャンプの可否を判断するのに使用、true=ジャンプできる、false=ジャンプできない）
    private bool isGrounded = false;

    private void Start()
    {
        //プレイヤーのRigidBodyを取得する
        playerRb = GetComponent<Rigidbody>();

        //プレイヤーのanimatorを取得する
        playerAnimator = GetComponent<Animator>();

        //プレイヤーのanimatorを初期化する
        playerAnimator.SetBool("Next", false);
        playerAnimator.SetBool("Back", false);

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

        //移動のアニメーションとジャンプの管理をする関数を呼び出す
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
        ///Camera.main.transform.forwardはカメラが向いている前方向(3次元ベクトル、Y成分も含む)→例：上を向いている(0,0.7,0.7)
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        }
    }


    //単純にジャンプのために速度を変える関数
    private void Jump()
    {
        //プレイヤーのY方向の速度だけを変更する。X,Z軸は今の状態のものをそのまま参照する
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, jumpForce, playerRb.linearVelocity.z);
    }

    //今のフレームで「Field」タグの地面に乗っているかの確認をする関数
    ///何かしらのオブジェクトと衝突している間、毎フレーム呼ばれる関数（Unityで定義されているイベント関数）
    ///引数は衝突しているオブジェクトの情報、戻り値は無し
    private void OnCollisionStay(Collision collision)
    {
        //衝突しているオブジェクトのタグに「Field」が含まれていたら
        if (collision.gameObject.tag.Contains("Field"))
        {
            //接触点を全て確認する
            ///contacts...接触している点のリスト。今回の場合は衝突しているオブジェクトの接触している点
            ///ContactPoint...１つの接触点の詳細情報
            foreach (ContactPoint contact in collision.contacts)
            {
                //もし接触している点がある程度上向きなら
                ///contact.normal...接触面の「向き」を表すベクトル
                ///今回の場合接触面の上方向の成分だけを取り出し、その向きを確認している
                ///真上...normal.y=1.0
                ///斜め上...normal.y=0.5～0.9
                ///横（壁）...normal.y=0
                ///下...normal.y=-1
                ///今回の条件を「contact.normal.y==1.0f」とかにすると
                ///完全に上向きのオブジェクト上じゃないとジャンプできなくなる
                if (contact.normal.y > 0.5f)
                {
                    //地面に乗っていることにする→ジャンプ可能状態であるということ
                    isGrounded = true;

                    //ここでreturnなのは、どこかしら衝突していれば関数を終了していいから
                    //仮にbreakだと動きはするが無駄な処理が入る
                    return;
                }
            }
        }
    }

    //衝突していたオブジェクトから離れた時に、そのオブジェクトが「Field」タグかを確認する関数
    ///衝突していた何かしらのオブジェクトから離れた瞬間に呼ばれる関数（Unityで定義されているイベント関数）
    ///これは毎フレームではなく、離れた一瞬の1回だけ呼ばれる
    ///こっちのcollisionには「今離れた相手の情報」が入る
    private void OnCollisionExit(Collision collision)
    {
        //もし離れたオブジェクトのタグに「Field」が含まれていたら
        if (collision.gameObject.tag.Contains("Field"))
        {
            //地面に乗ってないことにする→ジャンプ状態であるor落下状態である→ジャンプできない状態
            isGrounded = false;

        }
    }

    private void MoveAnimationController()
    {
        AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        playerAnimator.SetBool("Next", false);
        playerAnimator.SetBool("Back", false);


        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            playerAnimator.SetBool("Next", true);
        }
        else playerAnimator.SetBool("Back", true);

        //スペースが押されたかつ地面についているとき
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //ジャンプ開始のトリガーをいれる
            ///アニメーターのトリガーは一瞬だけONになるので、すぐ自動でOFFに戻る
            playerAnimator.SetTrigger("JumpStart");

            Jump();

        }

    }

}