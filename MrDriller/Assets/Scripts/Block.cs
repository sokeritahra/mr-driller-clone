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

public class Block : MonoBehaviour {

    public float velocity;
    public bool falling;
    //tila: staattinen vai tippumassa?
    public int gridPos;
    // paikka taulukossa?
    //ja sit onko blokki perus, X vai AIR

	void Start () {
		
	}
	
	void Update () {
        // if block underneath destroyed, falling = true

        ///if falling = true start with hold & wobble, then fall with velocity, 
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


}
