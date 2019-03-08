using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour{
    public GameObject grid;
    public int gameType = 0;
    private TileController tileController;
    private GameObject[] nodes;
    private GameObject[] nodesConnected;

    private void Awake(){
        tileController = grid.GetComponent<TileController>();
        nodes = tileController.GetAllNodes();
    }

    public void DidWin(){
        nodesConnected = tileController.GetAllConnectedNodes();
        if (CheckWinCondition()){
            Debug.Log("You Won");
        }
        else {
            Debug.Log("You Lost");
        }
    }

    private bool CheckWinCondition(){
        Debug.Log("check win condition");
        switch (gameType){
            case 0:
                return true;
            case 1:
                return StopAndGo();
        }
        return false;
    }

    private bool StopAndGo(){
        int goTally = 0;
        int playerTally = 0;
        foreach(GameObject node in nodes){
            if (node.name.Contains("YesNode")){
                goTally++;
            }
        }
        foreach(GameObject node in nodesConnected){
            if (node.name.Contains("NoNode")){
                return false;
            }
            if (node.name.Contains("YesNode")){
                playerTally++;
            }
        }
        if (playerTally >= goTally)
            return true;
        return false;
    }
}
