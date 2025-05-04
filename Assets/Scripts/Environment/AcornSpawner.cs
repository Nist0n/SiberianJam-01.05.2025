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

        [SerializeField] private LayerMask layerMask;
        
        
        public List<int> acornCounts;
        
        private void Awake()
        {
            foreach (var prefab in prefabs)
            {
                int randomCount = Random.Range(0, 10);
                acornCounts.Add(randomCount);

                for (int j = 0; j < randomCount; j++)
                {
                    bool spawned = SpawnAcorn(prefab);
                    while (!spawned)
                    {
                        spawned = SpawnAcorn(prefab);
                    }
                }
            }
        }

        private bool SpawnAcorn(GameObject prefab)
        {
            Vector3 zonePos = zone.transform.position;
            
            Vector3 pos = new Vector3(
                zonePos.x + Random.Range(-randomPosDistribution, randomPosDistribution),
                zonePos.y,
                zonePos.z + Random.Range(-randomPosDistribution, randomPosDistribution)
            );

            if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit))
            {
                pos.y = hit.point.y + 0.05f;
            }
            else
            {
                return false;
            }
            
            if (Physics.CheckSphere(new Vector3(pos.x, pos.y + 0.4f, pos.z), acornRadius))
            {
                // Collider[] colliders = Physics.OverlapSphere(new Vector3(pos.x, pos.y + 0.3f, pos.z), acornRadius);
                // foreach (Collider coll in colliders)
                // {
                //     Debug.Log("colliding " + coll.gameObject.name);
                //     Debug.Log(coll.gameObject.transform.position);
                // }
                return false;
            }
                    
            Instantiate(prefab, pos, Quaternion.identity);
            return true;
        }
    }
}