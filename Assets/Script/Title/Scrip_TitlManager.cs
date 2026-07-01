using System;
using TMPro;
using UnityEngine;

public class Scrip_TitlManager : MonoBehaviour
{
    //他スクリプトから参照する用難易度変数
    public static bool isHard;

    [Header("難易度ボタンテキスト")]
    [SerializeField] private TMP_Text hardButtonText;

    [Header("難易度表示テキスト")]
    [SerializeField]private GameObject hardText;

    [Header("月表示テキスト")]
    [SerializeField] private TMP_Text moonText;

    [Header("日表示テキスト")]
    [SerializeField] private TMP_Text dataText;

    [Header("曜日表示テキスト")]
    [SerializeField] private TMP_Text weekText;

    private AudioSource[] SE;

    private void Start()
    {
        isHard = false;

        hardText.SetActive(false);

        hardButtonText.text = "む\nず\nか\nし\nい";

        SE = GetComponents<AudioSource>();

        //日付の表示
        moonText.text = DateTime.Now.Month.ToString();
        dataText.text = DateTime.Now.Day.ToString();

        //曜日の表示
        string[] week = { "日", "月", "火", "水", "木", "金", "土" };
        weekText.text = "(" + week[(int)DateTime.Now.DayOfWeek] + ")";

    }


    //難易度調整関数
    public void OnChangeHard()
    {
        if (!isHard)
        {
            isHard = true;

            hardText.SetActive(true);

            hardButtonText.text = "か\nん\nた\nん";

            SE[0].Play();

        }
        else
        {
            isHard = false;

            hardText.SetActive(false);

            hardButtonText.text = "む\nず\nか\nし\nい";

            SE[1].Play();

        }
    }
}
