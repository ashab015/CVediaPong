using UnityEngine;
using System.Collections;

/// <summary> A sound on collision. </summary>
public class SoundOnCollision : MonoBehaviour {

    /// <summary> Sound to be played. </summary>
    public AudioClip Clip;

    /// <summary> Plays a sound with the NGUI.PlaySound helper function. </summary>
    ///
    /// <param name="col">  The col. </param>
    void OnCollisionEnter(Collision col)
    {
        if (Clip == null)
            return;
        NGUITools.PlaySound(Clip);
    }

}
