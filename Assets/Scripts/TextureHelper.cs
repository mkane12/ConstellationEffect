using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Following Utility Class Unity tutorial: 
// > https://learn.unity.com/tutorial/utility-helper-classes#5cefd11aedbc2a3ea1318b3b

// Make a static class; can't inherit  from anything; just static methods to be used
// Static class with static methods will be held indefinitely for life of the program
public static class TextureHelper {

    // TODO Davis: Rather than have separate tex____ for texture variables, create new class textureData, textureHelper, whatever
    // > Adds uniformity to code; can add helper functions to class
    // these variables are for cycling through texture map
    // Reference: https://www.youtube.com/watch?v=cMiY6svKt-s

    public static int columns = 4;
    public static int rows = 2;
    public static int fps = 8;
    private static int index;
    private static Vector2 tileSize;
    private static Vector2 offset;

    public static void Twinkle(Star star, float delay, Renderer renderer)
    {
        // Twinkle should happen regardless of state
        // TODO Davis: move some of this to helper function in new texture class
        // TODO Davis: don't call "new" in update functions; performance issue with cleaning up after that
        // calculate index for texture iteration
        // add random time offset, so stars twinkle at varying rates
        index = (int)(Time.time * fps * delay);
        index = index % (columns * rows);
        tileSize = new Vector2(1.0f / columns, 1.0f / rows);
        // split into horizontal and vertical indices
        var uIndex = index % rows;
        var vIndex = index / rows;
        // build offset
        // TODO Davis: how to update this without calling new
        offset = new Vector2(uIndex * tileSize.x,
            1.0f - tileSize.y - vIndex * tileSize.y);

        // TODO Davis: only pass new value to shader when it's changed rather than pass every frame
        // > cache previous value sent to shader, then check if new value is different before passing
        // TODO Davis: cache id number for shader instead of forcing it to look up id from string every time
        renderer.material.SetTextureOffset("_MainTex", offset);
        renderer.material.SetTextureScale("_MainTex", tileSize);
    }

}
