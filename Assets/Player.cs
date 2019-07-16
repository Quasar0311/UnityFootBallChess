using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{

    public Player(int team)
    {
        this.team = team;
    }

    public int team;
    public int Movement = 2;
    public int MovementRemaining = 2;
    public bool SkipThisUnit = false;
    
    public Hex Hex  { get; protected set; }
    
    public delegate void PlayerMovedDelegate (Hex oldHex, Hex newHex);
    public  PlayerMovedDelegate OnPlayerMoved;

    public List<Hex> hexPath;

    public void SetHex(Hex newHex)
    {
        Hex oldHex = Hex;

        if(Hex != null)
        {
            Hex.RemovePlayer(this, team);
        }

        Hex = newHex;

        Hex.AddPlayer(this, team);

        if(OnPlayerMoved != null)
        {
            OnPlayerMoved(oldHex, newHex);
        }
    }

    public bool DoMove()
    {
        if(hexPath == null)
        {
            return false;
        }

        
        if(hexPath.Count < 2)
        {
            hexPath = null;
            return false;
        }

        Hex oldHex = hexPath[0];
        Hex newHex = hexPath[1];

        if(hexPath.Count > 0)
        {
            hexPath.RemoveAt(0);
        }
        else
        {
            Debug.Log("no path");
            return false;
        }

        SetHex(newHex);

        return true;
    }

    public void ClearHexPath()
    {
        this.hexPath = new List<Hex>();
    }
    public void SetHexPath( List<Hex> hexpath )
    {
        this.hexPath = hexpath;
    }
}
