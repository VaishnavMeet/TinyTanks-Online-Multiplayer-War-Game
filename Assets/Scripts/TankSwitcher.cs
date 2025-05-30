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
    public GameObject camera;
    
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
        Button button = GameObject.FindWithTag("pick").GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(controller.OnSwapButtonClicked);

        // Restore tank health (if needed)
        controller.health = currentHealth;
        controller.moveSpeed = 3;
        
    }
}
