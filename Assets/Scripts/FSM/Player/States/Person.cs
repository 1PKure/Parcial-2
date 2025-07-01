using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Person : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void EnableControl();
    public abstract void DisableControl();
    public abstract Transform GetCameraTarget();
}
