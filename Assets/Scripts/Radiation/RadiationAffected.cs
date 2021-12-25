using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radiation
{
    public class RadiationAffected : MonoBehaviour
    {
        private Health health;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AffectByRadiation(float radiationLevel)
        {
            health.TakeDamage(radiationLevel);
            // Debug.LogFormat("SUPASUS, Affected by radiation with level of {0}", radiationLevel);
        }
    }
}