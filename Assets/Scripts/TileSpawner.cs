using System.Collections.Generic;
using UnityEngine;

namespace EndlessRun
{
    public class TileSpawner : MonoBehaviour
    {

        [SerializeField] int tileStartCount = 10;
        [SerializeField] int minimumStraightTiles = 3;
        [SerializeField] int maximumStraightTiles = 15;
        [SerializeField] GameObject startingTile;
        [SerializeField] List<GameObject> turnTiles;
        [SerializeField] List<GameObject> obstacles;

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

            //SpawnTile(selectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());
            SpawnTile(turnTiles[0].GetComponent<Tile>());
            AddNewDirection(Vector3.left);
        }

        void SpawnTile(Tile tile, bool spawnObstacle = false)
        {
            Quaternion newTileRotation = tile.gameObject.transform.rotation *
             Quaternion.LookRotation(currentTileDirection, Vector3.up);
            prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation);
            currentTiles.Add(prevTile);
            if (tile.type == TileType.STRAIGHT)
            {
                currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection);
            }
        }

        void DeletePreviousTile() {
            while (currentTiles.Count != 1) {
                GameObject tile = currentTiles[0];
                currentTiles.RemoveAt(0);
                Destroy(tile);
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
            else {
                tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) +
                 (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
            }
            
            currentTileLocation += tilePlacementScale;
            int currentPathLength = Random.Range(minimumStraightTiles,maximumStraightTiles);
            
            for (int i = 0; i < currentPathLength; i++)
            {
                SpawnTile(startingTile.GetComponent<Tile>(), (i==0) ? false : true);    
            }

            SpawnTile(selectRandomGameObjectFromList(turnTiles).GetComponent<Tile>());


        }

        GameObject selectRandomGameObjectFromList(List<GameObject> list)
        {
            if (list.Count == 0) return null;

            return list[Random.Range(0, list.Count)];
        }
    }

}