using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Utils
{
    [Serializable]
    public class Condition
    {
        [SerializeField]
        Disjunction[] and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            foreach (var dis in and)
            {
                if (!dis.Check(evaluators)) return false;
            }

            return true;
        }

        [Serializable]
        class Disjunction
        {
            [SerializeField]
            Predicate[] or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var pred in or)
                {
                    if (pred.Check(evaluators)) return true;
                }

                return false;
            }
        }

        [Serializable]
        class Predicate
        {
            [SerializeField] string predicate;
            [SerializeField] string[] parameters;
            [SerializeField]
            bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                foreach (var evaluator in evaluators)
                {
                    var result = evaluator.Evaluate(predicate, parameters);
                    if (result == null) continue;

                    if (result == negate) return false;
                }

                return true;
            }
        }
    }
}
