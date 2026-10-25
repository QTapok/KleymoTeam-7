using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NPC : MonoBehaviour
{
    public DialogueData dialogue;

    private void OnMouseDown()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (DialogueUI.Instance != null && dialogue != null)
            DialogueUI.Instance.Open(dialogue);
    }
}
