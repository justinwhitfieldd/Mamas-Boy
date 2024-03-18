using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI gameTimeText;
    public float gameTime = 0f;
    public bool textFlag = false;
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (textFlag == false)
        {
            gameTimeText.text = "Final Time: " + Time.time;
            textFlag = true;
        }
    }
}
