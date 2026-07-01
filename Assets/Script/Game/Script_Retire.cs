using UnityEngine;

public class Script_Retire : MonoBehaviour
{
    [Header("シーンマネージャーオブジェクト")]
    [SerializeField] private Script_SceneChange SS;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SS.ChangeScene("Title");
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
