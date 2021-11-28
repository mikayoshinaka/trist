using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColorActState
{
    public abstract void EnterState(ColorAction colorAct);

    public abstract void UpdateState(ColorAction colorAct);

    public abstract void DrawGizmosState(ColorAction colorAct);
}
