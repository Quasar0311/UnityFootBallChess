using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Defines grid position in world
public class Hex
{

    public Hex(HexMap hexMap, int q, int r){
        this.HexMap = hexMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);

        players = new Player[2];
    }


    public readonly int Q;  // Column #
    public readonly int R;  // Row #
    public readonly int S;  // -(Q + R)
    public readonly HexMap HexMap;

    readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;

    float radius = 1f;


    public Player[] players;
    public Ball ball;


    public override string ToString()
    {
        return Q + ", " + R;
    }

    // Returns world position of hex
    public Vector3 Position()
    {
        int modR = 1;

        if(R % 2 == 0) modR = 0;

        return new Vector3(
            HexHorizontalSpacing() * (this.Q - modR/2f),
            0,
            HexVerticalSpacing() * this.R
        );
    }

    public float HexHeight()
    {
        return radius * 2;
    }
    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }

    public static float Distance(Hex a, Hex b)
    {
        return
            Mathf.Max(
                Mathf.Abs(a.Q - b.Q),
                Mathf.Abs(a.R - b.R),
                Mathf.Abs(a.S - b.S)
            );
    }

    public void AddPlayer(Player player, int team)
    {
        if(players == null)
        {
            players = new Player[2];
        }

        players[team] = player;
    }

    public void RemovePlayer(Player player, int team)
    {
        if(players != null)
        {
            players[team] = null;
        }
    }
    
    public void AddBall(Ball Ball)
    {
        ball = Ball;
    }

    public void RemoveBall(Ball Ball)
    {
        ball = null;
    }
}
