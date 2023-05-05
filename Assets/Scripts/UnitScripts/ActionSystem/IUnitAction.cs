using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction
{
    void Setup(BaseUnit unit);
    BaseUnit GetUnit();
    bool UseActionPoints();
}
