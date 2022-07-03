using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] UnityEvent<float> onTakeDamage;
        public UnityEvent onDie;

        // dido: use this subclas instead of UnityEvent<float> if the above do not work.
        //[Serializable]
        //public class TakeDamageEvent : UnityEvent<float>
        //{
        //}

        LazyValue<float> health;

        bool wasDeadLastFrame = false;

        private void Awake()
        {
            // dido: lazy value wrapper initialize the property when it is used, not on Start()
            health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            // dido: if until this point the health has not being initialized, we initialize it by force.
            health.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return health.value <= 0;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            //print(gameObject.name + " took damage: " + damage);

            // dido: this is not part of the course. 
            // I decided to show the actual damage being done on final hit.
            // So even if I do 50 damage. If the enemy have 10 hp left, i will do only 10 damage.
            // ^^ probably not a great idea to use this method in real game as it will look strange to the player.

            //var damageDealt = damage < health.value ? damage : (damage - health.value);

            health.value = Mathf.Max(health.value - damage, 0);

            if (IsDead())
            {
                onDie.Invoke();
                AwardExperience(instigator);
            }
            else
            {
                onTakeDamage.Invoke(damage);
            }

            UpdateState();
        }

        public void Heal(float healthToRestore)
        {
            health.value = Mathf.Min(health.value + healthToRestore, GetMaxHealthpoints());
            UpdateState();
        }


        public float GetHealthPoints()
        {
            return health.value;
        }

        public float GetMaxHealthpoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return health.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void RegenerateHealth()
        {
            //dido: this function regenerates us to maximum of 70%
            var regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerationPercentage / 100;
            health.value = Mathf.Max(health.value, regenHealthPoints);
        }

        private void UpdateState()
        {
            var animator = GetComponent<Animator>();
            var cc = GetComponent<CapsuleCollider>();

            if (!wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("die");
                GetComponent<ActionScheduler>().CancelCurrentAction();

                if (cc) cc.enabled = false;
            }

            if (wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
                if (cc) cc.enabled = true;
            }


            wasDeadLastFrame = IsDead();
        }

        private void AwardExperience(GameObject instigator)
        {
            var experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;
            UpdateState();
        }

    }
}
