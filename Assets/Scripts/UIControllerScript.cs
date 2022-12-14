using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;
using TMPro;
using static Microsoft.MixedReality.Toolkit.Experimental.UI.KeyboardKeyFunc;
using UnityEngine.PlayerLoop;


using UnityEngine.Serialization;
using Microsoft;
//using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;

public class UIControllerScript : MonoBehaviour
{

    public GameObject InputController;
    public GameObject Board; //used to obtain current points
    public GameObject Scoreboard; //display score text

    //these two objects serve as toggle for the placement mode and the reset button
    //public GameObject PlacementToggleObject;
    //public GameObject ResetToggleObject;

    public GameObject GravitySlider;
    public GameObject SpeedSlider;

    public GameObject GravityDisplay;
    public GameObject SpeedDisplay;

    public float GravityMultiplier;
    public float SpeedMultiplier;

    public GameObject Constants;

    public List<GameObject> Levels;

    public int currentLevel = 0;

    public GameObject levelButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //periodically update the UI scoreboard
        //ScoreBoard.GetComponent<ButtonConfigHelper>().MainLabelText = $"Score: {Board.GetComponent<BoardHandler>().GetPoints()}"; //old version, where scoreboard was on the menu
        //Debug.Log(Scoreboard.GetComponent<TextMeshProUGUI>().text);

        int score = Board.GetComponent<BoardHandler>().GetPoints();
        int num_darts = GetNumberOfHitDarts();
        Scoreboard.GetComponent<TextMeshProUGUI>().text = $"\r\nPOINTS: \r\n{score}\r\n\r\nDARTS HIT: \r\n{num_darts}";

    }

    public void SetGravity(SliderEventData eventData)
    {
        float newValue = GravityMultiplier * eventData.NewValue;

        GravityDisplay.GetComponent<TextMeshPro>().text = $"{newValue:F2}";
        Constants.GetComponent<ConstantsScript>().Gravity = newValue;
    }

    public void SetSpeed(SliderEventData eventData)
    {

        float newValue = SpeedMultiplier * eventData.NewValue;
        SpeedDisplay.GetComponent<TextMeshPro>().text = $"{newValue:F2}";
        Constants.GetComponent<ConstantsScript>().DartsSpeed = newValue;
    }

    public void ToggleSliderActive()
    {
        if (GravitySlider.activeSelf)
        {
            GravitySlider.SetActive(false);
            SpeedSlider.SetActive(false);
        }
        else
        {
            GravitySlider.SetActive(true);
            SpeedSlider.SetActive(true);
        }
    }

    //when pressing the "place board" button, enable board placement
    public void SetBoardState()
    {
        Debug.Log("Switched to Board state! (Reason: Board placement button pressed)");
        InputController.GetComponent<InputController>().SetGameState(Modes.Board);
    }


    //select which gamemode is currently being played (for the future)
    void SetGameMode()
    {
        //doesn't do anything yet
    }

    public void ChangeLevel()
    {
        currentLevel = (currentLevel + 1) % Levels.Count;

        for (int i = 0; i < Levels.Count; i++)
        {
            if (i == currentLevel)
            {
                Levels[i].SetActive(true);
            }
            else
            {
                Levels[i].SetActive(false);
            }
        }

        levelButton.GetComponent<ButtonConfigHelper>().MainLabelText = $"Change level\r\nCurrent level: {currentLevel}";

    }

    public int GetNumberOfHitDarts()
    {
        int darts_counter = 0;
        /*foreach (Transform child in Board.transform)
        {
            if (child.gameObject.tag == "Dart")
            {
                darts_counter++;
            }
        }*/

        foreach (int elem in Board.GetComponent<BoardHandler>().points)
        {
            if (elem != 0)
            { 
                darts_counter++;
            }
        }

        return darts_counter;
    }

    //set the counter on the board to zero and destory all Dart game objects
    public void ResetGame()
    {
        Board.GetComponent<BoardHandler>().ResetPoints();

        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        foreach (GameObject go in rootObjects)
        {
            if(go.tag == "Dart")
            {
                Destroy(go);
            }
        }

        //if a dart hits the board, it is appended to the board as a child, so we also need to go over all the children of the board and destroy them
        foreach(Transform child in Board.transform)
        {
            if (child.gameObject.tag == "Dart")
            {
                Destroy(child.gameObject);
            }
        }


    }



}
