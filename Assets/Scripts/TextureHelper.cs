using System.Collections;
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
    public int fps;
    private int nextIndex;

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

    private Renderer renderer;
    // get cache id for shader properties - slightly more efficient than searching every time
    private int currTexID = Shader.PropertyToID("_CurrTex");
    private int nextTexID = Shader.PropertyToID("_NextTex");
    private int blendID = Shader.PropertyToID("_Blend");

    // this method is mostly to set valeus that will not change for a given star over its lifetime
    public void NewStarTex(Renderer r, float d)
    {
        renderer = r;
        renderer.material.SetTextureScale(currTexID, tileSize);
        renderer.material.SetTextureScale(nextTexID, tileSize);

        fps = StarManager.Instance.fps;

        nextOffset = new Vector2(tileSize.x, tileSize.y);

        // start index with random offset so twinkling is scattered
        delay = d;
    }

    public void Twinkle()
    {
        // 0 = 0
        // 1 = 1
        // 2 = 2
        // 3 = 3
        // 4 = 4
        // 5 = 5
        // 6 = 6
        // 7 = 7
        // 8 = 0

        int currIndex = nextIndex;

        // calculate current offset (before texture iterates)
        var uIndexCurr = currIndex % columns;
        var vIndexCurr = currIndex / columns;

        currOffset.x = uIndexCurr * tileSize.x;
        currOffset.y = 1.0f - tileSize.y - vIndexCurr * tileSize.y;//1.0f - tileSize.y - vIndexCurr * tileSize.y;

        renderer.material.SetTextureOffset(currTexID, currOffset);

        // calculate index for texture iteration
        // add random time offset, so stars twinkle at varying rates
        nextIndex = (int)(Time.time * fps * delay); 
        nextIndex = nextIndex % (columns * rows);

        // split into horizontal and vertical indices
        var uIndexNext = nextIndex % columns;
        var vIndexNext = nextIndex / columns;

        // build offset
        nextOffset.x = uIndexNext * tileSize.x;
        nextOffset.y = 1.0f - tileSize.y - vIndexNext * tileSize.y;

        // TODO: Now that we're blending between sprites, should be updating renderer every frame!
        renderer.material.SetTextureOffset(nextTexID, nextOffset);

        blend += Time.deltaTime;// * fps;

        if (blend > 1.0f)
        {
            blend = 0;
        }

        // TODO: seems smoother maybe? but still kinda bumpy... maybe need to multiply blend by something...
        renderer.material.SetFloat(blendID, blend);
    }

}
