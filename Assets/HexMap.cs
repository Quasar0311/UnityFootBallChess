using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour
{
    //Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        mouseController = GameObject.FindObjectOfType<MouseController>();
        canvas = GameObject.FindObjectOfType<Canvas>();
        canvas.gameObject.SetActive(false);
    }

    public bool AnimationIsPlaying = false;



    private void Update() {
        // TESTING: Hit spacebar to advance to next turn
        
        if (MouseController.moveit)
        {
            StartCoroutine("doTurn");
            
        }
        
    }

    public Canvas canvas;
    MouseController mouseController;
    public GameObject HexPrefab;
    public Material[] HexMaterials;
    public Mesh[] HexMesh;

    public MeshRenderer[,] GDs = new MeshRenderer[2,6]; 

    bool ballIsBlack = true;

    public Material[] teamMaterials;
    public int TurnNumber = 0;

    public Hex[,] hexes;
    public Hex[,] SelectionHexes;
    public Dictionary<Hex, GameObject> hexToGameObjectMap;
    private Dictionary<GameObject, Hex> gameObjectToHexMap;

    

    public List<Player> teamRed;
    public List<Player> teamBlue;

    public Ball Ball;
    public GameObject BallGO;
    public Dictionary<Player, GameObject> playerToGameObjectMap;

    public GameObject[] PlayerPrefab;
    public GameObject BallPrefab;

    public GameObject[] Goaldae;

    // public Mesh meshToCollide;

    public readonly int NumColumns = 7;
    public readonly int NumRows = 7;
    public readonly int NumPlayers = 3;


    public Hex GetHexAt(int x, int y){
        if(x >= NumColumns || x < 0 || y >= NumRows || y < 0)
        {
            return null;
        }
        if(hexes == null)
        {
            Debug.LogError("Hexes are not instantiated");
            return null;
        }
        return hexes[x, y];
    }
    public Hex GetSelectionHexAt(int x, int y){
        if(SelectionHexes == null)
        {
            Debug.LogError("Hexes are not instantiated");
            return null;
        }
        return SelectionHexes[x, y];
    }

    public Hex GetHexAt(Vector3 worldLocation){
        if(hexes == null)
        {
            Debug.LogError("Hexes are not instantiated");
            return null;
        }
        
        Hex a = hexes[0,0];
        
        int z = (int)(worldLocation.z / a.HexVerticalSpacing());

        return hexes[a.Q,z];
    }
    
    public Hex GetHexFromGameObject(GameObject hexGO)
    {
        if( gameObjectToHexMap.ContainsKey(hexGO) )
        {
            return gameObjectToHexMap[hexGO];
        }

        return null;
    }

    public GameObject GetHexGO(Hex h)
    {
        if(h == null)
        {
            return null;
        }
        if( hexToGameObjectMap.ContainsKey(h) )
        {
            return hexToGameObjectMap[h];
        }

        return null;
    }

    public GameObject GetPlayerGO(Player c)
    {
        if( playerToGameObjectMap.ContainsKey(c) )
        {
            return playerToGameObjectMap[c];
        }

        return null;
    }

    public Vector3 GetHexPosition(int q, int r)
    {
        Hex hex = GetHexAt(q, r);

        return GetHexPosition(hex);
    }

    public Vector3 GetHexPosition(Hex hex)
    {
        return hex.Position();
    }

    public void GenerateMap()
    {
        hexes = new Hex[NumColumns, NumRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();
        gameObjectToHexMap = new Dictionary<GameObject, Hex>();

        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                if(row % 2 == 0 && column == 6) continue;
                Hex h = new Hex(this, column, row);

                hexes[column, row] = h;

                GameObject hexGO = (GameObject)Instantiate(
                    HexPrefab,
                    h.Position(),
                    Quaternion.identity,
                    this.transform
                );

                hexToGameObjectMap[h] = hexGO;
                gameObjectToHexMap[hexGO] = h;

                hexGO.name = string.Format("HEX: {0},{1}", column, row);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().HexMap = this;

                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", column, row);

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[0];

                // if(h.R == 3 || h.R == 0 || h.R == 6 || h.Q == 0 || h.Q == 6 || (h.Q == 5 && h.R % 2 == 0))
                // {
                //     mr.material = HexMaterials[5];
                // }
                // MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();
                // mf.mesh = HexMesh[0];

                // GameObject SelectedHex = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // SelectedHex.transform.Rotate(0, 30, 0);
                // SelectedHex.transform.localScale = new Vector3(1, 1, 1);
                // SelectedHex.transform.position = hexGO.transform.position;
                // SelectedHex.transform.SetParent(hexGO.transform, true);

                // MeshRenderer SelectedHexMesh = SelectedHex.GetComponent<MeshRenderer>();
                // SelectedHexMesh.enabled = false;
                // MeshCollider SelectedHexmeshcol = SelectedHex.GetComponent<MeshCollider>();
                // SelectedHexmeshcol.sharedMesh = meshToCollide;
                // SelectedHex.AddComponent<ClickHexMap>();
                // SelectedHex.name = hexGO.name + "Collider";
                if(row == 0 && column == 0)
                {
                    GameObject GD = (GameObject)Instantiate(
                        Goaldae[0],
                        new Vector3(Mathf.Sqrt(3)*2.5f, 0, -Mathf.Sqrt(3)),
                        Quaternion.identity,
                        this.transform
                    );
                    MeshRenderer[] tiles = GD.GetComponentsInChildren<MeshRenderer>();
                    for(int i = 1; i < tiles.Length; i++)
                    {
                        GDs[0,i-1] = tiles[i];
                        tiles[i].gameObject.SetActive(false);
                    }
                }

                else if (row == 6 && column == 0)
                {
                    GameObject GD = (GameObject)Instantiate(
                        Goaldae[1],
                        new Vector3(Mathf.Sqrt(3) * 2.5f, 0, Mathf.Sqrt(3)*6.2f),
                        Quaternion.identity,
                        this.transform
                    );
                    MeshRenderer[] tiles = GD.GetComponentsInChildren<MeshRenderer>();
                    for(int i = 1; i < tiles.Length; i++)
                    {
                        GDs[1,i-1] = tiles[i];
                        tiles[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        // Player player1 = new Player();
        // SpawnPlayerAt(player1, PlayerPrefab, 3, 3);

        GenerateSelectionMap();

        // StaticBatchingUtility.Combine(this.gameObject);
    }

    public void GenerateSelectionMap()
    {
        SelectionHexes = new Hex[2, NumPlayers];

        for (int team = 0; team < 2; team++)
        {
            for (int p = 0; p < NumPlayers; p++)
            {
                Hex h;

                if(team == 0)
                {
                    h = new Hex(this, p-1, -1);
                }

                else
                {
                    h = new Hex(this, NumColumns - p, -1);
                }

                SelectionHexes[team, p] = h;

                GameObject hexGO = (GameObject)Instantiate(
                    HexPrefab,
                    h.Position(),
                    Quaternion.identity,
                    this.transform
                );

                hexToGameObjectMap[h] = hexGO;
                gameObjectToHexMap[hexGO] = h;

                hexGO.name = string.Format("SPAWNHEX: {0},{1}", team, p);
                hexGO.GetComponent<HexComponent>().Hex = h;
                hexGO.GetComponent<HexComponent>().HexMap = this;

                hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", team, p);

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[1];
                mr = hexGO.GetComponentsInChildren<MeshRenderer>()[2];
                mr.material = HexMaterials[1];
                
                Player player1 = new Player(team);
                SpawnPlayerAt(player1, PlayerPrefab[team], team, p, team);
            }
        }
        Debug.Log(teamBlue.Count);
        Debug.Log(teamRed.Count);
    }

    public void SpawnPlayerAt(Player player, GameObject prefab, int q, int r, int team)
    {
        if(playerToGameObjectMap == null)
        {
            playerToGameObjectMap = new Dictionary<Player, GameObject>();
        }
        Hex myHex = GetSelectionHexAt(q, r);
        GameObject myHexGO = hexToGameObjectMap[myHex];
        player.SetHex(myHex);
        GameObject playerGO = (GameObject)Instantiate(prefab, myHexGO.transform.position, Quaternion.identity, myHexGO.transform);
        player.OnPlayerMoved += playerGO.GetComponent<PlayerView>().OnPlayerMoved;  
        playerToGameObjectMap.Add(player, playerGO);
        if(team == 1)
        {
            if(teamBlue == null)
            {
                teamBlue = new List<Player>();
            }
            teamBlue.Add(player);
        }
        else
        {
            if(teamRed == null)
            {
                teamRed = new List<Player>();
            }
            teamRed.Add(player);
        }


    }
    
    public void SpawnBallAt(Ball ball, int q, int r)
    {
        Ball = ball;
        Hex myHex = GetHexAt(q, r);
        GameObject myHexGO = hexToGameObjectMap[myHex];
        ball.SetHex(myHex);
        BallGO = (GameObject)Instantiate(BallPrefab, myHexGO.transform.position, Quaternion.identity, myHexGO.transform);
        ball.OnBallMoved += BallGO.GetComponent<BallView>().OnBallMoved;
    }

    private int i = 1;

    IEnumerator doTurn()
    {
        MouseController.moveit = false;
        while (doBallTurn())
        {
            yield return new WaitForSeconds(1);
        }
        
        yield return new WaitForSeconds(1);
        while(doPlayersTurn())
        {
            yield return new WaitForSeconds(1);
        }
        // clearPaths();
        MouseController.serverstatus = 3;
        StopCoroutine("doTurn");
    }

    public bool doPlayersTurn()
    {
        if(teamBlue != null)
        {
            foreach (Player p in teamBlue)
            {
                if(p.DoMove() && p.Hex.ball != null)
                {
                    Debug.Log("first if loop");
                    isBallFree();
                    clearPaths();
                    if (canShoot())
                    {
                        canvas.gameObject.SetActive(true);
                    }
                    return false;
                }
            }
        }
        if(teamRed != null)
        {
            foreach (Player p in teamRed)
            {
                if(p.DoMove() && p.Hex.ball != null)
                {
                    Debug.Log("2 if loop");
                    isBallFree();
                    clearPaths();
                    if (canShoot())
                    {
                        canvas.gameObject.SetActive(true);
                    }
                    return false;
                }
            }
        }
        if (canShoot())
        {
            canvas.gameObject.SetActive(true);
        }
        return pathLeft();
    }
    
    bool canShoot()
    {
        int q = Ball.Hex.Q;
        int r = Ball.Hex.R;
        int ballTeam = Ball.team;
        Debug.Log(ballTeam);
        if(ballTeam == 0)
        {
            if((r == 6 && q > 0 && q < 5) || (r == 5 && q > 1 && q < 5))
            {
                return true;
            }
        }
        if(ballTeam == 1)
        {
            if((r == 0 && q > 0 && q < 5) || (r == 1 && q > 1 && q < 5))
            {
                return true;
            } 
        }
        return false;
    }

    public bool doBallTurn()
    {
        Debug.Log("ball if loop");
        isBallFree();
        Debug.Log(Ball.hexPath);
        return Ball.DoMove();
    }

    public void clearPaths()
    {
        foreach (Player p in teamBlue)
        {
            p.ClearHexPath();
        }
        foreach (Player p in teamRed)
        {
            p.ClearHexPath();
        }
        Ball.ClearHexPath();
    }

    public bool isBallFree()
    {
        MeshRenderer ballMR = BallGO.GetComponentInChildren<MeshRenderer>();
        Debug.Log(Ball);
        if(Ball.Hex.players[0] != null && Ball.Hex.players[1] != null)
        {
            int ballTeam = Random.Range(0,2);
            Ball.team = ballTeam;
            ballMR.material = teamMaterials[ballTeam];
            return false;
        }
        else if(Ball.Hex.players[0] != null)
        {
            Ball.team = 0;
            ballMR.material = teamMaterials[0];
            return false;
        }
        else if(Ball.Hex.players[1] != null)
        {
            Ball.team = 1;
            ballMR.material = teamMaterials[1];
            return false;
        }
        Ball.team = -1;
        ballMR.material = teamMaterials[2];
        return true;
    }
     
    bool pathLeft()
    {
        foreach (Player p in teamBlue)
        {
            if(p.hexPath != null && p.hexPath.Count > 1)
            {
                return true;
            }
        }
        foreach (Player p in teamRed)
        {
            if(p.hexPath != null && p.hexPath.Count > 1)
            {
                return true;
            }
        }
    return false;
    }
}
