using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Script_BGMManager : MonoBehaviour
{

    //BGMを管理するインスタンス
    private static Script_BGMManager BGMinstance;

    [Header("このシーンでのBGMを流すシーンリスト")]
    [SerializeField] private List<string> allowedScenes = new List<string>();

    private void Awake()
    {
        //もしBGMインスタンスが既に存在していて、それがこのオブジェクトでない場合は、このオブジェクトを破壊する
        //じゅーふくぼーし
        if (BGMinstance != null && BGMinstance != this)
        {
            Destroy(gameObject);
            return;
        }
        //もしまだインスタンスがなかったら、このオブジェクトをインスタンスとして設定する
        BGMinstance = this;

        //シーンを切り替えても消えないようにする
        DontDestroyOnLoad(gameObject);

        //シーンが切り替わったときに呼ばれるイベントを登録する
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //シーンが読み込まれたときに実行される
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        //今読み込まれたシーン名が、許可リストに含まれているかを確認する
        //含まれていなければオブジェクトを削除する
        if (!allowedScenes.Contains(scene.name)) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //オブジェクトが削除されるときにイベント解消（エラー防止）
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
