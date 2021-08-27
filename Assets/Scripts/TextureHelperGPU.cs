using System.Collections;
using System.Collections.Generic;
using TeamLab.Unity;
using UnityEngine;



// Following Utility Class Unity tutorial: 
// > https://learn.unity.com/tutorial/utility-helper-classes#5cefd11aedbc2a3ea1318b3b

// Static class with static methods will be held indefinitely for life of the program
// > BUT there can't be one per instance of object - static class applies to all instances, so not helpful here
public class TextureHelperGPU
{
    static public Data data = ConstellationGUI.data;

    // Rather than have separate tex____ for texture variables, create new class TextureHelper
    // > Adds uniformity to code; can add helper functions to class

    // these variables are for cycling through texture map
    // Reference: https://www.youtube.com/watch?v=cMiY6svKt-s
    public int columns = 4;
    public int rows = 2;
    public int twinkleSpeed;
    private float currIndex;
    private float nextIndex;

    private float blend = 0f;

    // don't necessarily know what columns or rows will be, but this will always return tileSize even if values changed
    // > especially good for relationships between variables
    private Vector2 tileSize {
        get
        {
            return new Vector2(1.0f / columns, 1.0f / rows);
        }
    }
    private Vector2 nextOffset;
    private Vector2 currOffset;
    private float delay;

    // get cache id for shader properties - slightly more efficient than searching every time
    private int currTexID = Shader.PropertyToID("_CurrTex");
    private int nextTexID = Shader.PropertyToID("_NextTex");
    private int blendID = Shader.PropertyToID("_Blend");

    // this method is mostly to set valeus that will not change for a given star over its lifetime
    public void NewStarTex(Material mat, float d)
    {
        mat.SetTextureScale(currTexID, tileSize);
        mat.SetTextureScale(nextTexID, tileSize);

        nextOffset = new Vector2(tileSize.x, tileSize.y);

        // start index with random offset so twinkling is scattered
        delay = d;

        currIndex = (int)(Time.time);
        nextIndex = currIndex;
    }


    /*public void Twinkle(float twinkleSpeed)
    {
        // calculate index for texture iteration
        // add random time offset, so stars twinkle at varying rates
        // Time.time = the time in seconds since the start of the game
        currIndex += (Time.deltaTime) * twinkleSpeed * delay;

        // Time.deltaTime = The completion time in seconds since the last frame
        blend += Time.deltaTime * twinkleSpeed * delay;

        // if we've moved on to next frame
        if ((int)currIndex >= (int)nextIndex)
        {
            nextIndex = currIndex + 1.0f;
            blend = 0;

            // calculate current offset (before texture iterates)
            var uIndexCurr = (int)currIndex % columns;
            var vIndexCurr = (int)currIndex / columns;

            currOffset.x = uIndexCurr * tileSize.x;
            currOffset.y = 1.0f - tileSize.y - vIndexCurr * tileSize.y;

            renderer.material.SetTextureOffset(currTexID, currOffset);

            // split into horizontal and vertical indices
            var uIndexNext = (int)nextIndex % columns;
            var vIndexNext = (int)nextIndex / columns;

            // build offset
            nextOffset.x = uIndexNext * tileSize.x;
            nextOffset.y = 1.0f - tileSize.y - vIndexNext * tileSize.y;

            renderer.material.SetTextureOffset(nextTexID, nextOffset);
        }

        renderer.material.SetFloat(blendID, blend);
    }*/

}
