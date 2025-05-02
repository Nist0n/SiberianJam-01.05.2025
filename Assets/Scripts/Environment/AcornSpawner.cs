using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class AcornSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject zone;

        [SerializeField] private List<GameObject> prefabs;
        
        [SerializeField] private float randomPosDistribution = 3;

        [SerializeField] private float acornRadius = 0.1f;

        public List<int> acornCounts;
        
        private void Awake()
        {
            Vector3 zonePos = zone.transform.position;
            
            foreach (var prefab in prefabs)
            {
                int randomCount = Random.Range(0, 10);
                acornCounts.Add(randomCount);

                for (int j = 0; j < randomCount; j++)
                {
                    Debug.Log(prefab.name);
                    
                    Vector3 pos = new Vector3(
                        zonePos.x + Random.Range(-randomPosDistribution, randomPosDistribution),
                        zonePos.y,
                        zonePos.z + Random.Range(-randomPosDistribution, randomPosDistribution)
                    );

                    if (Physics.CheckSphere(pos, acornRadius))
                    {
                        pos = new Vector3(
                            zonePos.x + Random.Range(-randomPosDistribution, randomPosDistribution),
                            zonePos.y,
                            zonePos.z + Random.Range(-randomPosDistribution, randomPosDistribution)
                        );
                    }
                    
                    Instantiate(prefab, pos, Quaternion.identity);
                }
            }
        }
    }
}