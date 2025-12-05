using System.Collections;
using UnityEngine;

public class PrizeSpawner : MonoBehaviour
{
    [Header("Prize Prefabs (drag 5 here)")]
    public GameObject[] prizePrefabs;

    [Header("Game Box Bounds")]
    public Transform boxMin;   // bottom-left corner inside box
    public Transform boxMax;   // top-right corner inside box

    [Header("Grid Settings")]
    public int rows = 4;
    public int columns = 4;
    public float spawnDelay = 0.5f;

    private void Start()
    {
        StartCoroutine(SpawnGrid());
    }

    private IEnumerator SpawnGrid()
    {

        Vector3 min = boxMin.position;
        Vector3 max = boxMax.position;

        float xSpacing = (max.x - min.x) / (columns - 1);
        float zSpacing = (max.z - min.z) / (rows - 1);

        for (int r = 0; r < rows; r++)
        {
            
            for (int c = 0; c < columns; c++)
            {
                if((r==0 && c==0)||(r==0&&c==1))
                {
                    continue;
                }
                Vector3 spawnPos = new Vector3(
                    min.x + xSpacing * c,
                    min.y,             
                    min.z + zSpacing * r
                );

                SpawnRandomPrize(spawnPos);

                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private void SpawnRandomPrize(Vector3 pos)
    {
        int index = Random.Range(0, prizePrefabs.Length);
        Quaternion randomRot = Quaternion.Euler(
        Random.Range(0f, 360f),
        Random.Range(0f, 360f),
        Random.Range(0f, 360f)
        );
        Instantiate(prizePrefabs[index], pos, randomRot);
    }
}
