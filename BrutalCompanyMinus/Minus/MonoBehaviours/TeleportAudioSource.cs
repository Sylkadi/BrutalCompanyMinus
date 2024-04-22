using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.MonoBehaviours
{
    internal class TeleportAudioSource : MonoBehaviour
    {
        public float objectLifeTime = 5.0f;

        public AudioSource audioSource;

        private void Start()
        {
            audioSource.Play();
        }

        private void Update()
        {
            objectLifeTime -= Time.deltaTime;
            if(objectLifeTime <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
