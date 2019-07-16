using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball
{

    public int Movement = 2;
    public int MovementRemaining = 2;
    public bool SkipThisUnit = false;

    public int team = -1;

    public Hex Hex  { get; protected set; }
    
    public delegate void BallMovedDelegate (Hex oldHex, Hex newHex);
    public  BallMovedDelegate OnBallMoved;

    public List<Hex> hexPath;

    public void SetHex( Hex newHex )
    {
        Hex oldHex = Hex;

        if(Hex != null)
        {
            Hex.RemoveBall(this);
        }

        Hex = newHex;

        Hex.AddBall(this);

        if(OnBallMoved != null)
        {
            OnBallMoved(oldHex, newHex);
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
        SkipThisUnit = false;
        this.hexPath = new List<Hex>();
    }
    public void SetHexPath( List<Hex> hexpath )
    {
        SkipThisUnit = false;
        this.hexPath = hexpath;
    }

    public Hex[] GetHexPath()
    {
        return (this.hexPath == null ) ? null : this.hexPath.ToArray();
    }

    public int GetHexPathLength()
    {
        return this.hexPath.Count;
    }

    public void RefreshMovement()
    {
        SkipThisUnit = false;
        MovementRemaining = Movement;
    }
}
