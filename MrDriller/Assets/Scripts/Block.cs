using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor {
    Red,
    Blue,
    Green,
    Yellow
}
//väri: 4 eri väriä

public enum BlockState {
    Static,
    Hold,
    Falling,
    LevelEnd
}

public class Block : MonoBehaviour {

    public float velocity;
    //public bool falling;
    //tila: staattinen vai tippumassa?
    public int gridPos;
    // paikka taulukossa?
    //ja sit onko blokki perus, X vai AIR
    public float holdTimer = 2f;
    public BlockState bs;

	void Start () {
		
	}
	
	void Update () {
        // if block underneath destroyed, hold & wobble for 2 seconds, falling = true

        //if (no block underneath) {
        //wobble
        if (bs == BlockState.Hold) {
            holdTimer -= Time.deltaTime;
            print(holdTimer);
        }

        if (holdTimer < 0) {
            bs = BlockState.Falling;
            holdTimer = 2f;
        }

        if(bs == BlockState.Falling) {
            Fall();
        }

        ///then fall with velocity, 
        ///then stop on top of next block OR merge with a same color block
        ///

        ///when the block is falling:
        /// check if block's center would pass (by unity yksikkö?? how??)
        ///to the next ruutu where there already is a static block
        ///if would, snap to the previous ruutu
        ///if not, keep falling
        ///

        ///check if there's a block of the same color on either side
        ///
        ///if (LeftBlock.BlockColor == gameObject.BlockColor) {
        /// do something }
        /// 
        ///if (yes and) the block's center would pass the same colored block's center,
        ///snap the centers on the same hrzntl level
    }

    void Fall() {
        transform.Translate(0, -velocity * Time.deltaTime, 0);
        //if (block underneath) {bs = BlockState.Static}
    }


}
