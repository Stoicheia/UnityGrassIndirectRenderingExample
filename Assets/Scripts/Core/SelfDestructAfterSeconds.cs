using System;
using System.Collections;
using UnityEngine;

namespace Core
{
    public class SelfDestructAfterSeconds : MonoBehaviour
    {
        [field: SerializeField] public float Seconds { private get; set; }

        private void Start()
        {
            StartCoroutine(SelfDestructAfter(Seconds));
        }

        private IEnumerator SelfDestructAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }
    }
}