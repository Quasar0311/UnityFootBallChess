using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance { set; get; }

    public Animator[] animator;

    private void Start() {
        Update_CurrentFunc = Update_StartGame;

        hexMap = GameObject.FindObjectOfType<HexMap>();

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();

        DontDestroyOnLoad(gameObject);

        animator = hexMap.GetComponentsInChildren<Animator>();
    }


    Vector3 lastMousePosition;
    int mouseDragThreshold = 1;
    Vector3 lastMouseGroundPlanePosition;
    Player selectedPlayer = null;

    List<Hex> hexPathList = new List<Hex>();
    LineRenderer lineRenderer;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    public GameObject ReadyButton;

    HexMap hexMap;
    Hex hexUnderMouse;
    Hex hexLastUnderMouse;

    bool blue = true;
    bool red = true;

    public float panSpeed = 15;
    public float panBorderThickness = 10;
    public LayerMask LayerIDForHexTiles;

    Vector3 cameraTargetOffset;

    public bool CoH = true;
    public static bool moveit = false;

    private Client client;
    private string selectingPlace = "Select|";
    public static int serverstatus = 0;

    private string EnemySelect = "";

    private int sendready = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            CancelUpdateFunc();
        }

        hexUnderMouse = MouseToHex();
        // clearRange();
        // if(hexUnderMouse!=null)
        // {
        //     GameObject hexGO = hexMap.GetHexGO(hexUnderMouse);
        //     MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
        //     mr.material = hexMap.HexMaterials[3];
        // }

        Update_CurrentFunc();

        lastMousePosition = Input.mousePosition;

        // Update_CameraScan();
        // Update_ScrollZoom();

        // if(selectionController.SelectedPlayer != null)
        // {
        //     DrawPath( ( hexPath != null ) ? hexPath : selectionController.SelectedPlayer.GetHexPath() );
        // }
        // else
        // {
        //     DrawPath( null );   // Clear the path display
        // }  
    }

    public void WhoRU(string name)
    {
        Instance = this;
        client = FindObjectOfType<Client>();


        if (name == "Host")
        {
            CoH = true;
        }
        else
        {
            CoH = false;
        }
    }

    public void ServerStatus(string name)
    {
        if (name == "Select")
        {
            serverstatus = 1;
        }

    }

    void Update_StartGame()
    {
        if(Input.GetMouseButtonUp(0))
        {

            if (hexUnderMouse == null) {
                return;
            }
            
            GameObject go = hexMap.GetHexGO(hexUnderMouse);
            MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();

            Player[] ps = hexUnderMouse.players;
            if (ps[0] != null && CoH)
            {
                renderer.material = hexMap.HexMaterials[4];
                selectedPlayer = ps[0];
                hexLastUnderMouse = hexUnderMouse;
                showRed();
                Update_CurrentFunc = Update_PlayerLocation;
            }
            else if (ps[1] != null && !CoH)
            {
                renderer.material = hexMap.HexMaterials[2];
                selectedPlayer = ps[1];
                hexLastUnderMouse = hexUnderMouse;
                showBlue();
                Update_CurrentFunc = Update_PlayerLocation;
            }
        }
    }

    void Update_PlayerLocation()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(hexUnderMouse == null)
            {
                return;
            }
            if(hexMap.teamRed.Contains(selectedPlayer) && hexUnderMouse.R > 2 || hexUnderMouse.players[0] != null)
            {
                return;
            }
            else if(hexMap.teamBlue.Contains(selectedPlayer) && hexUnderMouse.R < 4 || hexUnderMouse.players[1] != null)
            {
                return;
            }

            if (hexUnderMouse == null) {
                return;
            }

            clearRange();
            selectingPlace = selectingPlace + hexUnderMouse.ToString()[0] + "|" + hexUnderMouse.ToString()[3] + "|";
            selectedPlayer.SetHex(hexUnderMouse);
            selectedPlayer = null;
            if (!allPlayersSelected())
            {
                Update_CurrentFunc = Update_StartGame;
            }
            else
            {
                if (CoH)
                {
                    selectingPlace = selectingPlace + "a";
                }
                else
                {
                    selectingPlace = selectingPlace + "b";
                }

                client.Send(selectingPlace);

                Update_CurrentFunc = Update_WaitServerStatus;
            }
        }
    }

    void Update_WaitServerStatus()
    {
        if (serverstatus == 1)
        {
            if (CoH)
            {
                hexMap.teamBlue[0].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[0].ToString()), Int32.Parse(EnemySelect[1].ToString())));
                hexMap.teamBlue[1].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[2].ToString()), Int32.Parse(EnemySelect[3].ToString())));
                hexMap.teamBlue[2].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[4].ToString()), Int32.Parse(EnemySelect[5].ToString())));
            }
            else
            {
                hexMap.teamRed[0].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[0].ToString()), Int32.Parse(EnemySelect[1].ToString())));
                hexMap.teamRed[1].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[2].ToString()), Int32.Parse(EnemySelect[3].ToString())));
                hexMap.teamRed[2].SetHex(hexMap.GetHexAt(Int32.Parse(EnemySelect[4].ToString()), Int32.Parse(EnemySelect[5].ToString())));
            }
            
            Ball ball = new Ball();
            hexMap.SpawnBallAt(ball, 3, 3);
            Update_CurrentFunc = Update_DetectModeStart;
        }
    }

    public void SelectComplete(string a)
    {
        EnemySelect = a;
        serverstatus = 1;
    }

    public void BallComplete(string a)
    {
        if (a.Length == 0)
        {
            ;
        }
        else if (a.Length == 4)
        {
            List<Hex> newPath = new List<Hex>();
            int x = Int32.Parse(a[0].ToString());
            int y = Int32.Parse(a[2].ToString());
            newPath.Add(hexMap.GetHexAt(x, y));
            hexMap.Ball.hexPath = newPath;
        }
        else if (a.Length == 8)
        {
            List<Hex> newPath = new List<Hex>();
            int x = Int32.Parse(a[0].ToString());
            int y = Int32.Parse(a[2].ToString());
            int x1 = Int32.Parse(a[4].ToString());
            int y1 = Int32.Parse(a[6].ToString());
            newPath.Add(hexMap.GetHexAt(x, y));
            newPath.Add(hexMap.GetHexAt(x1, y1));
            hexMap.Ball.hexPath = newPath;
        }
        else
        {
            List<Hex> newPath = new List<Hex>();
            int x = Int32.Parse(a[0].ToString());
            int y = Int32.Parse(a[2].ToString());
            int x1 = Int32.Parse(a[4].ToString());
            int y1 = Int32.Parse(a[6].ToString());
            int x2 = Int32.Parse(a[8].ToString());
            int y2 = Int32.Parse(a[10].ToString());
            newPath.Add(hexMap.GetHexAt(x,y));
            newPath.Add(hexMap.GetHexAt(x1, y1));
            newPath.Add(hexMap.GetHexAt(x2, y2));
            hexMap.Ball.hexPath = newPath;
        }

        
    }

    public void TurnComplete(string a)
    {
        string[] turnData = a.Split('/');
        if (CoH)
        {
            for (int i = 0; i < 3; i++)
            {
                if (turnData[i].Length == 0)
                {
                    ;
                }

                else if (turnData[i].Length == 4)
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    hexMap.teamBlue[i].hexPath = newPath;
                }
                else if (turnData[i].Length == 8)
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    int x1 = Int32.Parse(turnData[i][4].ToString());
                    int y1 = Int32.Parse(turnData[i][6].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    newPath.Add(hexMap.GetHexAt(x1, y1));
                    hexMap.teamBlue[i].hexPath = newPath;
                }
                else
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    int x1 = Int32.Parse(turnData[i][4].ToString());
                    int y1 = Int32.Parse(turnData[i][6].ToString());
                    int x2 = Int32.Parse(turnData[i][8].ToString());
                    int y2 = Int32.Parse(turnData[i][10].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    newPath.Add(hexMap.GetHexAt(x1, y1));
                    newPath.Add(hexMap.GetHexAt(x2, y2));
                    hexMap.teamBlue[i].hexPath = newPath;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (turnData[i].Length == 0)
                {
                    ;
                }
                else if (turnData[i].Length == 4)
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    hexMap.teamRed[i].hexPath = newPath;
                }
                else if (turnData[i].Length == 8)
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    int x1 = Int32.Parse(turnData[i][4].ToString());
                    int y1 = Int32.Parse(turnData[i][6].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    newPath.Add(hexMap.GetHexAt(x1, y1));
                    hexMap.teamRed[i].hexPath = newPath;
                }
                else
                {
                    List<Hex> newPath = new List<Hex>();
                    int x = Int32.Parse(turnData[i][0].ToString());
                    int y = Int32.Parse(turnData[i][2].ToString());
                    int x1 = Int32.Parse(turnData[i][4].ToString());
                    int y1 = Int32.Parse(turnData[i][6].ToString());
                    int x2 = Int32.Parse(turnData[i][8].ToString());
                    int y2 = Int32.Parse(turnData[i][10].ToString());
                    newPath.Add(hexMap.GetHexAt(x, y));
                    newPath.Add(hexMap.GetHexAt(x1, y1));
                    newPath.Add(hexMap.GetHexAt(x2, y2));
                    hexMap.teamRed[i].hexPath = newPath;
                }
            }
        }
        moveit = true;

    }

    bool allPlayersSelected()
    {
        if (CoH)
        {
            for (int team = 0; team < 1; team++)
            {
                for (int p = 0; p < 3; p++)
                {
                    Hex h = hexMap.SelectionHexes[team, p];
                    if (h.players[0] != null || h.players[1] != null)
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            for (int team = 1; team < 2; team++)
            {
                for (int p = 0; p < 3; p++)
                {
                    Hex h = hexMap.SelectionHexes[team, p];
                    if (h.players[0] != null || h.players[1] != null)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    void DrawPath(List<Hex> hexPath)
    {
        if(hexPath == null || hexPath.Count == 0)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] ps = new Vector3[ hexPath.Count ];

        for (int i = 0; i < hexPath.Count; i++)
        {
            GameObject hexGO = hexMap.GetHexGO(hexPath[i]);
            ps[i] = hexGO.transform.position + (Vector3.up*0.5f);
        }

        lineRenderer.positionCount = ps.Length;
        lineRenderer.SetPositions(ps);
    }

    void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;

    //     // Also do cleanup of any UI stuff associated with modes.
        selectedPlayer = null;
        hexPathList = new List<Hex>();
        lineRenderer.enabled = false;
        clearRange();
    }

    void waitUntil()
    {
        if (serverstatus == 2)
        {
            Update_CurrentFunc = Update_DetectModeStart;
        }
    }
    public void waiteall()
    {
        serverstatus = 2;
    }

    void Update_DetectModeStart()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Ball b = hexMap.Ball;
            if (b.team == 1)
            {
                List<string> ya = new List<string>();
                string ballpath = "";
                if (b.hexPath != null && b.hexPath.Count > 1)
                {
                    foreach (Hex h in b.hexPath)
                    {
                        if (h != null)
                        {
                            ballpath += h.Q + "," + h.R + ",";
                        }

                    }
                    ya.Add(ballpath);
                    client.Send("Ball|" + string.Join("", ya) + "|1");
                }
            }
            else
            {
                List<string> ya = new List<string>();
                string ballpath = "";
                if (b.hexPath != null && b.hexPath.Count > 1)
                {
                    foreach (Hex h in b.hexPath)
                    {
                        if (h != null)
                        {
                            ballpath += h.Q + "," + h.R + ",";
                        }

                    }
                    ya.Add(ballpath);
                    client.Send("Ball|" + string.Join("", ya) + "|0");
                }

            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            

            if (CoH)
            {
                
                
                List<string> hey = new List<string>();

                foreach (Player p in hexMap.teamRed)
                {
                    string path = "";
                    if (p.hexPath == null)
                    {
                        ;
                    }
                    else
                    {
                        foreach (Hex h in p.hexPath)
                        {
                            int i = 0;
                            if (h != null)
                            {
                                path += h.Q + "," + h.R + ",";
                            }
                            else
                            {
                                path += "x,x,";
                            }
                        }
                    }
                    
                    path += "/";
                    hey.Add(path);
                }
                client.Send("Turn|" + string.Join("", hey) + "|a");
            }
            else
            {
                List<string> hey = new List<string>();
                foreach (Player p in hexMap.teamBlue)
                {
                    string path = "";
                    foreach (Hex h in p.hexPath)
                    {
                        if (h != null)
                        {
                            path += h.Q + "," + h.R + ",";
                        }
                        else
                        {
                            path += "x,x,";
                        }
                    }
                    path += "/";
                    hey.Add(path);
                }
                client.Send("Turn|" + string.Join("", hey) + "|b");
            }
            Update_CurrentFunc = waitUntil;

        }

        //Select Player on Board
        if (Input.GetMouseButtonUp(0))
        {

            if (hexUnderMouse == null) {
                return;
            }

            Player[] ps = hexUnderMouse.players;

// ###################
// BALL TEAM
// ###################
            if(hexUnderMouse.ball != null)
            {
                if(hexUnderMouse.ball.hexPath != null && hexUnderMouse.ball.hexPath.Count > 1)
                {
                    hexPathList = hexUnderMouse.ball.hexPath;
                    DrawPath(hexPathList);
                    hexLastUnderMouse = hexPathList[hexPathList.Count-1];
                }
                else
                {
                    hexPathList.Add(hexUnderMouse);
                    hexUnderMouse.ball.SetHexPath(hexPathList);
                    hexLastUnderMouse = hexUnderMouse;
                }
                showRange(hexLastUnderMouse);
                Update_CurrentFunc = Update_PlayerMovement;
                return;
            }
            else if(ps[0] != null && CoH)
            {
                selectedPlayer = ps[0];
            }
            else if(ps[1] != null && !CoH)
            {
                selectedPlayer = ps[1];
            }
            else
            {
                return;
            }
            if(selectedPlayer.hexPath != null && selectedPlayer.hexPath.Count > 0 && selectedPlayer.hexPath[0] != null )
            {
                hexPathList = selectedPlayer.hexPath;   
                DrawPath(hexPathList);
                hexLastUnderMouse = hexPathList[hexPathList.Count-1];
            }
            else
            {
                hexPathList.Add(hexUnderMouse);
                selectedPlayer.SetHexPath(hexPathList);
                hexLastUnderMouse = hexUnderMouse;
            }
            showRange(hexLastUnderMouse);
            Update_CurrentFunc = Update_PlayerMovement;
        }
        // else if ( selectionController.SelectedPlayer != null && Input.GetMouseButtonDown(1) )
        // {
        //     // We have a selected unit, and we've pushed down the right
        //     // mouse button, so enter unit movement mode.
        //     Update_CurrentFunc = Update_PlayerMovement;

        // }
        // else if( Input.GetMouseButton(1) && 
        //     Vector3.Distance( Input.mousePosition, lastMousePosition) > mouseDragThreshold )
        // {
        //     // Left button is being held down AND the mouse moved? That's a camera drag!
        //     Update_CurrentFunc = Update_CameraDrag;
        //     lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
        //     Update_CurrentFunc();
        // }
        // else if( Input.GetMouseButtonDown(0))
        // {
        //     // Debug.Log(hexUnderMouse);
        // }
    }

    Hex MouseToHex()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerIDForHexTiles.value;

        if( Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask) )
        {
            // Something got hit
            // Debug.Log( hitInfo.rigidbody.gameObject );

            // The collider is a child of the "correct" game object that we want.
            GameObject hexGO = hitInfo.rigidbody.gameObject;

            return hexMap.GetHexFromGameObject(hexGO);
        }

        return null;
    }

    Vector3 MouseToGroundPlane(Vector3 mousePos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay (mousePos);
        // What is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0) {
            //Debug.LogError("Why is mouse pointing up?");
            return Vector3.zero;
        }
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    void Update_PlayerMovement()
    {
        // Is this a different hex than before (or we don't already have a path)
        if (hexUnderMouse == null) {
            return;
        }
        if(Input.GetMouseButtonUp(0) && hexUnderMouse != hexLastUnderMouse )
        {
            List<Hex> neighbours = getHexRange(hexLastUnderMouse);
            if(!neighbours.Contains(hexUnderMouse))
            {
            }
            else if (hexPathList.Count > 2)
            {
            }
            else
            {
                hexLastUnderMouse = hexUnderMouse;
                clearRange();
                showRange(hexUnderMouse);
                hexPathList.Add(hexUnderMouse);
                DrawPath(hexPathList);
            }
        }

        if(Input.GetKeyUp(KeyCode.R))
        {
            if(hexPathList.Count>1)
            {
                hexPathList.Remove(hexPathList[hexPathList.Count-1]);
                DrawPath(hexPathList);
                clearRange();
                showRange(hexPathList[hexPathList.Count-1]);
            }
        }
    }

    void Update_CameraScan()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);

        Vector3 pos = transform.position;
        Vector3 hitPos = mouseRay.origin + (mouseRay.direction * rayLength);

        if(Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if(Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        
        transform.position = pos;
    }
    void Update_ScrollZoom()
    {
        // Zoom to scrollwheel
        float scrollAmount = Input.GetAxis ("Mouse ScrollWheel");
        float minHeight = 4;
        float maxHeight = 10;
        // Move camera towards hitPos
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);
        Vector3 dir = hitPos - Camera.main.transform.position;

        Vector3 p = Camera.main.transform.position;

        // Stop zooming out at a certain distance.
        // TODO: Maybe you should still slide around at 20 zoom?
        if (scrollAmount > 0 || p.y < (maxHeight - 0.1f)) {
            cameraTargetOffset += dir * scrollAmount;
        }
        Vector3 lastCameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + cameraTargetOffset, Time.deltaTime * 5f);
        cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;


        p = Camera.main.transform.position;
        if (p.y < minHeight) {
            p.y = minHeight;
        }
        if (p.y > maxHeight) {
            p.y = maxHeight;
        }
        Camera.main.transform.position = p;

        // Change camera angle
        Camera.main.transform.rotation = Quaternion.Euler (
            Mathf.Lerp (30, 60, Camera.main.transform.position.y / maxHeight),
            Camera.main.transform.rotation.eulerAngles.y,
            Camera.main.transform.rotation.eulerAngles.z
        );


    }

    void Update_CameraDrag ()
    {
        if( Input.GetMouseButtonUp(1) )
        {
            CancelUpdateFunc();
            return;
        }
        
        // Right now, all we need are camera controls

        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

        Vector3 diff = lastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate (diff, Space.World);

        lastMouseGroundPlanePosition = hitPos = MouseToGroundPlane(Input.mousePosition);
    }

    List<Hex> getHexRange(Hex h)
    {
        List<Hex> neighbours = new List<Hex>();
        int q = h.Q;
        int r = h.R;
        if(h.R % 2 == 0)
        {
            neighbours.Add(hexMap.GetHexAt(q  , r-1));
            neighbours.Add(hexMap.GetHexAt(q+1, r-1));
            neighbours.Add(hexMap.GetHexAt(q+1, r  ));
            neighbours.Add(hexMap.GetHexAt(q+1, r+1));
            neighbours.Add(hexMap.GetHexAt(q  , r+1));
            neighbours.Add(hexMap.GetHexAt(q-1, r  ));
        }
        else
        {
            neighbours.Add(hexMap.GetHexAt(q  , r-1));
            neighbours.Add(hexMap.GetHexAt(q-1, r-1));
            neighbours.Add(hexMap.GetHexAt(q+1, r  ));
            neighbours.Add(hexMap.GetHexAt(q-1, r+1));
            neighbours.Add(hexMap.GetHexAt(q  , r+1));
            neighbours.Add(hexMap.GetHexAt(q-1, r  ));
        }
        return neighbours;
    }

    void showRange(Hex h)
    {
        GameObject centerGo = hexMap.GetHexGO(h);
        List<GameObject> neighbours = new List<GameObject>();
        int q = h.Q;
        int r = h.R;
        if(h.R % 2 == 0)
        {
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q  , r-1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q+1, r-1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q+1, r  )));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q+1, r+1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q  , r+1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q-1, r  )));
        }
        else
        {
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q  , r-1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q-1, r-1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q+1, r  )));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q-1, r+1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q  , r+1)));
            neighbours.Add(hexMap.GetHexGO(hexMap.GetHexAt(q-1, r  )));
        }
        foreach (GameObject go in neighbours)
        {
            if(go != null)
            {
                MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
                renderer.material = hexMap.HexMaterials[1];
            }
        }
    }

    
    void clearRange()
    {
        foreach (Hex h in hexMap.hexes)
        {
            if(h != null)
            {
                GameObject go = hexMap.GetHexGO(h);
                MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
                renderer.material = hexMap.HexMaterials[0];
                // if(h.R == 3 || h.R == 0 || h.R == 6 || h.Q == 0 || h.Q == 6 || (h.Q == 5 && h.R % 2 == 0))
                // {
                //     renderer.material = hexMap.HexMaterials[5];
                // }

                renderer = go.GetComponentsInChildren<MeshRenderer>()[2];
                renderer.material = hexMap.HexMaterials[0];
                
            }
        }
        foreach (Hex h in hexMap.SelectionHexes)
        {
            if(h != null)
            {
                GameObject go = hexMap.GetHexGO(h);
                MeshRenderer renderer = go.GetComponentInChildren<MeshRenderer>();
                renderer.material = hexMap.HexMaterials[1];

                renderer = go.GetComponentsInChildren<MeshRenderer>()[2];
                renderer.material = hexMap.HexMaterials[1];
            }
        }
    }

    void showRed()
    {
        foreach(Hex h in hexMap.hexes)
        {
            if(h != null && h.R < 3 && h.players[0] == null)
            {
                GameObject go = hexMap.GetHexGO(h);
                MeshRenderer renderer = go.GetComponentsInChildren<MeshRenderer>()[2];
                renderer.material = hexMap.HexMaterials[4];
            }
        }
    }
    void showBlue()
    {
        foreach(Hex h in hexMap.hexes)
        {
            if(h != null && h.R > 3 && h.players[1] == null)
            {
                GameObject go = hexMap.GetHexGO(h);
                MeshRenderer renderer = go.GetComponentsInChildren<MeshRenderer>()[2];
                renderer.material = hexMap.HexMaterials[2];
            }
        }
    }

    int? redGoal;
    int? blueGoal;

    void Update_Shoot()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            redGoal = 1;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            redGoal = 2;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            redGoal = 3;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            redGoal = 4;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            redGoal = 5;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            redGoal = 6;
            if (CoH)
            {
                client.Send("Aim|" + redGoal.ToString() + "|0");
                Update_CurrentFunc = Update_DetectModeStart;
            }
            else
            {
                client.Send("Aim|" + redGoal.ToString() + "|1");
                Update_CurrentFunc = Update_DetectModeStart;
            }
        }
        
        
    }

    
    public void Goal()
    {
        hexMap.Ball.SetHex(new Hex(hexMap, 3, 7));
        foreach (Animator a in animator)
        {
            a.Play("JUMP00", -1, 0);
        }
    }
    public void Save()
    {
        hexMap.Ball.SetHex(new Hex(hexMap, 1, 7));
        foreach (Animator a in animator)
        {
            a.Play("JUMP00", -1, 0);
        }
    }
    
    public void getShot()
    {
        Camera.main.transform.position = new Vector3(4.25f, 4, 5);
        Camera.main.transform.Rotate(-20, 0, 0);
        hexMap.canvas.gameObject.SetActive(false);
        Update_CurrentFunc = Update_Shoot;
        for (int i = 0; i < 6; i++)
        {
            hexMap.GDs[1, i].gameObject.SetActive(true);
        }
    }

    public void shootScene()
    {
        client.Send("Shoot|");
        if(hexMap.Ball.team == 0)//Redteam
        {
            Camera.main.transform.position = new Vector3(4.25f,4,5);
            Camera.main.transform.Rotate(-20,0,0);
            hexMap.canvas.gameObject.SetActive(false);
            Update_CurrentFunc = Update_Shoot;
            /*for(int i = 0; i < 6; i++)
            {
                hexMap.GDs[1,i].gameObject.SetActive(true);
            }*/
        }
        else if(hexMap.Ball.team == 1)//Bteam
        {
            Camera.main.transform.position = new Vector3(4.25f,4,2);
            Camera.main.transform.Rotate(-20,180,0);
            hexMap.canvas.gameObject.SetActive(false);
            Update_CurrentFunc = Update_Shoot;
            /*for(int i = 1; i < 6; i++)
            {
                hexMap.GDs[1,i].gameObject.SetActive(true);
            }*/
        }
        else
        {
            Debug.Log("You somehow created a shoot error...");
        }
    }

}