using System.Collections.Generic;
using UnityEngine;

namespace EndlessRun
{
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private float spawnChance = 0.3f;
        [SerializeField] private int tileStartCount = 10;
        [SerializeField] private int minimumStraightTiles = 3;
        [SerializeField] private int maximumStraightTiles = 15;
        [SerializeField] private GameObject startingTile;
        [SerializeField] private List<GameObject> turnTiles;
        [SerializeField] private List<GameObject> obstacles;

        Vector3 currentTileLocation = Vector3.zero;
        Vector3 currentTileDirection = Vector3.forward;
        GameObject prevTile;
        List<GameObject> currentTiles;
        List<GameObject> currentObstacles;

        void Start()
        {
            currentTiles = new List<GameObject>();
            currentObstacles = new List<GameObject>();
            Random.InitState(System.DateTime.Now.Millisecond);

            for (int i = 0; i < tileStartCount; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>());
            }

            SpawnTile(selectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
            
        }

        void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            Quaternion newTileRotation = tile.gameObject.transform.rotation *
             Quaternion.LookRotation(currentTileDirection, Vector3.up);
            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);

            if (spawnObstacle) SpawnObstacle(); 

            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            }
        }

        void DeletePreviousTile()
        {
            while (currentTiles.Count != 1)
            {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
            }

            while (currentObstacles.Count != 0)
            {
                GameObject obstacle = currentObstacles[0];
                currentObstacles.RemoveAt(0);
                Destroy(obstacle);
            }
        }

        public void AddNewDirection(Vector3 direction)
        {
            currentTileDirection = direction;
            DeletePreviousTile();

            Vector3 tilePlacementScale;
            if (prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS)
            {
                tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 +
                 (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }
            else
            {
                tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) +
                 (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }

            currentTileLocation += tilePlacementScale;
            int currentPathLength = Random.Range(minimumStraightTiles, maximumStraightTiles);

            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true);
            }

            SpawnTile(selectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
        }

        void SpawnObstacle()
        {
            if (Random.value > spawnChance) return;

            GameObject obstaclePrefab = selectRandomGameObjectFromList(obstacles);
            Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation *
             Quaternion.LookRotation(currentTileDirection, Vector3.up);

            GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);
            currentObstacles.Add(obstacle);
        }

        GameObject selectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;

            return list[Random.Range(0, list.Count)];
        }
    }

}