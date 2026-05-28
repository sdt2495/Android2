using UnityEngine;
using UnityEngine.UI;

public class BounceText : MonoBehaviour
{
    public Text targetText;       
    public float scaleAmount = 1.1f; 
    public float speed = 2f;         

    private Vector3 originalScale;

    void Start()
    {
        originalScale = targetText.rectTransform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * (scaleAmount - 1);
        targetText.rectTransform.localScale = originalScale * scale;
    }
}
