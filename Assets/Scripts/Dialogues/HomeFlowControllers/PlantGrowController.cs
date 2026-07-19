using UnityEngine;

public class PlantGrowController : MonoBehaviour
{
    public void HandlePlantGrow()
    {
        HomeItemController home = GameObject.FindFirstObjectByType<HomeItemController>();

        home.HandlePlantAnimComplete();
    }
}
