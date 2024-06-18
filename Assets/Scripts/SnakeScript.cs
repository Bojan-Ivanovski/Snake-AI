using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeScript : MonoBehaviour
{
    private bool SecondTime = false;
    private List<Vector3> SnakeMemory;
    public GameObject WorldObject;
    public GameObject SnakeBody;
    private float SnakeTimer = 0f;
    private Dictionary<Vector3, GameObject> SpacesOnScreen;
    private Vector3 position = new Vector3(-15, 7, -1);
    private Vector3 VectorToAdd = new Vector3(0, -1, 0);
    private bool blocked = false;

    void Start()
    {
        SnakeMemory = new List<Vector3>();
        SpacesOnScreen = WorldObject.GetComponent<NewBehaviourScript>().SpacesOnScreen;
        SpacesOnScreen[new Vector3(position.x, position.y, 0)] = gameObject;
        gameObject.transform.position = position;
        
    }
    
    void Update()
    {
        SnakeTimer += Time.deltaTime;
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && VectorToAdd.y != -1 && VectorToAdd.y != 1 && !blocked)
        {
            VectorToAdd = new Vector3(0, 1, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
            blocked = true;
        }
        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && VectorToAdd.y != 1 && VectorToAdd.y != -1 && !blocked)
        {
            VectorToAdd = new Vector3(0, -1, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            blocked = true;
        }
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && VectorToAdd.x != 1 && VectorToAdd.x != -1 && !blocked)
        {
            VectorToAdd = new Vector3(-1, 0, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
            blocked = true;
        }
        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && VectorToAdd.x != -1 && VectorToAdd.x != 1 && !blocked)
        {
            VectorToAdd = new Vector3(1, 0, 0);
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
            blocked = true;
        }

        if (SnakeTimer > 0.8f) 
        {
            SnakeMemory.Add(position);

            position += VectorToAdd;
            SnakeTimer = 0f;
            blocked = false;

            GameObject checkObject = SpacesOnScreen[new Vector3(position.x, position.y, 0)];

            if (checkObject != null)
            {
                if(checkObject.name.Contains("-1point"))
                {
                    WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints -= 1;
                    if(WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints >= 0)
                    {
                        Vector3 BodyPositionToDelete = SnakeMemory[SnakeMemory.Count - WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints - 2];
                        Destroy(SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)]);
                        SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)] = null;
                    }
                }
                else if (checkObject.name.Contains("1point"))
                    WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints += 1;
                if (checkObject.name.Contains("SnakeBody") || WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints < 0)
                {
                    SceneManager.LoadScene(2);
                }
                Destroy(SpacesOnScreen[new Vector3(position.x, position.y, 0)]);
            }

            GenerateBody();
        }
        gameObject.transform.position = position;
    }

    void GenerateBody()
    {
        for (int i = 0; i < WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints; i++)
        {
            if (SecondTime)
            {
                Vector3 BodyPositionToDelete = SnakeMemory[SnakeMemory.Count - i - 2];
                Destroy(SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)]);
                SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)] = null;
            }
            Vector3 BodyPosition = SnakeMemory[SnakeMemory.Count - i - 1];
            SpacesOnScreen[new Vector3(BodyPosition.x, BodyPosition.y, 0)] = Instantiate(SnakeBody, BodyPosition, Quaternion.identity);
            SecondTime = true;
        }
    }
}
