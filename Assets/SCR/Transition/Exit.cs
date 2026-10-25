using UnityEngine;

public class Exit : MonoBehaviour
{
    public string targetLocation;
    public string requiredFlag;
    public string lockedMessage = "Заперто";

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(requiredFlag) && !GameState.Instance.HasFlag(requiredFlag))
        {
            Debug.Log(lockedMessage);
            return;
        }

        LocationManager.Instance.GoTo(targetLocation);
    }
}