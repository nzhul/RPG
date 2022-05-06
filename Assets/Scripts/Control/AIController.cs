using RPG.Attributes;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using RPG.Utils;
using UnityEngine;

namespace RGP.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;

        [Range(0f, 1f)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Fighter _fighter;
        Health _health;
        GameObject _player;
        Mover _mover;
        LazyValue<Vector3> _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int _currentWaypointIndex = 0;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _player = GameObject.FindWithTag("Player");

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead())
            {
                return;
            }

            if (InAttackRange() && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {

            var nextPosition = _guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, patrolSpeedFraction);

            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            var distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);
        }

        private bool InAttackRange()
        {
            var distance = Vector3.Distance(transform.position, _player.transform.position);
            return distance < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, chaseDistance);
#endif

            // Alternative.
            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
