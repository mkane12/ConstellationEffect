﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Following Utility Class Unity tutorial: 
// > https://learn.unity.com/tutorial/utility-helper-classes#5cefd11aedbc2a3ea1318b3b

// Static class with static methods will be held indefinitely for life of the program
// > BUT there can't be one per instance of object - static class applies to all instances, so not helpful here
public class TextureHelper
{

    // Rather than have separate tex____ for texture variables, create new class TextureHelper
    // > Adds uniformity to code; can add helper functions to class

    // these variables are for cycling through texture map
    // Reference: https://www.youtube.com/watch?v=cMiY6svKt-s
    public int columns = 4;
    public int rows = 2;
    public int fps = 8;
    private int index;
    // don't necessarily know what columns or rows will be, but this will always return tileSize even if values changed
    // > especially good for relationships between variables
    private Vector2 tileSize {
        get
        {
            return new Vector2(1.0f / columns, 1.0f / rows);
        }
    }
    private Vector2 offset;
    private float delay;

    private Renderer renderer;
    // get cache id for shader property _MainTex - slightly more efficient than searching every time
    private int mtid = Shader.PropertyToID("_MainTex");

    // this method is mostly to set valeus that will not change for a given star over its lifetime
    public void NewStarTex(Renderer r, float d)
    {
        renderer = r;
        renderer.material.SetTextureScale("_MainTex", tileSize);

        offset = new Vector2(tileSize.x, tileSize.y);

        // start index with random offset so twinkling is scattered
        delay = d;
    }

    public void Twinkle()
    {
        int prevIndex = index;

        // calculate index for texture iteration
        // add random time offset, so stars twinkle at varying rates
        index = (int)(Time.time * fps * delay);
        index = index % (columns * rows);

        // split into horizontal and vertical indices
        var uIndex = index % rows;
        var vIndex = index / rows;

        // build offset
        offset.x = uIndex * tileSize.x;
        offset.y = 1.0f - tileSize.y - vIndex * tileSize.y;

        // only update renderer when index has actually changed
        if(prevIndex != index)
        {
            renderer.material.SetTextureOffset(mtid, offset);
        }
    }

}