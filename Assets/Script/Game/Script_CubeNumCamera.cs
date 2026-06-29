using System.Collections;
using TMPro;
using UnityEngine;

public class Script_CubeNumCamera : MonoBehaviour
{
    //タイルの数字テキストを扱う用変数
    private TMP_Text numText;

    //離れた時の時間を計測する用変数
    private float timer = 0f;

    //コルーチン管理用
    private Coroutine showCoroutine;

    private void Start()
    {
        //タイルの子オブジェクトの数字テキストを格納する
        numText = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        //カメラの方を向かせる
        numText.transform.LookAt(Camera.main.transform);

        //逆向きになっているから反転して直す
        numText.transform.Rotate(0, 180, 0);
    }

    //タグが「Player」に衝突したら子オブジェクトの数字テキストを消す
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //もし再表示待機中ならキャンセルする
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
            }

            //数字テキストを非表示にする
            numText.gameObject.SetActive(false);
        }
    }

    //タグが「Player」から離れたらタイルを消す（下のコルーチンを使えば、文字だけ再表示可能）
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            //離れたらタイルを消す
            gameObject.SetActive(false);
        }
    }

    //呼ばれてから１秒後に再表示する
    private IEnumerator ExitTimer()
    {
        //１秒待機
        yield return new WaitForSeconds(1f);

        //数字テキストを再表示する
        numText.gameObject.SetActive(true);
    }

}
