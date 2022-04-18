﻿using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float WAYPOINT_GIZMO_RADIUS = 0.3f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            for (int i = 0; i < transform.childCount; i++)
            {
                var j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), WAYPOINT_GIZMO_RADIUS);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));

            }
        }

        public int GetNextIndex(int i)
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }

            return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
