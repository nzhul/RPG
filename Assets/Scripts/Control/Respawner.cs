using Cinemachine;
using RGP.Control;
using RPG.Attributes;
using RPG.SceneManagement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Transform _respawnLocation;
        [SerializeField] float _respawnDelay = 2;
        [SerializeField] float _fadeTime = 0.5f;
        [SerializeField] float _healthRegenPercentage = 20;
        [SerializeField] float _enemyHealthRegenPercentage = 20;

        private void Awake()
        {
            GetComponent<Health>().onDie.AddListener(Respawn);
        }

        private void Start()
        {
            if (GetComponent<Health>().IsDead())
            {
                Respawn();
            }

        }

        private void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            yield return new WaitForSeconds(_respawnDelay);
            var fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeTime);
            RespawnPlayer();
            ResetEnemies();
            savingWrapper.Save();
            yield return fader.FadeIn(_fadeTime);

        }

        private void ResetEnemies()
        {
            foreach (var enemyController in FindObjectsOfType<AIController>())
            {
                var health = enemyController.GetComponent<Health>();
                if (health && !health.IsDead())
                {
                    enemyController.Reset();
                    health.Heal(health.GetMaxHealthpoints() * _enemyHealthRegenPercentage / 100);
                }
            }
        }

        private void RespawnPlayer()
        {
            var positionDelta = _respawnLocation.position - transform.position;

            GetComponent<NavMeshAgent>().Warp(_respawnLocation.position);
            var health = GetComponent<Health>();
            health.Heal(health.GetMaxHealthpoints() * _healthRegenPercentage / 100);
            var activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if (activeVirtualCamera.Follow == transform)
            {
                activeVirtualCamera.OnTargetObjectWarped(transform, positionDelta);
            }

        }
    }
}
