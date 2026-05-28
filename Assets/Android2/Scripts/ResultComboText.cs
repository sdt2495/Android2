using UnityEngine;
using UnityEngine.UI;

public class ResultComboText : MonoBehaviour
{
    public Text resultText;

    void Start()
    {
        int maxCombo = PlayerPrefs.GetInt("MaxCombo", 0);

        resultText.text = "MAX COMBO : " + maxCombo;
    }
}