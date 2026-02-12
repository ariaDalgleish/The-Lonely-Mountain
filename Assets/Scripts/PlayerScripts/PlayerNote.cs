using UnityEngine;

public class PlayerNote : MonoBehaviour
{
    [SerializeField] public GameObject playerNote;
    [SerializeField] private RectTransform putAwayText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNote.activeSelf)
        {
            if (InputManager.Instance.PutAway())
            {
                playerNote.SetActive(false);
                putAwayText.gameObject.SetActive(false);
            }
        }
    }

    public void CallPlayerNoteToggle()
    {
        if (playerNote != null)
        {
            playerNote.SetActive(!playerNote.activeSelf);
            putAwayText.gameObject.SetActive(true);
        }

        
    }
}
