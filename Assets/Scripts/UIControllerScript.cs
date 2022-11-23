using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

public class UIControllerScript : MonoBehaviour
{

    public GameObject InputController;
    public GameObject Board; //used to obtain current points
    public GameObject ScoreBoard; //display score text

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //periodically update the UI scoreboard
        ScoreBoard.GetComponent<ButtonConfigHelper>().MainLabelText = $"Score: {Board.GetComponent<BoardHandler>().GetPoints()}";

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

    //set the counter on the board to zero and destory all Dart game objects
    public void ResetGame()
    {
        Board.GetComponent<BoardHandler>().ResetPoints();

        List<GameObject> rootObjects = new List<GameObject>();
        Scene scene = SceneManager.GetActiveScene();

        scene.GetRootGameObjects(rootObjects);

        foreach (GameObject go in rootObjects)
        {
            if(go.name == "Dart(Clone)")
            {
                Destroy(go);
            }
        }

        //if a dart hits the board, it is appended to the board as a child, so we also need to go over all the children of the board and destroy them
        foreach(Transform child in Board.transform)
        {
            if (child.gameObject.name == "Dart(Clone)")
            {
                Destroy(child.gameObject);
            }
        }


    }



}
