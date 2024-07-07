using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class SnakeScript : MonoBehaviour
{
    private bool SecondTime = false;
    private List<Vector3> SnakeMemory;
    public GameObject WorldObject;
    public GameObject StartAiButton;
    public GameObject SnakeBody;
    public GameObject SnakePath;
    public GameObject SnakeRedPath;
    public Stack<GameObject> paths = new Stack<GameObject>();

    private int AICounter = 0;
    private List<SnakePos> SnakePosList = new List<SnakePos>();

    private bool AppleFound = false;
    public bool AppleEaten = false;

    private float SnakeTimer = 0f;
    private Dictionary<Vector3, GameObject> SpacesOnScreen;
    private Vector3 position = new Vector3(-13, 7, -1);
    private Vector3 VectorToAdd = new Vector3(0, -1, 0);
    private bool blocked = false;
    public bool blockedByAI = false;

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
        if (!WorldObject.GetComponent<NewBehaviourScript>().useSelected)
        {
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
            blockedByAI = false;
        }
        else
        {
            blockedByAI = true;
            StartAI();
        }

        if (SnakeTimer > 0.8f && !blockedByAI) 
        {
            SnakeMemory.Add(position);

            position += VectorToAdd;
            SnakeTimer = 0f;
            blocked = false;

            if(!SpacesOnScreen.ContainsKey(new Vector3(position.x, position.y, 0)))
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                GameObject checkObject = SpacesOnScreen[new Vector3(position.x, position.y, 0)];

                if (checkObject != null)
                {
                    if (checkObject.name.Contains("-1point"))
                    {
                        WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints -= 1;
                        if (WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints >= 0)
                        {
                            Vector3 BodyPositionToDelete = SnakeMemory[SnakeMemory.Count - WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints - 2];
                            Destroy(SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)]);
                            SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)] = null;
                        }
                    }
                    else if (checkObject.name.Contains("1point"))
                    {
                        AppleEaten = true;
                        AppleFound = false;
                        WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints += 1;
                        Debug.Log(WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Count);
                        WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Remove(SpacesOnScreen[new Vector3(position.x, position.y, 0)]);
                        Debug.Log(WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Count);
                    }
                    if (checkObject.name.Contains("SnakeBody") || WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints < 0)
                    {
                        SceneManager.LoadScene(2);
                    }
                    Destroy(SpacesOnScreen[new Vector3(position.x, position.y, 0)]);
                }
            
            
            }

            GenerateBody();
        }

        if (SnakeTimer > 0.4f && blockedByAI && AppleFound)
        {
            SnakePos snake = SnakePosList[AICounter];
            if (AICounter < SnakePosList.Count)
                AICounter++;
            SnakeMemory.Add(position);

            position = snake.Position;
            SnakeTimer = 0f;

            Dictionary<string, Quaternion> rotations = new Dictionary<string, Quaternion>
            {
                { "U", Quaternion.Euler(0, 0, 180) },
                { "D", Quaternion.Euler(0, 0, 0) },
                { "L", Quaternion.Euler(0, 0, 270) },
                { "R", Quaternion.Euler(0, 0, 90) }
            };

            gameObject.transform.rotation = rotations[snake.Rotation];

            GameObject checkObject = SpacesOnScreen[new Vector3(position.x, position.y, 0)];

            if (checkObject != null)
            {
                if (checkObject.name.Contains("-1point"))
                {
                    WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints -= 1;
                    if (WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints >= 0)
                    {
                        Vector3 BodyPositionToDelete = SnakeMemory[SnakeMemory.Count - WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints - 2];
                        Destroy(SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)]);
                        SpacesOnScreen[new Vector3(BodyPositionToDelete.x, BodyPositionToDelete.y, 0)] = null;
                    }
                }
                else if (checkObject.name.Contains("1point"))
                {
                    AppleEaten = true;
                    AppleFound = false;
                    WorldObject.GetComponent<NewBehaviourScript>().GlobalPoints += 1;
                    Debug.Log(WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Count);
                    WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Remove(SpacesOnScreen[new Vector3(position.x, position.y, 0)]);
                    Debug.Log(WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen.Count);
                }
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

    class SnakePos
    {
        public Vector3 Position;
        public string Rotation;
        public SnakePos Past;
        public int Cost;
    }

    double ManhatenDistance(Vector3 one , Vector3 two)
    {
        return Math.Abs(one.x - two.x) + Math.Abs(one.y - two.y);
    }

    double Heruistic(SnakePos obj)
    {
        PriorityQueue<Vector3, double> min = new PriorityQueue<Vector3, double>();
        foreach (GameObject apple in WorldObject.GetComponent<NewBehaviourScript>().ApplesOnScreen)
        {
            Vector3 temp = apple.transform.position;
            min.Enqueue(temp, ManhatenDistance(obj.Position, temp));
        }
        return ManhatenDistance(min.Dequeue(), obj.Position);
    }

    bool validMove(SnakePos check, SnakePos current)
    {
        GameObject checkState = SpacesOnScreen[check.Position];
        if (current.Rotation == "U" && check.Rotation == "D")
            return false;
        if (current.Rotation == "D" && check.Rotation == "U")
            return false;
        if (current.Rotation == "R" && check.Rotation == "L")
            return false;
        if (current.Rotation == "L" && check.Rotation == "R")
            return false;
        if (checkState != null && checkState.name.Contains("-1point"))
            return false;
        if (checkState != null && checkState.name.Contains("1point"))
            return true;
        if (checkState == null)
            return true;
        return false;
    }


    bool isGoal(GameObject checkState)
    {
        if (checkState != null && checkState.name.Contains("-1point"))
            return false;
        if (checkState != null && checkState.name.Contains("1point"))
            return true;
        return false;
    }


    Dictionary<string, SnakePos> States(SnakePos currentState)
    {

        Vector3 currentSnakePos = currentState.Position;

        Dictionary<string, SnakePos> newStates = new Dictionary<string, SnakePos>();
        SnakePos keepGoing = new SnakePos();




        SnakePos up = new SnakePos() {
            Position = new Vector3(currentSnakePos.x, currentSnakePos.y + 1, 0),
            Rotation = "U",
            Past = currentState,
            Cost = currentState.Cost + 1
        };
        SnakePos right = new SnakePos()
        {
            Position = new Vector3(currentSnakePos.x + 1, currentSnakePos.y, 0),
            Rotation = "R",
            Past = currentState,
            Cost = currentState.Cost + 1
        };
        SnakePos down = new SnakePos()
        {
            Position = new Vector3(currentSnakePos.x, currentSnakePos.y - 1, 0),
            Rotation = "D",
            Past = currentState,
            Cost = currentState.Cost + 1
        };
        SnakePos left = new SnakePos()
        {
            Position = new Vector3(currentSnakePos.x - 1, currentSnakePos.y, 0),
            Rotation = "L",
            Past = currentState,
            Cost = currentState.Cost + 1
        };

        if (currentState.Rotation == "U")
            keepGoing = up;
        if (currentState.Rotation == "D")
            keepGoing = down;
        if (currentState.Rotation == "R")
            keepGoing = right;
        if (currentState.Rotation == "L")
            keepGoing = left;

        if (SpacesOnScreen.ContainsKey(up.Position) && validMove(up, currentState) && currentState.Rotation != "U")
            newStates.Add("Up",up);
        if (SpacesOnScreen.ContainsKey(right.Position) && validMove(right, currentState) && currentState.Rotation != "R")
            newStates.Add("Right", right);
        if (SpacesOnScreen.ContainsKey(left.Position) && validMove(left, currentState) && currentState.Rotation != "L")
            newStates.Add("Left", left);
        if (SpacesOnScreen.ContainsKey(down.Position) && validMove(down, currentState) && currentState.Rotation != "D")
            newStates.Add("Down", down);

        if (SpacesOnScreen.ContainsKey(keepGoing.Position) && validMove(keepGoing, currentState))
            newStates.Add("KeepGoing", keepGoing);

        return newStates;
    }


    void DepthFirstSearch(Vector3 firstState, string firstRotation)
    {
        SnakePos state = new SnakePos() { Position = firstState, Rotation = firstRotation, Past = null };
        Dictionary<string, SnakePos> states = new Dictionary<string, SnakePos>();
        Stack<SnakePos> actions = new Stack<SnakePos>();
        List<Vector3> visited = new List<Vector3>();

        int counterS = 0;

        actions.Push(state);

        while (paths.Count > 0 && !AppleFound)
        {
            Destroy(paths.Pop());
        }


        while (actions.Count > 0 && !AppleFound)
        {
            counterS++;
            SnakePos node = actions.Pop();
            if (visited.Contains(node.Position))
                continue;
            visited.Add(node.Position);
            Debug.Log(node.Position + " " + node.Rotation);
            paths.Push(Instantiate(SnakePath, node.Position, Quaternion.identity));

            if (SpacesOnScreen.ContainsKey(node.Position) && isGoal(SpacesOnScreen[node.Position]))
            {
                AICounter = 0;
                SnakePosList.Clear();
                paths.Push(Instantiate(SnakeRedPath, node.Position, Quaternion.identity));
                AppleFound = true;
                SnakePos back = node;
                while (back.Past != null)
                {
                    paths.Push(Instantiate(SnakeRedPath, back.Past.Position, Quaternion.identity));
                    SnakePosList.Add(back);
                    back = back.Past;
                }
                SnakePosList.Reverse();
                Debug.Log("WOOO " + node.Past.Past);
                break;
            }

            states = States(node);
            foreach (string key in states.Keys)
                actions.Push(states[key]);
        }

        if(!AppleFound)
            SceneManager.LoadScene(2);
    }

    void BredthFirstSearch(Vector3 firstState, string firstRotation)
    {
        SnakePos state = new SnakePos() { Position = firstState, Rotation = firstRotation , Past = null};
        Dictionary<string, SnakePos> states = new Dictionary<string, SnakePos>();
        Queue<SnakePos> actions = new Queue<SnakePos>();
        List<Vector3> visited = new List<Vector3>();
        
        int counterS = 0;

        actions.Enqueue(state);

        while(paths.Count > 0 && !AppleFound)
        {
            Destroy(paths.Pop());
        }


        while (actions.Count > 0 && !AppleFound)
        {
            counterS++;
            SnakePos node = actions.Dequeue();
            if (visited.Contains(node.Position))
                continue;
            visited.Add(node.Position);
            Debug.Log(node.Position + " " + node.Rotation);
            paths.Push(Instantiate(SnakePath, node.Position, Quaternion.identity));

            if (SpacesOnScreen.ContainsKey(node.Position) && isGoal(SpacesOnScreen[node.Position]))
            {
                AICounter = 0;
                SnakePosList.Clear();
                paths.Push(Instantiate(SnakeRedPath, node.Position, Quaternion.identity));
                AppleFound = true;
                SnakePos back = node;
                while(back.Past != null)
                {
                    paths.Push(Instantiate(SnakeRedPath, back.Past.Position, Quaternion.identity));
                    SnakePosList.Add(back);
                    back = back.Past;
                }
                SnakePosList.Reverse();
                Debug.Log("WOOO " + node.Past.Past);
                break;
            }

            states = States(node);
            foreach (string key in states.Keys)
                actions.Enqueue(states[key]);
        }

        if (!AppleFound)
            SceneManager.LoadScene(2);
    }

    void UniformCostSearch(Vector3 firstState, string firstRotation)
    {
        SnakePos state = new SnakePos() { Position = firstState, Rotation = firstRotation, Past = null , Cost = 0};
        Dictionary<string, SnakePos> states = new Dictionary<string, SnakePos>();
        PriorityQueue<SnakePos, int> actions = new PriorityQueue<SnakePos, int>();
        List<Vector3> visited = new List<Vector3>();


        int counterS = 0;

        actions.Enqueue(state, state.Cost);

        while (paths.Count > 0 && !AppleFound)
        {
            Destroy(paths.Pop());
        }


        while (actions.Count > 0 && !AppleFound)
        {
            counterS++;
            SnakePos node = actions.Dequeue();
            if (visited.Contains(node.Position))
                continue;
            Debug.Log(node.Position + " " + node.Rotation);
            visited.Add(node.Position);
            paths.Push(Instantiate(SnakePath, node.Position, Quaternion.identity));

            if (SpacesOnScreen.ContainsKey(node.Position) && isGoal(SpacesOnScreen[node.Position]))
            {
                AICounter = 0;
                SnakePosList.Clear();
                paths.Push(Instantiate(SnakeRedPath, node.Position, Quaternion.identity));
                AppleFound = true;
                SnakePos back = node;
                while (back.Past != null)
                {
                    paths.Push(Instantiate(SnakeRedPath, back.Past.Position, Quaternion.identity));
                    SnakePosList.Add(back);
                    back = back.Past;
                }
                SnakePosList.Reverse();
                Debug.Log("WOOO " + node.Past.Past);
                break;
            }

            states = States(node);
            foreach (string key in states.Keys)
                actions.Enqueue(states[key], states[key].Cost);
        }


        if (!AppleFound)
            SceneManager.LoadScene(2);
    }

    void GreedySearch(Vector3 firstState, string firstRotation)
    {
        SnakePos state = new SnakePos() { Position = firstState, Rotation = firstRotation, Past = null, Cost = 0 };
        Dictionary<string, SnakePos> states = new Dictionary<string, SnakePos>();
        PriorityQueue<SnakePos, double> actions = new PriorityQueue<SnakePos, double>();
        List<Vector3> visited = new List<Vector3>();


        int counterS = 0;

        actions.Enqueue(state, Heruistic(state));

        while (paths.Count > 0 && !AppleFound)
        {
            Destroy(paths.Pop());
        }


        while (actions.Count > 0 && !AppleFound)
        {
            counterS++;
            SnakePos node = actions.Dequeue();
            if (visited.Contains(node.Position))
                continue;
            Debug.Log(node.Position + " " + node.Rotation);
            visited.Add(node.Position);
            paths.Push(Instantiate(SnakePath, node.Position, Quaternion.identity));

            if (SpacesOnScreen.ContainsKey(node.Position) && isGoal(SpacesOnScreen[node.Position]))
            {
                AICounter = 0;
                SnakePosList.Clear();
                paths.Push(Instantiate(SnakeRedPath, node.Position, Quaternion.identity));
                AppleFound = true;
                SnakePos back = node;
                while (back.Past != null)
                {
                    paths.Push(Instantiate(SnakeRedPath, back.Past.Position, Quaternion.identity));
                    SnakePosList.Add(back);
                    back = back.Past;
                }
                SnakePosList.Reverse();
                Debug.Log("WOOO " + node.Past.Past);
                break;
            }

            states = States(node);
            foreach (string key in states.Keys)
                actions.Enqueue(states[key], Heruistic(states[key]));
        }


        if (!AppleFound)
            SceneManager.LoadScene(2);
    }

    void AstarSearch(Vector3 firstState, string firstRotation)
    {
        SnakePos state = new SnakePos() { Position = firstState, Rotation = firstRotation, Past = null, Cost = 0 };
        Dictionary<string, SnakePos> states = new Dictionary<string, SnakePos>();
        PriorityQueue<SnakePos, double> actions = new PriorityQueue<SnakePos, double>();
        List<Vector3> visited = new List<Vector3>();


        int counterS = 0;

        actions.Enqueue(state, Heruistic(state) + state.Cost);

        while (paths.Count > 0 && !AppleFound)
        {
            Destroy(paths.Pop());
        }


        while (actions.Count > 0 && !AppleFound)
        {
            counterS++;
            SnakePos node = actions.Dequeue();
            if (visited.Contains(node.Position))
                continue;
            Debug.Log(node.Position + " " + node.Rotation);
            visited.Add(node.Position);
            paths.Push(Instantiate(SnakePath, node.Position, Quaternion.identity));

            if (SpacesOnScreen.ContainsKey(node.Position) && isGoal(SpacesOnScreen[node.Position]))
            {
                AICounter = 0;
                SnakePosList.Clear();
                paths.Push(Instantiate(SnakeRedPath, node.Position, Quaternion.identity));
                AppleFound = true;
                SnakePos back = node;
                while (back.Past != null)
                {
                    paths.Push(Instantiate(SnakeRedPath, back.Past.Position, Quaternion.identity));
                    SnakePosList.Add(back);
                    back = back.Past;
                }
                SnakePosList.Reverse();
                Debug.Log("WOOO " + node.Past.Past);
                break;
            }

            states = States(node);
            foreach (string key in states.Keys)
                actions.Enqueue(states[key], Heruistic(states[key]) + states[key].Cost);
        }


        if (!AppleFound)
            SceneManager.LoadScene(2);
    }

    public void StartAI()
    {
        string rotation = "";
        if(gameObject.transform.rotation == Quaternion.Euler(0, 0, 0))
            rotation = "D";
        if (gameObject.transform.rotation == Quaternion.Euler(0, 0, 90))
            rotation = "R";
        if (gameObject.transform.rotation == Quaternion.Euler(0, 0, 180))
            rotation = "U";
        if (gameObject.transform.rotation == Quaternion.Euler(0, 0, 270))
            rotation = "L";
        
        if(WorldObject.GetComponent<NewBehaviourScript>().selectedValue == 0)
            BredthFirstSearch(new Vector3(position.x, position.y, 0), rotation);
        if (WorldObject.GetComponent<NewBehaviourScript>().selectedValue == 1)
            DepthFirstSearch(new Vector3(position.x, position.y, 0), rotation);
        if (WorldObject.GetComponent<NewBehaviourScript>().selectedValue == 2)
            UniformCostSearch(new Vector3(position.x, position.y, 0), rotation);
        if (WorldObject.GetComponent<NewBehaviourScript>().selectedValue == 3)
            GreedySearch(new Vector3(position.x, position.y, 0), rotation);
        if (WorldObject.GetComponent<NewBehaviourScript>().selectedValue == 4)
            AstarSearch(new Vector3(position.x, position.y, 0), rotation);

    }



}
