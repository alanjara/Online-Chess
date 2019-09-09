using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceColor : MonoBehaviour
{
    Color defaultcolor;
    // Start is called before the first frame update
    void Start()
    {
        defaultcolor = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetColor()
    {
        GetComponent<Renderer>().material.color = defaultcolor;
    }

    public void BecomeRed()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Color.red);
    }
}
