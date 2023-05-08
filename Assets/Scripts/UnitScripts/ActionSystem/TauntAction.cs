using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TauntAction : BaseAction
{
    List<Vector3> _pathVectorList = new List<Vector3>();

    public void Taunt()
    {
        if (!CanDoAction)
        {
            unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
            unit.SelectedAction.Value = UnitAction.Action.None;
            Debug.Log("Not Enough actions points");
            return;
        }


        unit.Threat.Value += 1000;

        unit.SpawnObjectClientRpc(unit.transform.position + new Vector3(-0.5f, 0.5f), 6);
        StartCoroutine(TauntEffect());
        
    }

    IEnumerator TauntEffect()
    {
        yield return new WaitForSeconds(0.3f);
        unit.ActionStatus.Value = BaseUnit.ActionState.Normal;
        unit.SelectedAction.Value = UnitAction.Action.None;
        UseActionPoints();
    }

    public void ShowMoveTiles()
    {
        int distance = 2 + (int)Mathf.Floor(unit.Stats.Value.Dexterity / 2);
        Vector3 position = unit.transform.position - new Vector3(0.5f, 0.5f);

        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                // Determinar si la posición es parte del diamante
                if (Mathf.Abs(i) + Mathf.Abs(j) <= distance)
                {
                    PlayerUnit currentPlayer = (PlayerUnit)unit;
                    int x = (int)position.x + i;
                    int y = (int)position.y + j;
                    if (x < 0 || y < 0 || x >= 18 || y >= 16)
                    {
                        continue;
                    }
                    List<Vector3> vectorList = Pathfinding.Instance.FindPathToNotWalkable(position, new Vector3(x, y));

                    if (vectorList != null && vectorList.Count > 1 && vectorList.Count - 1 <= distance)
                    {
                        currentPlayer.SetMapVisualTileActiveClientRpc(x, y, new Color(0.1f, 0.8f, 0.1f, 0.5f));
                    }
                }
            }
        }
    }
}
