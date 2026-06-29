using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Script_StartManager : MonoBehaviour
{
    [Header("スタートパネル")]
    [SerializeField] private GameObject startPanel;

    [Header("カウントダウン用テキスト")]
    [SerializeField] private TMP_Text countdownText;

    [Header("カウントダウンの秒数")]
    [SerializeField] private int countdown = 3;

    [Header("スタート時には無効にするオブジェクト")]
    [SerializeField] private List<GameObject> gameObjects;

    //unityちゃんの動きを制御しているスクリプトから参照する（true=動ける,false=動けない）
    public static bool startUnitytyan = false;

    //カウントダウン用のSEを格納する用変数
    private AudioSource[] SEs;

    private void Start()
    {
        //スタートパネルがあったら起動しておく
        startPanel.SetActive(true);

        //最初はゲーム中で使用するオブジェクトを無効化する
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(false);
        }

        //カウントダウンテキストに文章とフォントサイズを設定する
        countdownText.text = "エンターをおしてスタート";
        countdownText.fontSize = 100;

        //カウントダウン音を格納する
        SEs = GetComponents<AudioSource>();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) StartCoroutine(StartCountdown());
    }

    //タイマーを稼働させカウントダウンをするコルーチン
    private IEnumerator StartCountdown()
    {
        //タイマーに設定した時間を入れる
        int timer = countdown;

        //カウントダウンのフォントサイズを変える
        countdownText.fontSize = 450;

        //タイマーがー0より大きい間
        while (timer > 0)
        {
            //タイマーの現在の時間をカウントダウンテキストに入れる
            countdownText.text = timer.ToString();

            //カウントダウン音を鳴らす
            SEs[0].Play();

            //1秒待つ
            yield return new WaitForSeconds(1f);

            //１秒待ったのでタイマーから１引く
            timer--;
        }

        //カウントダウンのフォントサイズを変える
        countdownText.fontSize = 200;

        //カウントダウン音を鳴らす
        SEs[1].Play();

        //0になったらテキストを「スタート！」にする
        countdownText.text = "スタート！";

        //「START」がすぐに消えないように0.5秒まつ
        yield return new WaitForSeconds(1f);

        //カウントダウンパネルを消す
        startPanel.SetActive(false);

        //ゲーム中で使用するオブジェクトを有効化する
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(true);
        }

        startUnitytyan = true;

        //「クシコスポスト」をループで開始！！！
        SEs[2].Play();
        SEs[2].loop = true;

    }

}
