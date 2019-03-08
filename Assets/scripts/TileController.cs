using System;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour{
    Color32 activeColor;
    public GameObject startNode;
    private GameObject currentNode;
    public GameObject gameMaster;
    public GameObject[] pipes;
    public GameObject[] nodes;
    private List<GameObject> nodesConnected;
    private List<GameObject> pipesConnected;
   
    void Start() {
        ResetArrays();
        SetCurrentNode();
        FindPaths();
        activeColor = new Color32(241, 26, 54, 255);
    }

    public void FindPaths(){
        CircleCollider2D col2D = currentNode.GetComponent<CircleCollider2D>();
        foreach (GameObject pipe in pipes){
            if (pipe.gameObject.GetInstanceID() != currentNode.GetInstanceID() &&
            col2D.bounds.Intersects(pipe.gameObject.GetComponent<Collider2D>().bounds)){
                CheckDirection(pipe.gameObject);
            }
        }
    }

    public void UpdateNode(GameObject node){
        currentNode = node;
        FindPaths();
        nodesConnected.Add(node);
        SetNodeAsActive(node);
    }

    public GameObject GetDestinationNodePosition(GameObject pipe, GameObject startNode){
        BoxCollider2D box2D = pipe.GetComponent<BoxCollider2D>();
        foreach (GameObject node in nodes){
            if (box2D.bounds.Intersects(node.gameObject.GetComponent<CircleCollider2D>().bounds)
            && node.GetInstanceID() != startNode.GetInstanceID()){
                return node;
            }
        }
        return null;
    }

    private void CheckDirection(GameObject pipe){
        float xDifference = currentNode.transform.position.x - pipe.transform.position.x;
        float yDifference = currentNode.transform.position.y - pipe.transform.position.y;
        if (yDifference < 0 && Math.Abs(yDifference) > Math.Abs(xDifference)){
            pipe.tag = "North";
            return;
        }
        if (xDifference < 0 && Math.Abs(xDifference) > Math.Abs(yDifference)){
            pipe.tag = "East";
            return;
        }
        if (yDifference > 0 && Math.Abs(yDifference) > Math.Abs(xDifference)){
            pipe.tag = "South";
            return;
        }
        if (xDifference > 0 && Math.Abs(xDifference) > Math.Abs(yDifference)){
            pipe.tag = "West";
            return;
        }
    }

    public void SetCurrentNode(){
        currentNode = nodesConnected[nodesConnected.Count - 1];
    }

    public GameObject PreviousNode(){
        if (nodesConnected.Count < 2)
            return null;
        return nodesConnected[nodesConnected.Count - 2];
    }

    public void AddNode(GameObject node){
        nodesConnected.Add(node);
    }

    public void RemoveNode(){
        int position = nodesConnected.Count - 1;
        SetNodeAsInactive(nodesConnected[position]);
        nodesConnected.RemoveAt(position);
    }

    public void AddPipe(GameObject pipe){
        pipesConnected.Add(pipe);
    }

    public void RemovePipe(){
        int position = pipesConnected.Count - 1;
        pipesConnected.RemoveAt(position);
    }

    public GameObject GetCurrentStartNode() {
        int position = nodesConnected.Count - 1;
        return nodesConnected[position];
    }

    public GameObject GetOriginalStartNode(){
        return startNode;
    }

    public void ResetArrays(){
        nodesConnected = new List<GameObject> { startNode };
        pipesConnected = new List<GameObject>();
    }

    public void SetNodeAsActive(GameObject node){
        SpriteRenderer rend = node.GetComponent<SpriteRenderer>();
        rend.color = activeColor;
    }

    public void SetNodeAsInactive(GameObject node){
        SpriteRenderer rend = node.GetComponent<SpriteRenderer>();
        rend.color = Color.white;
    }

    public void SetAllNodesAsInactive(){
        foreach(GameObject node in nodesConnected){
            if(node.tag != "StartNode")
                SetNodeAsInactive(node); 
        }
    }

    public GameObject[] GetAllNodes(){
        return nodes;
    }

    public GameObject[] GetAllConnectedNodes(){
        return nodesConnected.ToArray();
    }
}

