using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Script_ResultManager : MonoBehaviour
{
    [Header("スコアテキスト")]
    [SerializeField] private TMP_Text scoreText;

    [Header("ハイスコアテキスト")]
    [SerializeField]private TMP_Text highScoreText;

    [Header("メッセージテキスト")]
    [SerializeField] private GameObject messageText;

    [Header("メッセージリスト")]
    [SerializeField] private List<string> messageList;

    [Header("難易度テキスト")]
    [SerializeField] private GameObject hardText;

    private void Start()
    {
        //カーソルの固定を解放する
        Cursor.lockState = CursorLockMode.None;

        //スコアを格納する
        scoreText.text = "SCORE：" + Script_ScoreTimer.scoreTimer.ToString();

        //最初はハイスコアテキストとメッセージを無効化する
        highScoreText.gameObject.SetActive(false);

        //メッセージにランダムに１つ入れる
        messageText.GetComponent<TMP_Text>().text = messageList[Random.Range(0, messageList.Count)];

        if (Scrip_TitlManager.isHard) hardText.SetActive(true);
        else hardText.SetActive(false);

        //もしベストスコアだったらデータ更新と報告テキストの表示
        UpdateScore();

    }

    private void UpdateScore()
    {
        int bestscore;

        //ハードモードならこっちの処理
        if (Scrip_TitlManager.isHard)
        {
            //現在のベストスコアを表示する
            bestscore = PlayerPrefs.GetInt("bestscore_hard", 1000000);

            //比較する
            if (Script_ScoreTimer.scoreTimer < bestscore)
            {
                //ハイスコアテキストを表示
                highScoreText.gameObject.SetActive(true);

                //更新
                PlayerPrefs.SetInt("bestscore_hard", Script_ScoreTimer.scoreTimer);

            }

        }
        else
        {
            //現在のベストスコアを表示する
            bestscore = PlayerPrefs.GetInt("bestscore", 1000000);

            //比較する
            if (Script_ScoreTimer.scoreTimer < bestscore)
            {
                //ハイスコアテキストを表示
                highScoreText.gameObject.SetActive(true);

                //更新
                PlayerPrefs.SetInt("bestscore", Script_ScoreTimer.scoreTimer);

            }
        }
        //セーブ
        PlayerPrefs.Save();


    }

}
