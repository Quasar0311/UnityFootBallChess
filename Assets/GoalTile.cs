using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Defines grid position in world
public class GoalTile
{

    public GoalTile(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    Hex dummy = new Hex(new HexMap(), 0,0);

    public readonly int X;  // Column #
    public readonly int Y;  // Row #

    public readonly float width = 1.89f;
    public readonly float height = 1.12f;

    // Returns world position of hex
    public Vector3 Position()
    {
        return new Vector3(
            width*X,
            height*Y,
            0
        );
    }

    public float TileHeight()
    {
        return width;
    }
    public float TileWidth()
    {
        return height;
    }

    public float VerticalShift()
    {
        return TileHeight()/2;
    }

    /*public float HorizontalShift()
    {
        return ;
    }


    public void AddPan(Player player)
    {
    }

    public void RemovePan(Player player)
    {
    }*/
}
