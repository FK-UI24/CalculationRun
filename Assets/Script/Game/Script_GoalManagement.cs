using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_GoalManagement : MonoBehaviour
{
    [Header("ゴールしたら無効化するゲームオブジェクト")]
    [SerializeField] private List<GameObject> endObejct;

    [Header("Unityちゃんについてる操作スクリプト")]
    [SerializeField] private Script_PlayerCameraMove SPCM;

    [Header("ゴールテキスト")]
    [SerializeField] private TMP_Text goalText;

    [Header("フェード用のパネルがあるCanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("フェードアウトの時間")]
    [SerializeField] private float fadeOutTime = 0f;

    //ゴール時SEを管理する用変数
    private AudioSource SE;

    void Start()
    {
        //ゴールテキストは最初は非表示にする
        goalText.gameObject.SetActive(false);

        //ゴール時SEを格納する
        SE = GetComponent<AudioSource>();

    }

    //ゴールタイルに乗っているかを判定しゴール処理をする関数
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            if (collision.transform.position.y > transform.position.y)
            {
                //ゴールテキストを表示する
                goalText.gameObject.SetActive(true);

                //SEを鳴らす
                SE.Play();

                //ゴールしたら一部オブジェクトを無効化する
                foreach (GameObject go in endObejct)
                {
                    go.SetActive(false);
                }

                //unityちゃん操作用スクリプトを停止する
                //unityちゃんに直接ついているから別で直接止める
                SPCM.enabled = false;

                StartCoroutine(goalSystem());

            }
        }
    }

    //ゴール処理をする関数
    private IEnumerator goalSystem()
    {
        while (true)
        {
            if (!SE.isPlaying)
            {
                //ちょっとだけ余裕を持たせる
                yield return new WaitForSeconds(0.5f);

                ///フェードアウト中、プレイヤーが誤って裏のボタンを押さないようにパネルにクリック判定を入れる
                canvasGroup.blocksRaycasts = true;

                //時間を計測するためのタイマーの時間を格納する用変数
                float timer = 0f;

                //タイマーの数値がインスペクター側で設定したフェードアウト時間になるまで処理を繰り返す
                while (timer < fadeOutTime)
                {
                    //タイマーの時間を追加する
                    timer += Time.deltaTime;

                    //タイマーの進行度に合わせてalphaを１から０へ滑らかに変化させる
                    ///Mathf.Lerpは第一引数（始まりの位置）から第二引数（終わりの位置）へ滑らかに移動する
                    ///第三引数は0から1までが入り、0は初めの位置、1が終わりの位置である
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeOutTime);

                    //ゴールテキストも進行度に合わせて透明にする
                    goalText.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutTime);

                    //処理を次のフレームまで止める
                    ///いわゆるreturnのコルーチン版である
                    yield return null;
                }

                //ループが終わったからここで一旦完全に透明度を1にする
                canvasGroup.alpha = 1f;

                //フェードアウトが終了したらシーンを遷移する
                SceneManager.LoadScene("Result");
            }

            yield return null;

        }

    }

}
