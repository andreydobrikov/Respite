///////////////////////////////////////////////////////////
// 
// EntityUtils.cs
//
// What it does: Utilities for entity-related behaviours
//
// Notes: 	
// 
// To-do:
//
///////////////////////////////////////////////////////////

#define DEBUG_SWEEP_RAYCASTS

using UnityEngine;
using System.Collections.Generic;

public class EntityUtils  
{
    // Sweep test a point with a radius and return the coverage
    // TODO: Make less awful.
    public static float SweepRadius(Vector3 origin, Vector3 target, float radius, int samples, int layermask)
    {
        float coverage = 0.0f;
        float sweepCount = (float)samples;

        Vector3 direction = target - origin;

        float distanceToTarget = (direction).magnitude;

        float angleToTarget = Mathf.Atan2(direction.x, direction.z);
        float targetOffset  = Mathf.Atan(radius / distanceToTarget);
        
        float sweepStart = angleToTarget - targetOffset;
        float sweepDelta = (targetOffset * 2.0f) / (sweepCount);

        float coverageDelta = 1.0f / sweepCount;
        RaycastHit hitInfo;
        
        for(int i = 0; i < samples; ++i)
        {
            float currentAngle = (sweepStart + (sweepDelta * i)) * Mathf.Rad2Deg;   
            Vector3 rayDirection = Quaternion.Euler(0.0f, currentAngle, 0.0f) * Vector3.forward;

            #if DEBUG_SWEEP_RAYCASTS
            Debug.DrawRay(origin, rayDirection, Color.magenta);
            #endif

            if(!Physics.Raycast(origin, rayDirection, out hitInfo, distanceToTarget, layermask))
            {
                coverage += coverageDelta;
            } 
        }
        return coverage;
    }
}
