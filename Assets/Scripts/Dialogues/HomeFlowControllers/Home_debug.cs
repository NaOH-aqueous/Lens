using UnityEngine;

public class Home_debug : MonoBehaviour
{
    public HomeItemController home;
    public Item shovel;
    public Item origamiFlower;
    public Item magnifier;
    public Item key;

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
        DialogueManager.Instance.SetBoolVariable("magnifier_get", true);
        DialogueManager.Instance.SetBoolVariable("key_get", true);
        DialogueManager.Instance.SetBoolVariable("time_get", true);


        // Inventory
        InventoryManager.Instance.QueueItem(shovel);
        InventoryManager.Instance.QueueItem(magnifier);
        InventoryManager.Instance.QueueItem(key);

        home.isChapterCleared = true;

        Debug.Log("Loaded debug progression");
    }
}
