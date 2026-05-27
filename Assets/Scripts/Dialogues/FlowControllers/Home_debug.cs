using UnityEngine;

public class Home_debug : MonoBehaviour
{
    public Item shovel;
    public Item origamiFlower;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            DebugProcess();
        }
    }

    private void DebugProcess()
    {
        DialogueManager.Instance.SetBoolVariable("shovel_get", true);
        DialogueManager.Instance.SetBoolVariable("origami_get", true);


        // Inventory
        InventoryManager.Instance.QueueItem(shovel);
        InventoryManager.Instance.QueueItem(origamiFlower);


        Debug.Log("Loaded debug progression");
    }
}
