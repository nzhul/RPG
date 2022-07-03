using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue _dialogue = null;
        [SerializeField] string conversantName;

        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (_dialogue == null)
            {
                return false;
            }

            if (GetComponent<Health>().IsDead()) return false;
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, _dialogue);
            }

            return true;
        }

        public string GetName()
        {
            return conversantName;
        }
    }
}
