using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainTest : MonoBehaviour
{
    public Image[] tiles;

    private TapCountProgress progress;

    private int tapCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        progress = new TapCountProgress(tiles);
        progress.UpdateTapCount(tapCount);
    }

    public void Tap()
    {
        tapCount++;
        tapCount %= tiles.Length;
        progress.UpdateTapCount(tapCount);
    }
}


public class TapCountProgress
{
    Image[] tiles;

    public TapCountProgress(Image[] tiles)
    {
        this.tiles = tiles;
    }

    public void UpdateTapCount(int tapCount)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (i < tapCount)
            {
                tiles[i].color = Color.red;
            } else
            {
                tiles[i].color = Color.white;
            }
        }
    }
}