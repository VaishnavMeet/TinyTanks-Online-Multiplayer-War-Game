using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class TankSwitcher : MonoBehaviour
{
    public GameObject currentTank;
    
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public GameObject camera;

    GameObject GlualUi;
    GameObject AiUi;
    GameObject TreeUi;
    GameObject ObstclesUi;
    GameObject SpeedUi;

    public void SwitchTank(GameObject tankPrefab)
    {

        Vector3 currentPos = currentTank.transform.position;
        Quaternion currentRot = currentTank.transform.rotation;

        // Optional: Preserve data from current tank
        float currentHealth = currentTank.GetComponent<TankController2D>().health;
         int Glual = currentTank.GetComponent<TankController2D>().Glual; 
         int AiRobots = currentTank.GetComponent<TankController2D>().AiRobots; 
         int obstcles = currentTank.GetComponent<TankController2D>().obstcles; 
         int TreeHide = currentTank.GetComponent<TankController2D>().TreeHide; 
         int SpeedBoast = currentTank.GetComponent<TankController2D>().SpeedBoast; 
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

        GlualUi = GetComponent<GameManager>().GlualUi;
        AiUi = GetComponent<GameManager>().AiUi;
        TreeUi = GetComponent<GameManager>().TreeUi;
        ObstclesUi = GetComponent<GameManager>().ObstclesUi;
        SpeedUi = GetComponent<GameManager>().SpeedUi;


        // Restore tank health (if needed)
        controller.health = currentHealth;
        controller.Glual = Glual;
        controller.AiRobots = AiRobots;
        controller.obstcles = obstcles;
        controller.TreeHide = TreeHide;
        controller.SpeedBoast = SpeedBoast;
        controller.moveSpeed = 3;

        controller.GlualTxt = GlualUi.GetComponentInChildren<Text>();
        controller.AiRobotsTxt = AiUi.GetComponentInChildren<Text>();
        controller.TreeHideTxt = TreeUi.GetComponentInChildren<Text>();
        controller.obstclesTxt = ObstclesUi.GetComponentInChildren<Text>();
        controller.SpeedBoastTxt = SpeedUi.GetComponentInChildren<Text>();
        Button button1 = GlualUi.GetComponent<Button>();
        Button button2 = AiUi.GetComponent<Button>();
        Button button3 = TreeUi.GetComponent<Button>();
        Button button4 = ObstclesUi.GetComponent<Button>();
        Button button5 = SpeedUi.GetComponent<Button>();
        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(controller.UseGlual);
        button2.onClick.RemoveAllListeners();
        button2.onClick.AddListener(controller.UseAiRobots);
        button3.onClick.RemoveAllListeners();
        button3.onClick.AddListener(controller.UseTreeHide);
        button4.onClick.RemoveAllListeners();
        button4.onClick.AddListener(controller.UseObstacles);
        button5.onClick.RemoveAllListeners();
        button5.onClick.AddListener(controller.UseSpeedBooster);


    }
}
