using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        // dido: avoid jiggering in healthbar.
        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
