using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    EntitySpawner entitySpawner;
    Player player;

    void Awake()
    {
    }

	void Start ()
    {
        GameObject npcSpawnerfab = Resources.Load("EntitySpawner") as GameObject;
        GameObject npcSpanwerObj = Instantiate(npcSpawnerfab, Vector3.zero, Quaternion.identity);
        npcSpanwerObj.name = npcSpawnerfab.name;
        npcSpanwerObj.transform.parent = gameObject.transform;
        entitySpawner = npcSpanwerObj.GetComponent<EntitySpawner>();
        entitySpawner.Init();

        entitySpawner.StartSpawn(0, 1f);

        player = FindObjectOfType<Player>();
        MotionPath path = FindObjectOfType<MotionPath>();
        if (player && path)
        {
            player.StartGame(path);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
