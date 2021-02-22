using UnityEngine;
using UnityEngine.UI;

public class ChangeMines : MonoBehaviour
{
    Text sliderText;
    // Start is called before the first frame update
    void Start()
    {
        sliderText = GetComponent<Text>();
    }

    public void UpdateText(float value)
    {
        sliderText.text = value + "";
    }
}
