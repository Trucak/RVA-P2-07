using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    public void OnChangeSeasonPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.ChangeSeason();
        }
    }

    public void OnRestartGardenPressed()
    {
        if (GardenManager.Instance != null)
        {
            GardenManager.Instance.RestartGarden();
        }
    }
}
