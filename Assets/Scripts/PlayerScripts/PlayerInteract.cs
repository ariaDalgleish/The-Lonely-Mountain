using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private float pickupTime = 2f;
    [SerializeField]
    private RectTransform pickupImageRoot;
    [SerializeField]
    private Image pickupProgressImage;
    [SerializeField]
    private TextMeshProUGUI interactText;

    private Interact interactableObject;
    private float currentPickupTimerElapsed;
    private bool hasInteractedThisHold = false;

    private SurvivalManager _survivalManager;
    private CharacterController controller;
    private InputManager _inputManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputManager = InputManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteractionUI();
    }

    private void HandleInteractionUI()
    {
        Interact();

        if (HasItemTargetted())
        {
            pickupImageRoot.gameObject.SetActive(true);

            if (_inputManager.IsInteractKeyPressed())
            {
                if (!hasInteractedThisHold)
                {
                    IncrementPickupProgressAndTryComplete();
                }
                //interactableObject.CallInteract(this)
            }
            else
            {
                currentPickupTimerElapsed = 0f;
                hasInteractedThisHold = false; // Reset when key is released

            }

            UpdatePickupProgressImage();
        }
        else
        {
            pickupImageRoot.gameObject.SetActive(false);
            currentPickupTimerElapsed = 0f;
            hasInteractedThisHold = false;
        }
    }

    private bool HasItemTargetted()
    {
        return interactableObject != null;
    }
    private void IncrementPickupProgressAndTryComplete()
    {
        currentPickupTimerElapsed += Time.deltaTime;
        if (currentPickupTimerElapsed >= pickupTime)
        {
            Debug.Log("Pick up item");
            interactableObject.CallInteract(this);
            hasInteractedThisHold = true;
            // Prevent further interaction until key is released
            // Only set interactableObject = null if you want to clear the reference (e.g., for items)
        }
    }

    private void UpdatePickupProgressImage()
    {
        float pct = currentPickupTimerElapsed / pickupTime;
        pickupProgressImage.fillAmount = pct;
    }

    public void Interact()
    {
        var layermask0 = 1 << 0; // Default
        var layermask3 = 1 << 3; // Interactable
        var finalmask = layermask0 | layermask3;

        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);


        if (Physics.Raycast(ray, out hit, 4f, finalmask)) // Range
        {

            var hitItem = hit.transform.GetComponent<Interact>();
            if (hitItem == null)
            {
                interactableObject = null;
                return;
            }
            if (hitItem != interactableObject)
            {
                interactableObject = hitItem;
                interactText.text = interactableObject?.GetPromptText(this) ?? "";
            }
        }
        else
        {
            interactableObject = null;
        }

    }
}
