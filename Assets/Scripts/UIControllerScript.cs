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


        //hacky workaround for hololens problems:
        //instead of directly calling the method, the update method queries whether certain objects in the scene
        //are active. if they are, the repsective function is called and then the object's state is set to inactive again.
        //all the buttons do is enable the game object again.

        /*if (PlacementToggleObject.activeSelf)
        {
            SetBoardState();
            PlacementToggleObject.SetActive(false);
        }

        if (ResetToggleObject.activeSelf || PlacementToggleObject.activeSelf) //score resets when re-placing board
        {
            ResetGame();
            ResetToggleObject.SetActive(false);
        }*/

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
