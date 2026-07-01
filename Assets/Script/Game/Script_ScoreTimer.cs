using System.Collections;
using TMPro;
using UnityEngine;

public class Script_ScoreTimer : MonoBehaviour
{
    [Header("タイマー用関数を表示する用テキスト")]
    [SerializeField] private TMP_Text scoreTimerText;

    //他のシーンからスコアタイマーを参照する用の変数
    public static int scoreTimer;

    private void Start()
    {
        //最初は何もないので０を入れる
        scoreTimerText.text = "0";

        //上記と同じく０を入れる
        scoreTimer = 0;

    }

    //ここは最初無効化されているので
    private void OnEnable()
    {
        StartCoroutine(plusScore());
    }

    //スコアを換算する用変数
    private IEnumerator plusScore()
    {
        while (true)
        {
            //タイマーの現在の時間をカウントダウンテキストに入れる
            scoreTimerText.text = scoreTimer.ToString();

            //１秒待つ
            yield return new WaitForSeconds(1f);

            //スコアタイマーが99999以上になったらそれ以上増えないようにする
            if (scoreTimer >= 99999) yield return null;

            //１秒待ったので追加する
            scoreTimer++;

        }
    }

}
