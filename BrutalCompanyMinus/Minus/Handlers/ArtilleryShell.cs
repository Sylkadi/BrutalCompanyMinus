using HarmonyLib;
using System;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    internal class ArtilleryShell : MonoBehaviour
    {
        public Transform transform;

        public static float speed = 100.0f;

        private float timeTillExplode = 1.0f;

        public Vector3 target = Vector3.zero;

        private void Start()
        {
            transform.LookAt(target);
            timeTillExplode = Vector3.Distance(transform.position, target) / speed;
        }

        private void Update()
        {
            transform.position += transform.forward * Time.deltaTime * speed;
            if(timeTillExplode > 0)
            {
                timeTillExplode -= Time.deltaTime;
            } else
            {
                Landmine.SpawnExplosion(transform.position, true, 5, 6);
                Destroy(transform.gameObject);
            }
        }

        public static void FireAt(Vector3 at, Vector3 from) => Net.Instance.FireAtServerRpc(at, from);
    }
}
