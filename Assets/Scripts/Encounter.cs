﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour {

    public struct EncounterJSON
    {
        public string[] enemies;
        public int[] counts;
    }

    public Transform trianglePoint1;
    public Transform trianglePoint2;
    public Transform trianglePoint3;

    public int floorNumber;

    private EncounterJSON m_encounter;

    public GameObject testSpehre;

	// Use this for initialization
	void Start () {
        int testsize = 5;
        m_encounter.enemies = new string[testsize];
        m_encounter.counts = new int[testsize];
        string debugInfo = "Set\t\tCount\n";
        for (int i = 0; i < testsize; i++)
        {
            m_encounter.enemies[i] = "Test";
            int count = Random.Range(1, 10);
            m_encounter.counts[i] = count;
            debugInfo = i + "\t\t" + count + "\n";
        }
        print(debugInfo);
        SpawnEncounter();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //Attempt to spawn in a triangle patter where the backside is unused, in a circular pattern
    //index 0 would be in the middle, while the last index would be the outer edge
    void SpawnEncounter()
    {
        //define triangle using the 3 points, where 1 points to player
        //always favour middle for spawns
        //back wall is defined by the corners 2 and 3

        Vector3 middle = (trianglePoint1.position + trianglePoint2.position + trianglePoint3.position) / 3;
        //float sin60 = Mathf.Sin(60 * Mathf.Deg2Rad);
        Vector3[] prevSet = new Vector3[3];
        Vector3[] pointNormals = new Vector3[3];
        for (int i = 0; i < 3; i++)
            prevSet[i] = middle;
        pointNormals[0] = (trianglePoint1.position - middle).normalized;
        pointNormals[1] = (trianglePoint2.position - middle).normalized;
        pointNormals[2] = (trianglePoint3.position - middle).normalized;
        float prevRadius = 0;


        for (int i = 0; i < m_encounter.counts.Length; i++)
        {
            string debugString = "";
            int count = m_encounter.counts[i];
            debugString += "Detected " + count + " enemies to create.\n";
            float radius = /*EnemyByName(m_encounter.enemies[i]).m_stats.radius*/ 1f;

            float triangleLength = (prevSet[0] - prevSet[1]).magnitude;
            debugString += "Previous triangle length: " + triangleLength + "\n";
            int perSideCount = Mathf.CeilToInt(count / 2);
            float halfCount = count / 2;

            //check if we need more space
            //radii required appears to be FloorToInt(count / 2) * 4
            float requiredLength = Mathf.FloorToInt(halfCount) * 4 * radius;
            debugString += "Required triangle length: " + requiredLength + "\n";
            if (triangleLength < requiredLength)
            {
                float halfoffsetsqaured = Mathf.Pow((requiredLength - triangleLength)/2, 2f);
                //calculate the required pointnormal scale
                float scalar = Mathf.Sqrt(halfoffsetsqaured * 2); //c^2 = a^2 + b^2 where a == b
                //scale the triangle
                for (int j = 0; j < 3; j++)
                    prevSet[j] += pointNormals[j] * scalar;

                debugString += "New triangle length: " + (prevSet[0] - prevSet[1]).magnitude + "\n";
            }

            if (count % 2 == 0)//even
            {
                float spawnGap = requiredLength / (perSideCount + 1);
                Vector3 leftSide = (prevSet[1] - prevSet[0]).normalized;
                Vector3 rightSide = (prevSet[2] - prevSet[0]).normalized;
                for (int j = 1; j <= perSideCount; j++)
                {
                    debugString += "Spawning enemy using offset: " + spawnGap * j + "\n";
                    //spawn on both sides
                    SpawnEnemy(m_encounter.enemies[i], prevSet[0] + leftSide * (spawnGap * j));
                    SpawnEnemy(m_encounter.enemies[i], prevSet[0] + rightSide * (spawnGap * j));
                }
            }
            else//odd
            {
                SpawnEnemy(m_encounter.enemies[i], prevSet[0]);
                if (count == 1)//single enemy
                {
                    debugString += "Only one enemy, were done this set.\n";
                    print(debugString);
                    continue;
                }
                float spawnGap = requiredLength / (perSideCount - 1);
                //calculate triange sides
                Vector3 leftSide = (prevSet[1] - prevSet[0]).normalized;
                Vector3 rightSide = (prevSet[2] - prevSet[0]).normalized;
                debugString += "It's odd so it behaves oddly, " + perSideCount + " per side including front.\n";
                for (int j = 1; j <= perSideCount; j++)
                {
                    //spawn on both sides
                    debugString += "Spawning enemy using offset: " + spawnGap * j + "\n";
                    SpawnEnemy(m_encounter.enemies[i], prevSet[0] + leftSide * (spawnGap * j));
                    SpawnEnemy(m_encounter.enemies[i], prevSet[0] + rightSide * (spawnGap * j));
                }
                

            }

            print(debugString);

            //increment prevSet triangle
            for (int j = 0; j < 3; j++)
                prevSet[j] += pointNormals[j] * radius;
            prevRadius = radius;
        }
    }

    Enemy EnemyByName(string e)
    {
        Enemy enemy = new Enemy();
        enemy.m_stats.radius = 1;
        return enemy;
    }

    void SpawnEnemy(string e, Vector3 p)
    {
        Instantiate(testSpehre, p, Quaternion.identity);
    }
}
