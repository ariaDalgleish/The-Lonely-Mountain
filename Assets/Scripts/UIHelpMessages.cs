using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHelpMessages : MonoBehaviour
{
    public static UIHelpMessages Instance { get; private set; }

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    private float timer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
            timer = messageDuration;
        }
    }

    private void Update()
    {
        if (messagePanel != null && messagePanel.activeSelf)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
                messagePanel.SetActive(false);
        }
    }
}
