using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    SplitterTools.Splitter splitterTools;

    // Start is called before the first frame update
    void Start()
    {
        splitterTools = new SplitterTools.Splitter();

        SplitterTools.SplitterValue value = splitterTools.CountSplitValues(25, 4);

        Debug.Log(value.Count1 + "x " + value.Value1 + " and " + value.Count2 + "x " + value.Value2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
