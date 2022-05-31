using Assets.Scripts.Combat;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] Fighter[] fighters;
        [SerializeField] bool activateOnStart = false;

        private void Start()
        {
            Activate(activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (var fighter in fighters)
            {
                var target = fighter.GetComponent<CombatTarget>();

                if (target != null)
                {
                    target.enabled = shouldActivate;
                }

                fighter.enabled = shouldActivate;
            }
        }
    }

}