using GameDevTV.Inventories;
using RPG.Abilities;
using RPG.Attibutes;
using RPG.Core;
using UnityEngine;

namespace Assets.Scripts.Abilities
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] TargetingStrategy targetingStrategy;
        [SerializeField] FilterStrategy[] filterStrategies;
        [SerializeField] EffectStrategy[] effectStrategies;
        [SerializeField] float cooldownTime = 0f;
        [SerializeField] float manaCost = 0f;

        public override void Use(GameObject user)
        {
            var mana = user.GetComponent<Mana>();
            if (mana.GetMana() < manaCost)
            {
                return;
            }

            var cooldownStore = user.GetComponent<CooldownStore>();
            if (cooldownStore.GetTimeRemaining(this) > 0) return;

            var data = new AbilityData(user);

            var scheduler = user.GetComponent<ActionScheduler>();
            scheduler.StartAction(data);

            targetingStrategy.StartTargeting(data,
                () =>
                {
                    TargetAquired(data); //dido: we are using lambda, so we can pass the data as argument.
                });
        }

        private void TargetAquired(AbilityData data)
        {
            if (data.IsCancelled()) return;

            var mana = data.GetUser().GetComponent<Mana>();
            if (!mana.UseMana(manaCost)) return;

            var cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, cooldownTime);

            foreach (var filterStrategy in filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }

            foreach (var effect in effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
        }

        private void EffectFinished()
        {

        }
    }
}
