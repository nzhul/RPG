using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField]
        Transform _target;

        private void LateUpdate()
        {
            transform.position = _target.position;
        }
    }

}