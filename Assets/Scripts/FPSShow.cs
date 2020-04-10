using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSShow : MonoBehaviour
{
    public Text fpsText;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void Update()
    {
        fpsText.text = 1 / Time.deltaTime + "";
    }
}
