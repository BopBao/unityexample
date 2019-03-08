using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DrawMeshLine : MonoBehaviour{
    Vector3[] emptyArray;
    Mesh mesh;
    List<Vector3> vertices;
    int[] triangles;
    private readonly float halfLineThickness = 0.35f;
    private readonly float elevation = -1.0f;
    int index = 0;

    private void Awake(){
        emptyArray = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
        mesh = new Mesh();
        mesh.MarkDynamic();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new List<Vector3>(emptyArray);
        CreateTriangles();

    }

    public void CreateLine(Vector3 position, int direction){
        AddEndPoint(position, direction);
        UpdateMesh();
    }

    public void AddLinePoint(Vector3 position, int direction){
        AddEndPoint(position, direction);
        index ++;
        vertices.AddRange(emptyArray);
        CreateTriangles();
    }

    public void RemovePoint(){
        vertices.RemoveRange(vertices.Count - 4, 4);
        index--;
        CreateTriangles();
    }

    public void AddStartPoint(Vector3 startPoint, int direction){
        int position = index * 4;
        switch (direction){
            case 0:
                return;
            // NORTH //
            case 1:
                vertices[position] = WestVertex(startPoint);
                vertices[position + 1] = EastVertex(startPoint);
                break;
            // EAST // 
            case 2:
                vertices[position] = NorthVertex(startPoint);
                vertices[position + 1] = SouthVertex(startPoint);
                break;
            // SOUTH //
            case 3:
                vertices[position] = EastVertex(startPoint);
                vertices[position + 1] = WestVertex(startPoint);
                break;
            // WEST //
            case 4:
                vertices[position] = SouthVertex(startPoint);
                vertices[position + 1] = NorthVertex(startPoint);
                break;
        }
    }

    private void AddEndPoint(Vector3 endPoint, int direction){
        int size = vertices.Count;
        int position = index * 4;
        switch (direction){
            case 0:
                return;
            case 1:
                vertices[position + 2] = (WestVertex(endPoint));
                vertices[position + 3] = (EastVertex(endPoint));
                break;
            case 2:
                vertices[position + 2] = (NorthVertex(endPoint));
                vertices[position + 3] = (SouthVertex(endPoint));
                break;
            case 3:
                vertices[position + 2] = (EastVertex(endPoint));
                vertices[position + 3] = (WestVertex(endPoint));
                break;
            case 4:
                vertices[position + 2] = (SouthVertex(endPoint));
                vertices[position + 3] = (NorthVertex(endPoint));
                break;
        }
    }

    public void ResetLine(){
        index = 0;
        vertices = new List<Vector3>(emptyArray);
        CreateTriangles();
        mesh.Clear();
    }

    void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
    }

    private Vector3 NorthVertex(Vector3 point){
        return new Vector3(point.x, point.y + halfLineThickness, elevation);
    }

    private Vector3 SouthVertex(Vector3 point){
        return new Vector3(point.x, point.y - halfLineThickness, elevation);
    }

    private Vector3 WestVertex(Vector3 point){
        return new Vector3(point.x + halfLineThickness, point.y, elevation);
    }

    private Vector3 EastVertex(Vector3 point){
        return new Vector3(point.x - halfLineThickness, point.y, elevation);
    }

    private void CreateTriangles(){
        int size = (1 + index) * 6;

        triangles = new int[size];
        for(int i = 0; i< index + 1; i++){
            int position = i * 6;
            int vertexPosition = i * 4;

            int vertexStartLeft = vertexPosition;
            int vertexStartRight = vertexPosition + 1;
            int vertextEndLeft = vertexPosition + 2;
            int vertexEndRight = vertexPosition + 3;

            triangles[position] = vertexStartLeft;
            triangles[position + 1] = vertextEndLeft;
            triangles[position + 2] = vertexStartRight;
            triangles[position + 3] = vertexStartRight;
            triangles[position + 4] = vertextEndLeft;
            triangles[position + 5] = vertexEndRight;

        }
    }
}
