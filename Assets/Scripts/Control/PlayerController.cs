using RPG.Attributes;
using RPG.Movement;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    // DIDO:
    // IgnoreRayCastLayer! > I've set the player prefab to be on IgnoreRayCast Layer to avoid problems with movement,
    // when clicking on top of player
    // This might cause problems in future - keep that in mind!
    public class PlayerController : MonoBehaviour
    {
        private Health _health;

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector3 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI())
            {
                return;
            }

            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent())
            {
                return;
            }

            if (InteractWithMovement())
            {
                return;
            }

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            var hits = RaycastAllSorted();
            foreach (var hit in hits)
            {
                var raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            var hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);

            var distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }

                SetCursor(CursorType.Movement);

                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = Vector3.zero;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (!hasHit) return false;

            // Find nearest navmesh point
            NavMeshHit navMeshHit;
            var hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point,
                out navMeshHit,
                maxNavMeshProjectionDistance,
                NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            var mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}