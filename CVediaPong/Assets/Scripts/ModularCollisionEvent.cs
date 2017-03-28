using UnityEngine;
using System.Collections;

// A modular class meant to able to be thown on any object with a collider and
// trigger events on the collision.
public class ModularCollisionEvent : MonoBehaviour {

    // The Event to be triggered
    public EventDelegate Event;
    // GameObject To Expect the collision with.
    public GameObject ExpectedCollisionObject;

    // This event calls when this gameobject collides with the expected rigidbody then calls the EventDelegate
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
