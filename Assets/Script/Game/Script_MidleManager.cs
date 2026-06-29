using UnityEngine;

public class Script_MidleManager : MonoBehaviour
{

    [Header("「Script_Respworn」がついているオブジェクト")]
    [SerializeField] private Script_Respworn SR;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") SR.MidleRespworn();
    }
}
