using UnityEngine;

public class Script_Respworn : MonoBehaviour
{
    [Header("リスポーン対象のプレイヤー")]
    [SerializeField] private GameObject Player;

    [Header("リスポーンにする高さ")]
    [SerializeField] private float DiePosY;

    [Header("復活させる足場")]
    [SerializeField] private Transform tileParent;

    //リスポーンする座標を格納する変数
    private Vector3 RespwornPos;

    //リスポーン時の音声を格納する用変数
    private AudioSource respwornSE;


    private void Start()
    {
        //最初は初期位置をリスポーンエリアにする
        RespwornPos = new Vector3(0, 1, 0);
        
        //リスポーン音声を格納する
        respwornSE = GetComponent<AudioSource>();

    }

    private void FixedUpdate()
    {
        //もし指定の高さを下回ったら
        if (Player.transform.position.y <= DiePosY)
        {
            //消えたタイルを復活させる
            //タイルの子オブジェクトの個数分（タイル列の数）
            for(int i = 0; i < tileParent.childCount; i++)
            {
                //各列を代入する
                Transform row = tileParent.GetChild(i);

                //代入した１つの列の子オブジェクトの個数分
                for(int j = 0; j < row.childCount; j++)
                {
                    //代入した列の子オブジェクトを取得する
                    Transform tile = row.GetChild(j);

                    //取得した１つのタイルのオブジェクトを復活させる
                    tile.gameObject.SetActive(true);

                    //各タイルについている子オブジェクトも復活させる
                    for(int k = 0; k < tile.childCount; k++)
                    {
                        tile.GetChild(k).gameObject.SetActive(true);
                        
                        //さらにそれについている子オブジェクトも復活させる（やり方は同じ）
                        Transform tilechild=tile.GetChild(k);

                        for(int l = 0; l < tilechild.childCount; l++)
                        {
                            tilechild.GetChild(l).gameObject.SetActive(true);
                        }
                    }

                }
            }

            //プレイヤーの位置を指定の位置に戻す
            Player.transform.position = RespwornPos;

            //リスポーン音声を鳴らす
            respwornSE.Play();

        }
    }

    public void MidleRespworn()
    {
        RespwornPos = new Vector3(0, 1, 111.5f);
    }

}
