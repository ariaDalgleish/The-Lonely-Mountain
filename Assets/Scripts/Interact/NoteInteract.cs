using UnityEngine;

public class NoteInteract : MonoBehaviour
{
    public Interact openFromInteraction;
    private PlayerNote _playerNote;

    private void Awake()
    {
        // Try to cache if PlayerNote is already present in scene(s)
        _playerNote = Object.FindFirstObjectByType<PlayerNote>(FindObjectsInactive.Include);
        
    }

    private void OnEnable()
    {
        if (openFromInteraction != null)
            openFromInteraction.GetInteractEvent.HasInteracted += HandleInteraction;
    }

    private void OnDisable()
    {
        if (openFromInteraction != null)
            openFromInteraction.GetInteractEvent.HasInteracted -= HandleInteraction;
    }

    private void HandleInteraction()
    {
        // Lazy lookup: attempt to find when the interaction occurs (covers scenes loaded later)
        if (_playerNote == null)
            _playerNote = Object.FindFirstObjectByType<PlayerNote>(FindObjectsInactive.Include);

        if (_playerNote == null)
        {
            Debug.LogWarning("PlayerNote not found. Ensure the scene containing PlayerNote is loaded or expose a persistent reference.");
            return;
        }
        
        _playerNote.CallPlayerNoteToggle();
    }
}
