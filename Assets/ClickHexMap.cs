using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHexMap : MonoBehaviour
{
    Material material;

    private void OnMouseDown()
    {
        Debug.Log("sdf");
        Debug.Log(GameObject.Find(name).transform.parent + "click");
        GameObject select = GameObject.Find(name).transform.parent.gameObject;
        surroundhexToWhite(select);



    }
    private void OnMouseUp()
    {
        GameObject select = GameObject.Find(name).transform.parent.gameObject;
        surroundhexToOriginal(select);
    }

    private void surroundhexToWhite(GameObject center)
    {

        GameObject sur1, sur2, sur3, sur4, sur5, sur6;
        MeshRenderer surr1, surr2, surr3, surr4, surr5, surr6;

        string x = name[5].ToString();
        string y = name[7].ToString();

        int intx = Int32.Parse(x);
        int inty = Int32.Parse(y);

        if (inty % 2 == 0)
        {
            sur1 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty - 1).ToString()));

            sur2 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty - 1).ToString()));
            sur3 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty).ToString()));
            sur4 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty + 1).ToString()));
            sur5 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty + 1).ToString()));
            sur6 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty).ToString()));

            if (sur1 != null)
            {
                surr1 = sur1.GetComponentInChildren<MeshRenderer>();
                surr1.material.color = new Color(1, 1, 1, 1);
                


            }
            if (sur2 != null)
            {
                surr2 = sur2.GetComponentInChildren<MeshRenderer>();
                surr2.material.color = new Color(1, 1, 1, 1);
            }
            if (sur3 != null)
            {
                surr3 = sur3.GetComponentInChildren<MeshRenderer>();
                surr3.material.color = new Color(1, 1, 1, 1);
            }
            if (sur4 != null)
            {
                surr4 = sur4.GetComponentInChildren<MeshRenderer>();
                surr4.material.color = new Color(1, 1, 1, 1);
            }
            if (sur5 != null)
            {
                surr5 = sur5.GetComponentInChildren<MeshRenderer>();
                surr5.material.color = new Color(1, 1, 1, 1);
            }
            if (sur6 != null)
            {
                surr6 = sur6.GetComponentInChildren<MeshRenderer>();
                surr6.material.color = new Color(1, 1, 1, 1);
            }
        }

        if (inty % 2 == 1)
        {
            sur1 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty - 1).ToString()));

            sur2 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty - 1).ToString()));
            sur3 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty).ToString()));
            sur4 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty + 1).ToString()));
            sur5 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty + 1).ToString()));
            sur6 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty).ToString()));

            if (sur1 != null)
            {
                surr1 = sur1.GetComponentInChildren<MeshRenderer>();
                surr1.material.color = new Color(1, 1, 1, 1);


            }
            if (sur2 != null)
            {
                surr2 = sur2.GetComponentInChildren<MeshRenderer>();
                surr2.material.color = new Color(1, 1, 1, 1);
            }
            if (sur3 != null)
            {
                surr3 = sur3.GetComponentInChildren<MeshRenderer>();
                surr3.material.color = new Color(1, 1, 1, 1);
            }
            if (sur4 != null)
            {
                surr4 = sur4.GetComponentInChildren<MeshRenderer>();
                surr4.material.color = new Color(1, 1, 1, 1);
            }
            if (sur5 != null)
            {
                surr5 = sur5.GetComponentInChildren<MeshRenderer>();
                surr5.material.color = new Color(1, 1, 1, 1);
            }
            if (sur6 != null)
            {
                surr6 = sur6.GetComponentInChildren<MeshRenderer>();
                surr6.material.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private void surroundhexToOriginal(GameObject center)
    {

        
        MeshRenderer ddsur = center.GetComponentInChildren<MeshRenderer>();
        material = ddsur.material;

        GameObject sur1, sur2, sur3, sur4, sur5, sur6;
        MeshRenderer surr1, surr2, surr3, surr4, surr5, surr6;

        string x = name[5].ToString();
        string y = name[7].ToString();

        int intx = Int32.Parse(x);
        int inty = Int32.Parse(y);

        if (inty % 2 == 0)
        {
            sur1 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty - 1).ToString()));

            sur2 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty - 1).ToString()));
            sur3 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty).ToString()));
            sur4 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty + 1).ToString()));
            sur5 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty + 1).ToString()));
            sur6 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty).ToString()));

            if (sur1 != null)
            {
                surr1 = sur1.GetComponentInChildren<MeshRenderer>();
                surr1.material.color = material.color;



            }
            if (sur2 != null)
            {
                surr2 = sur2.GetComponentInChildren<MeshRenderer>();
                surr2.material.color = material.color;
            }
            if (sur3 != null)
            {
                surr3 = sur3.GetComponentInChildren<MeshRenderer>();
                surr3.material.color = material.color;
            }
            if (sur4 != null)
            {
                surr4 = sur4.GetComponentInChildren<MeshRenderer>();
                surr4.material.color = material.color;
            }
            if (sur5 != null)
            {
                surr5 = sur5.GetComponentInChildren<MeshRenderer>();
                surr5.material.color = material.color;
            }
            if (sur6 != null)
            {
                surr6 = sur6.GetComponentInChildren<MeshRenderer>();
                surr6.material.color = material.color;
            }
        }

        if (inty % 2 == 1)
        {
            sur1 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty - 1).ToString()));

            sur2 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty - 1).ToString()));
            sur3 = GameObject.Find("HEX: " + ((intx + 1).ToString()) + "," + ((inty).ToString()));
            sur4 = GameObject.Find("HEX: " + ((intx).ToString()) + "," + ((inty + 1).ToString()));
            sur5 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty + 1).ToString()));
            sur6 = GameObject.Find("HEX: " + ((intx - 1).ToString()) + "," + ((inty).ToString()));

            if (sur1 != null)
            {
                surr1 = sur1.GetComponentInChildren<MeshRenderer>();
                surr1.material.color = material.color;


            }
            if (sur2 != null)
            {
                surr2 = sur2.GetComponentInChildren<MeshRenderer>();
                surr2.material.color = material.color;
            }
            if (sur3 != null)
            {
                surr3 = sur3.GetComponentInChildren<MeshRenderer>();
                surr3.material.color = material.color;
            }
            if (sur4 != null)
            {
                surr4 = sur4.GetComponentInChildren<MeshRenderer>();
                surr4.material.color = material.color;
            }
            if (sur5 != null)
            {
                surr5 = sur5.GetComponentInChildren<MeshRenderer>();
                surr5.material.color = material.color;
            }
            if (sur6 != null)
            {
                surr6 = sur6.GetComponentInChildren<MeshRenderer>();
                surr6.material.color = material.color;
            }
        }
    }
}