using UnityEngine;
using System.Collections;

/// <summary>
/// A modular class meant to able to be thown on any object with a collider and
/// trigger events on the collision.
/// </summary>
public class ModularCollisionEvent : MonoBehaviour {

    /// <summary> The Event to be triggered. </summary>
    public EventDelegate Event;
    /// <summary> GameObject To Expect the collision with. </summary>
    public GameObject ExpectedCollisionObject;

    /// <summary> This event calls when this gameobject collides with the expected rigidbody then calls the EventDelegate. </summary>
    ///
    /// <param name="col">  The col. </param>
    void OnCollisionEnter(Collision col)
    {
        if (ExpectedCollisionObject == null)
            return;
        if (col.gameObject == ExpectedCollisionObject)
        {
            if (Event != null)
                Event.Execute();
        }
    }

}
