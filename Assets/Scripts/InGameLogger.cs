using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InGameLogger : MonoBehaviour
{
    // Assign this in the Inspector to your UI Text element
    public TextMeshProUGUI logText;

    // Maximum number of log messages to keep
    public int maxLogCount = 100;

    // Use a queue to store log messages
    private Queue<string> logQueue = new Queue<string>();

    void Awake()
    {
        // Subscribe to the log message callback
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        // Unsubscribe when this object is destroyed
        Application.logMessageReceived -= HandleLog;
    }

    // This method is called whenever a Debug.Log message is output
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedLog = string.Format("[{0}] {1}", type, logString);
        logQueue.Enqueue(formattedLog);

        // If we've reached the maximum number of logs, dequeue the oldest message
        while (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }
    }

    void Update()
    {
        // Update the UI Text element with the current log messages
        if (logText != null)
        {
            logText.text = string.Join("\n", logQueue.ToArray());
        }
    }
}
