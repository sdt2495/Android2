using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TapToStart : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("画面タップ → ゲーム開始");
        SceneManager.LoadScene("Main"); 
    }
}
