using Assets.Scripts.AfterWin;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.System.Wave
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager instance;


        public WaveData waveData;

        private Tilemap tilemap;
        private Timer timer;

        private List<Vector2Int> spawnTiles;
        private List<Vector2Int> spawnTilesClone;

        private int nextWave;
        private int bossDefeated = 0;
        private bool stopped = false;

        private void Awake()
        {
            tilemap = GetComponent<Tilemap>();
            timer = GetComponentInChildren<Timer>();
        }

        private void Start()
        {
            spawnTiles = GenerateRandomPos();
            spawnTilesClone = new List<Vector2Int>();
            instance = this;
        }

        private void Update()
        {
            if (!stopped && nextWave > 0)
            {
                bool tooLate = timer.currentSecond < 1;
                bool boardCleared = transform.childCount < 2;

                if (tooLate)
                    SpawnNextWave(15);
                else if (boardCleared)
                    SpawnNextWave(10);
            }
        }

        private void SpawnNextWave(int addSeconds)
        {
            if (nextWave > waveData.waves.Length)
                nextWave = 1;
            spawnTilesClone.Clear();
            spawnTilesClone.AddRange(spawnTiles);

            foreach (Mob mobType in waveData.waves[nextWave - 1].mobs)
            {
                for (int i = 0; i < mobType.amount; i++)
                {
                    Vector2Int randomPos = spawnTilesClone[Random.Range(0, spawnTilesClone.Count)];
                    PlaceMob(mobType.mob, randomPos);
                    spawnTilesClone.Remove(randomPos);
                }
            }

            if (nextWave == 15)
                timer.AddTime(5);
            else
                timer.AddTime(addSeconds);

            nextWave++;
        }

        public void StartSpawn()
        {
            nextWave = 1;
            SpawnNextWave(30);
        }

        private List<Vector2Int> GenerateRandomPos()
        {
            List<Vector2Int> randomPos = new List<Vector2Int>();

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (tilemap.HasTile((Vector3Int)pos))
                        randomPos.Add(pos);
                }
            }

            return randomPos;
        }

        private void PlaceMob(GameObject mobPrefab, Vector2Int randomPos)
        {
            Instantiate(mobPrefab, (Vector2)randomPos, Quaternion.identity, transform);
        }

        public void StopHesAlreadyDead()
        {
            gameObject.SetActive(false);
        }

        public void DefeatedABoss()
        {
            bossDefeated++;
            if (bossDefeated >= 2)
            {
                Victory();
                stopped = true;
            }
        }

        private void Victory()
        {
            InstructionText.instance.Victory();


            foreach (var mob in GetComponentsInChildren<MobController>())
            {
                if (!mob.disabledDie)
                    mob.Die();
            }

            GameObject.FindWithTag("Gate").GetComponent<Gate>().Open();
        }
    }
}