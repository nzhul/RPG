using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Abilities/Filters/Tag", order = 0)]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = string.Empty;

        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach (var obj in objectsToFilter)
            {
                if (obj.CompareTag(tagToFilter))
                {
                    yield return obj;
                }
            }
        }
    }
}
