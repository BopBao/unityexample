using UnityEngine;
using System.Collections.Generic;

// After end node go diagnoally to a final win node

public class PathDrawer : MonoBehaviour{
    private static readonly int NO_PATH = 0;
    private static readonly int NORTH = 1;
    private static readonly int EAST = 2;
    private static readonly int SOUTH = 3;
    private static readonly int WEST = 4;

    public Material lineMaterial;
    public GameObject grid;
    private DrawMeshLine drawMeshLine;
    LayerMask nodeLayer;
    LayerMask pipeLayer;
    private TileController tileController;
    private Vector3 startPosition;
    private GameObject currentStart;
    private GameObject currentDestination;
    private bool makeLine = false;
    int path = NO_PATH;

    private void Awake(){
        tileController = grid.GetComponent<TileController>();
        drawMeshLine = gameObject.GetComponent<DrawMeshLine>();
        nodeLayer = LayerMask.GetMask("Node");
        pipeLayer = LayerMask.GetMask("Pipe");
    }

    void Start(){
        currentStart = tileController.GetOriginalStartNode();
        SetStartPosition();
    }

    void Update(){
        if (Input.GetButtonDown("Fire1"))
            makeLine = DoStartLine();
        if (Input.GetButton("Fire1")){
            Vector3 pointerPosition = GetPointerPosition();
            if (makeLine && path == NO_PATH){
                // Tag Adjacent Pipes with Directions //
                tileController.FindPaths();
                // Find Direction Player is Taking //
                GameObject hitObject = DeterminePath(pointerPosition);
                if(hitObject != null){
                    // Check if Path is Backtracking //
                    GameObject previousNode = tileController.PreviousNode();
                    bool backtracking = false;
                    if (previousNode != null)
                        backtracking = Backtracking(pointerPosition, previousNode.transform.position);
                    if (backtracking){
                        // Remove a Node //
                        tileController.RemoveNode();
                        // Set Start New Node //
                        currentStart = tileController.GetCurrentStartNode();
                        SetStartPosition();
                        tileController.SetCurrentNode();
                        drawMeshLine.RemovePoint();
                        // Set Path to No Path //
                        path = NO_PATH;
                        return;
                    }
                    // Add to Pipe Array //
                    tileController.AddPipe(hitObject);
                    // Find the Next Node //
                    currentDestination = tileController.GetDestinationNodePosition(hitObject, currentStart);
                    if (currentDestination == null)
                        path = NO_PATH;
                }
                // Add Start Points to the Mesh Renderer //
                drawMeshLine.AddStartPoint(startPosition, path);
            }
            if (makeLine && path != NO_PATH)
                CreateLineToMouse(pointerPosition);
        }else if (Input.GetButtonUp("Fire1")){
            ResetLine();
        }
    }

    private bool DoStartLine(){
        RaycastHit2D hit = Get2DHitOnNode(GetPointerPosition());
        if (hit == false || hit.collider == null || hit.collider.gameObject == null)
            return false;
        if (hit.collider.gameObject.name == "StartNode")
            return true;
        return false;
    }

    private GameObject DeterminePath(Vector3 pointerPosition){
        RaycastHit2D hit = Get2DHitOnPipe(pointerPosition);
        if (hit == false || hit.collider == null || hit.collider.gameObject == null)
            return null;
        GameObject hitObject = hit.collider.gameObject;
            
        switch (hitObject.tag){
            case "North":
                path = NORTH;
                return hitObject;
            case "East":
                path = EAST;
                return hitObject;
            case "South":
                path = SOUTH;
                return hitObject;
            case "West":
                path = WEST;
                return hitObject;
            default:
                return null;
        }
    }

    public void CreateLineToMouse(Vector3 pointerPosition){
        DetectNextNode(pointerPosition);
        Vector3 updatedPosition = ConstrainLine(pointerPosition);
        drawMeshLine.CreateLine(updatedPosition, path);
    }

    private Vector3 GetPointerPosition(){
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 8;
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }

    private void DetectNextNode(Vector3 pointerPosition){
        RaycastHit2D hit = Get2DHitOnNode(pointerPosition);
        if (hit == false || hit.collider == null || hit.collider.gameObject == null)
            return;
        GameObject hitObject = hit.collider.gameObject;
        if (hitObject.tag == "StartNode")
            return;
        if (hitObject.tag.Contains("Node") && hitObject.GetInstanceID() == currentDestination.GetInstanceID() && hitObject.tag != "EndNode"){
            tileController.UpdateNode(hitObject);
            AddNode(hitObject);
        }else if (hitObject.tag == "EndNode" && hitObject.GetInstanceID() == currentDestination.GetInstanceID())
            gameObject.GetComponent<WinCondition>().DidWin();
        else if (hitObject.GetInstanceID() == currentStart.GetInstanceID())
            ReassessDirection();
    }

    private RaycastHit2D Get2DHitOnPipe(Vector3 position){
        Vector2 position2D = new Vector2(position.x, position.y);
        return Physics2D.Raycast(position2D, Vector2.zero, nodeLayer);
    }

    private RaycastHit2D Get2DHitOnNode(Vector3 position){
        Vector2 position2D = new Vector2(position.x, position.y);
        return Physics2D.Raycast(position2D, Vector2.zero, pipeLayer); 
    }

    public void AddNode(GameObject node){
        Vector3 position = node.transform.position;
        drawMeshLine.AddLinePoint(position, path);
        path = NO_PATH;
        currentStart = tileController.GetCurrentStartNode();
        SetStartPosition();
    }

    private void ResetLine(){
        makeLine = false;
        path = NO_PATH;
        currentStart = tileController.GetOriginalStartNode();
        SetStartPosition();
        drawMeshLine.ResetLine();
        tileController.SetAllNodesAsInactive();
        tileController.ResetArrays();
        tileController.SetCurrentNode();
    }

    private Vector3 ConstrainLine(Vector3 pointerPosition){
        Vector3 currentPosition = currentStart.transform.position;
        Vector3 destination = currentDestination.transform.position;
        switch (path){
            // NO_PATH //
            case 0:
                return pointerPosition;
            // NORTH //
            case 1:
                // Constrain Line Left and Right //
                pointerPosition.x = currentPosition.x;
                // Constrain Line Bottom to Start Point //
                if (pointerPosition.y < currentPosition.y)
                    pointerPosition.y = currentPosition.y;
                // Constrain Line Top to End Point //
                if (pointerPosition.y > destination.y)
                    pointerPosition.y = destination.y;
                return pointerPosition;
            // EAST //
            case 2:
                // Constrain Line Top and Bottom //
                pointerPosition.y = currentPosition.y;
                // Constrain Line Left to Start Point //
                if (pointerPosition.x < currentPosition.x)
                    pointerPosition.x = currentPosition.x;
                // Constrain Line Right to End Point //
                if (pointerPosition.x > destination.x)
                    pointerPosition.x = destination.x;
                return pointerPosition;
            // SOUTH //
            case 3:
                // Constrain Line Left and Right //
                pointerPosition.x = currentPosition.x;
                // Constrain Top to Start Point //
                if (pointerPosition.y > currentPosition.y)
                    pointerPosition.y = currentPosition.y;
                // Constrain Bottom to End Point //
                if (pointerPosition.y < destination.y)
                    pointerPosition.y = destination.y;
                return pointerPosition;
            // WEST //
            case 4:
                // Constrain Line Top and Bottom //
                pointerPosition.y = currentPosition.y;
                // Constrain Line Right to Start Point //
                if (pointerPosition.x > currentPosition.x)
                    pointerPosition.x = currentPosition.x;
                // Constrain Line Left to End Point //
                if (pointerPosition.x < destination.x)
                    pointerPosition.x = destination.x;
                return pointerPosition;
            default:
                return pointerPosition;
        }
    }

    private bool Backtracking(Vector3 position, Vector3 previousNodePosition){
        switch (path){
            // NO PATH //
            case 0:
                return false;
            // NORTH //
            case 1:
                if (position.y < previousNodePosition.y)
                    return true;
                return false;
            // EAST //
            case 2:
                if (position.x < previousNodePosition.x)
                    return true;
                return false;
            // SOUTH //
            case 3:
                if (position.y > previousNodePosition.y)
                    return true;
                return false;
            // WEST //
            case 4:
                if (position.x > previousNodePosition.x)
                    return true;
                return false;
        }
        return false;
    }

    private void ReassessDirection(){
        tileController.RemovePipe();
        path = NO_PATH;
    }

    private GameObject GetLastGameObject(List<GameObject> list){
        int position = list.Count - 1;
        return list[position];
    }

    private void SetStartPosition(){
        startPosition = currentStart.transform.position;
    }
}

