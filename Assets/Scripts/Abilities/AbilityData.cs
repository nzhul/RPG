﻿using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        GameObject user;
        Vector3 targetedPoint;
        public IEnumerable<GameObject> targets;
        bool cancelled = false;

        public AbilityData(GameObject user)
        {
            this.user = user;
        }

        public IEnumerable<GameObject> GetTargets()
        {
            return targets;
        }

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            this.targets = targets;
        }

        public Vector3 GetTargetedPoint()
        {
            return targetedPoint;
        }

        public void SetTargetedPoint(Vector3 targetedPoint)
        {
            this.targetedPoint = targetedPoint;
        }

        public GameObject GetUser()
        {
            return user;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            cancelled = true;
        }

        public bool IsCancelled()
        {
            return cancelled;
        }
    }
}
