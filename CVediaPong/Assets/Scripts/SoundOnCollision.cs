using UnityEngine;
using System.Collections;

public class SoundOnCollision : MonoBehaviour {

    // Sound to be played
    public AudioClip Clip;

    // Plays a sound with the NGUI.PlaySound helper function
    void OnCollisionEnter(Collision col)
    {
        if (Clip == null)
            return;
        NGUITools.PlaySound(Clip);
    }

}
