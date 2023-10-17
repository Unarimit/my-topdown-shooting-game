using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class BulletController : MonoBehaviour
    {
        public GameObject ParticalSystem;

        [HideInInspector]
        public Vector3 InitiatePos;
        private void Awake()
        {
            InitiatePos = transform.position;
            StartCoroutine(StopPartical());
        }
        private void FixedUpdate()
        {
        }
        private void OnCollisionEnter(Collision collision)
        {
            //Destroy(gameObject);
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            StartCoroutine(ClearBullet());
        }

        private IEnumerator StopPartical()
        {
            for(int i = 0; i < 2; i++)
            {
                if(i == 0) yield return new WaitForSeconds(0.3f);
                if(i == 1)
                {
                    var temp = ParticalSystem.GetComponent<ParticleSystem>().emission;
                    temp.enabled = false;
                    yield return null;
                }
            }
        }
        private IEnumerator ClearBullet()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0) yield return new WaitForSeconds(2f);
                if (i == 1)
                {
                    Destroy(gameObject);
                    yield return null;
                }
            }
        }
    }
}
