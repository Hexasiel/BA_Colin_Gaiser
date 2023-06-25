using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyWave 
{
    public List<Spawn> spawns;
    public List<LightningSpawn> lightnings;
    public float m_duration = 20f;

    public EnemyWave(float difficulty){
        GenerateWave(difficulty);
    }

    void GenerateWave(float difficulty){
        spawns = new List<Spawn>();
        lightnings= new List<LightningSpawn>();

        bool side = Random.Range(0, 2) == 1;

        if ( difficulty > 0) {
           int countSmallEnemies = (int)(difficulty / 0.2f);
            Debug.Log("Spawn " + countSmallEnemies + "Small Enemies");
           for (int i = 0; i < countSmallEnemies; i++) {
                float randomTime = Random.Range(0, m_duration);
                spawns.Add(new Spawn(EnemyType.Small, randomTime, RandomSide(side, difficulty)));
           }
        }
        if (difficulty >= 1) {
            int countSiegeEnemies = (int)(difficulty / 0.5f);
            for (int i = 0; i < countSiegeEnemies; i++) {
                float randomTime = Random.Range(0, m_duration);
                spawns.Add(new Spawn(EnemyType.Siege, randomTime, RandomSide(side, difficulty)));
            }
        }
        if (difficulty >= 2) {
            int countLightnings = (int)(difficulty / 0.05f);
            for (int i = 0; i < countLightnings; i++) {
                float randomTime = Random.Range(0, m_duration);
                lightnings.Add(new LightningSpawn(RandomGroundPos(-155f, 155f), randomTime));
            }
        }
        if (difficulty >= 3) {
            int countBigEnemies = (int)(difficulty / 1.5f);
            for (int i = 0; i < countBigEnemies; i++) {
                float randomTime = Random.Range(0, m_duration);
                spawns.Add(new Spawn(EnemyType.Big, randomTime, RandomSide(side, difficulty)));
            }
        }
        if (difficulty >= 5) {
            int countFlyingEnemies = (int)(difficulty / 1.5f);
            for (int i = 0; i < countFlyingEnemies; i++) {
                float randomTime = Random.Range(0, m_duration);
                spawns.Add(new Spawn(EnemyType.Flying, randomTime, RandomSide(side, difficulty)));
            }
        }
    }

    bool RandomSide(bool baseSide, float difficulty) {
        if(difficulty < 4f) {
            return baseSide;
        }
        else {
            return Random.Range(0, 2) == 1;
        }

    }

    Vector3 RandomGroundPos(float minXValue, float maxXValue) {
        Vector3 randomPos = new Vector3();
        randomPos.x = Random.Range(minXValue, maxXValue);
        randomPos.y = 100;
        LayerMask mask = LayerMask.GetMask("Terrain");

        RaycastHit2D hit = Physics2D.Raycast(randomPos, Vector2.down, 200, mask);

        if (hit.collider != null) {
            randomPos.y = hit.point.y;
        }
        return randomPos;
    }


    //Types
    //---------------------------------------------------------------------------------------------

    public struct Spawn {
        public EnemyType m_type;
        public float m_time;
        public bool m_side;

        public Spawn(EnemyType type, float time, bool side) {
            m_type = type;
            m_time = time;
            m_side = side;
        }
    }

    public struct LightningSpawn {
        public Vector3 m_position;
        public float m_time;
        public LightningSpawn(Vector3 position, float time) {
            m_position = position;
            m_time = time;
        }
    }

    public enum EnemyType {
        Small, Big, Flying, Siege
    }
}
