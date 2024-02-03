using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI log;
    [SerializeField] bool isLogging = false;
    public static Logger Instance { get; private set; }
    protected void Awake()
    {
        Instance = this;
        if (!isLogging && transform.childCount > 0)
            transform.GetChild(0).gameObject.SetActive(isLogging);

    }

    public void Log(string message)
    {
        if (isLogging)
            log.text += "\n" + message;
    }

    public void Log(int message)
    {
        if (isLogging)
            log.text += "\n" + message.ToString();
    }


    protected void OnDestroy()
    {
        Instance = null;
    }
}
