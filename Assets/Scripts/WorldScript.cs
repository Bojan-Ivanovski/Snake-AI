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
    public GameObject BadApple;
    public Dictionary<Vector3,GameObject> SpacesOnScreen;
    public GameObject ObjectUI;

    private void Awake()
    {
        SpacesOnScreen = new Dictionary<Vector3, GameObject>();
        for (int i = -15; i < 16; i++)
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
        if(AppleTimer > 4f)
        {
            List<GameObject> Selections = new List<GameObject>() {Apple, BadApple, Apple, Apple, BadApple};
            SpawnObject(Selections[UnityEngine.Random.Range(0, Selections.Count)]);
            AppleTimer = 0f;
        }
        ObjectUI.GetComponent<TextMeshProUGUI>().text = "Points: " + GlobalPoints;
    }

    void SpawnObject(GameObject ObjectToSpawn)
    {
        for (int i = 0; i < ApplesToLoad; i++)
        {
            bool UseFillFree = true;
            int SmallCounter = 0;
            int X = UnityEngine.Random.Range(-15, 16);
            int Y = UnityEngine.Random.Range(-7, 8);
            Vector3 position = new Vector3(X, Y, 0);

            while (SmallCounter < 10 )
            {
                if (SpacesOnScreen[position] == null)
                {
                    SpacesOnScreen[position] = Instantiate(ObjectToSpawn, position, Quaternion.identity);
                    UseFillFree = false;
                    break;
                }
                X = UnityEngine.Random.Range(-15, 15);
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
