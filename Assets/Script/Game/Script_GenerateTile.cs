using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Script_GenerateTile : MonoBehaviour
{
    [Header("9枚のタイルを子オブジェクトに持った空オブジェクトのグループ")]
    [SerializeField] private List<GameObject> TilesGroup;

    [Header("1～9のマテリアルを順番に入れるリスト")]
    [SerializeField] private List<Material> TileMaterialGroup;

    //タイルに設置する用の数字のリスト
    private List<int> tileNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    private void Start()
    {
        //設置したタイルグループの数だけ数字と色とタグをグループごとに設置する作業を行う
        for(int i = 0; i < TilesGroup.Count; i++)
        {
            //タイルに設置する数字のリストの中身をランダムにする
            for(int j = 0; j < tileNumbers.Count; j++)
            {
                //「Fisher-Yatesシャッフル」というらしい
                //今の位置から後ろの範囲でランダムに一つ選ぶ
                int rand = Random.Range(j, tileNumbers.Count);

                //今の要素と選んだ要素を交換する
                int temp = tileNumbers[j];
                tileNumbers[j]=tileNumbers[rand];
                tileNumbers[rand] = temp;
            }

            //9枚のタイルを子オブジェクトに持った空オブジェクトのグループの子オブジェクトの数だけ繰り返す
            for (int j = 0; j < TilesGroup[i].transform.childCount; j++)
            {
                //タイルグループの空オブジェクトグループの子オブジェクトをインデックス番号で指定して
                //その子オブジェクトのTMP_Textのtextをシャッフルした数字のリストを順番に入れていく
                TilesGroup[i].transform.GetChild(j).gameObject.GetComponentInChildren<TMP_Text>().text = tileNumbers[j].ToString();

                //数字に対応したタグを入れる
                TilesGroup[i].transform.GetChild(j).gameObject.tag = "Field_" + tileNumbers[j].ToString();

                //数字に対応した色をタイルにつけていく
                for (int k = 1; k <= 9; k++)
                {
                    //1～9で同じ数字になったら
                    if (k == tileNumbers[j])
                    {
                        //マテリアルを対応した色にする
                        TilesGroup[i].transform.GetChild(j).gameObject.GetComponent<Renderer>().material = TileMaterialGroup[k-1];

                        //ここは１回終わればいいので脱出する
                        break;
                    }
                }


            }


        }
    }
}
