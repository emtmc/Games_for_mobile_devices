using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    public abstract void onSelect();
    public abstract void onDeselect();
    public abstract void scale();
    public abstract void move();
    public abstract void rotateObj();
    public abstract void ScaleorRotate();
}

