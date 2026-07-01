using UnityEngine;

public class Script_MidleManager : MonoBehaviour
{

    [Header("「Script_Respworn」がついているオブジェクト")]
    [SerializeField] private Script_Respworn SR;

    private AudioSource SE;

    //SEを一回だけ鳴らすトリガー
    private bool isSEPlayed = false;

    private void Start()
    {
        //SEを格納する
        SE = GetComponent<AudioSource>();

        isSEPlayed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") SR.MidleRespworn();

        if (!isSEPlayed)
        {
            SE.Play();
            isSEPlayed = true;
        }
    }
}
