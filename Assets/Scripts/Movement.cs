using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Movement : NetworkBehaviour
{
    public Action onPositionReached;
    public Action notReachable;

    List<Vector3> pathVectorList;
    int currentPathIndex;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0,0) + new Vector3(1f, 1f) * .5f;
    }

    private void Update()
    {
        
    }

    public void HandleMovement()
    {
        if (pathVectorList != null)
        {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if(Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
                {
                    transform.position = targetPosition;

                }
            }
            else
            {
                currentPathIndex++;
                if(currentPathIndex >= pathVectorList.Count)
                {
                    pathVectorList = null;
                    onPositionReached?.Invoke();
                }
            }
            
        } else
        {
            notReachable?.Invoke();
        }
    }

    public void SetTargetPosition(Vector3 targetPosition, Action onPositionReached, Action notReachable)
    {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);
        if(pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
            this.onPositionReached = onPositionReached;
        }
        else
        {
            this.notReachable = notReachable;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
