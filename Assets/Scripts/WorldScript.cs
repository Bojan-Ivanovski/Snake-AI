using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    public int GlobalPoints = 0;
    public int ApplesToLoad = 1;
    private float WorldTimer = 0f;
    private float AppleTimer = 0f;
    public GameObject Apple;
    public List<GameObject> ApplesOnScreen = new List<GameObject>();
    public GameObject BadApple;
    public Dictionary<Vector3,GameObject> SpacesOnScreen;
    public GameObject ObjectUI;
    public GameObject Snake;


    public bool useSelected = false;
    public int selectedValue = 0;
    public UnityEngine.UI.Button changeText;
    public Sprite change1;
    public Sprite change2;


    public void SelectedAI(TMP_Dropdown selected)
    {
        selectedValue = selected.value;
    }

    public void UseSelected()
    {
        if (useSelected)
        {
            changeText.image.sprite = change2;
        }
        if (!useSelected)
        {
            changeText.image.sprite = change1;
        }
        useSelected = !useSelected;
    }


    private void Awake()
    {
        SpacesOnScreen = new Dictionary<Vector3, GameObject>();
        for (int i = -13; i < 14; i++)
            for (int j = -7; j < 8; j++)
                SpacesOnScreen.Add(new Vector3(i, j, 0), null);
    }

    void Start()
    {
        SpawnObject(Apple);
    }

    void Update()
    {
        WorldTimer += Time.deltaTime;
        if(WorldTimer > 60f)
            ApplesToLoad = 2;
        
        AppleTimer += Time.deltaTime;
        if(Snake.GetComponent<SnakeScript>().AppleEaten)
        {
            SpawnObject(Apple);

            Snake.GetComponent<SnakeScript>().AppleEaten = false;
        }
        ObjectUI.GetComponent<TextMeshProUGUI>().text = "Points: " + GlobalPoints;
    }

    void SpawnObject(GameObject ObjectToSpawn)
    {
        for (int i = 0; i < ApplesToLoad; i++)
        {
            bool UseFillFree = true;
            int SmallCounter = 0;
            int X = UnityEngine.Random.Range(-13, 14);
            int Y = UnityEngine.Random.Range(-7, 8);
            Vector3 position = new Vector3(X, Y, 0);

            while (SmallCounter < 10 )
            {
                if (SpacesOnScreen[position] == null)
                {
                    GameObject temp = Instantiate(ObjectToSpawn, position, Quaternion.identity);
                    ApplesOnScreen.Add(temp);
                    SpacesOnScreen[position] = temp;
                    UseFillFree = false;
                    break;
                }
                X = UnityEngine.Random.Range(-13, 13);
                Y = UnityEngine.Random.Range(-7, 7);
                position = new Vector3(X, Y, 0);
                SmallCounter++;
            }

            if (UseFillFree)
            {
                foreach (Vector3 key in SpacesOnScreen.Keys)
                {
                    if (SpacesOnScreen[key] == null)
                    {
                        SpacesOnScreen[key] = Instantiate(ObjectToSpawn, key, Quaternion.identity);
                        break;
                    }
                }
            }
                    
        }
    }
}
