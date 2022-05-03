using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        /// <summary>
        /// Example: Weapon damage = +20 to physical damage.
        /// Example2: Amulet gives +50 physical damage.
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        IEnumerable<float> GetAdditiveModifiers(Stat stat);

        /// <summary>
        /// Example: Weapon/Amulet gives +10% to physical damage.
        /// </summary>
        /// <param name="stat">Physical Damage</param>
        /// <returns></returns>
        IEnumerable<float> GetPercentageModifiers(Stat stat);
    }
}
