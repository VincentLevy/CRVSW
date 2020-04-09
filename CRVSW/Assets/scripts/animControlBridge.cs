using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//serves to create a bridge between the sprite's animation and the prefab's animation
// if any non-me person ever sees this, I don't really know what I am doing
public class animControlBridge : MonoBehaviour
{
    public animControl anim;

    public void SetIsAttacking(bool value)
    {
        anim.SetIsAttacking(value);
    }
}
