using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Cinemachine;

public class TankSwitcher : MonoBehaviour
{
    public GameObject currentTank;
    
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public GameObject pickUp; // UI pickup reference (Image + Button)
    public GameObject camera;
    public List<Transform> spawneGeneration; // Spawn points
    public GameObject BulletPickupPrefab;    // Prefab to instantiate at each point
    public List<GameObject> allBulletPrefab; // All bullet/item prefabs to assign randomly

    public List<Sprite> allTankBodySprites;
    public List<Sprite> allTankBarrelSprites;
    public void SwitchTank(GameObject tankPrefab)
    {

        Vector3 currentPos = currentTank.transform.position;
        Quaternion currentRot = currentTank.transform.rotation;

        // Optional: Preserve data from current tank
        float currentHealth = currentTank.GetComponent<TankController2D>().health;

        // Destroy old tank
        Destroy(currentTank);

        //  Instantiate a new tank instance
        GameObject newTank = Instantiate(tankPrefab, currentPos, currentRot);
        currentTank = newTank;

        // Reassign joystick references
        TankController2D controller = newTank.GetComponent<TankController2D>();
        controller.moveJoystick = moveJoystick;
        controller.aimJoystick = aimJoystick;
        camera.GetComponent<CemraSetup>().player= newTank.transform;
        // Hook up pickup button
        Button button = pickUp.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(controller.OnSwapButtonClicked);

        // Restore tank health (if needed)
        controller.health = currentHealth;
        controller.moveSpeed = 3;
        // Spawn bullet pickups
        GenerateBulletPickups();
    }


    private void GenerateBulletPickups()
    {
        foreach (Transform spawn in spawneGeneration)
        {
            GameObject pickup = Instantiate(BulletPickupPrefab, spawn.position, Quaternion.identity);

            // Assign a random bullet/item prefab
            PickUp pickupScript = pickup.GetComponent<PickUp>();
            if (pickupScript != null && allBulletPrefab.Count > 0)
            {
                int index = Random.Range(0, allBulletPrefab.Count);
                pickupScript.prefab = allBulletPrefab[index];
            }
        }
    }
}
