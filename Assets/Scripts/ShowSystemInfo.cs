using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSystemInfo : MonoBehaviour
{
    void Start()
    {
        GetComponent<UnityEngine.UI.Text>().text = SystemInfo.graphicsDeviceType + "-" + SystemInfo.operatingSystem;
    }
}
