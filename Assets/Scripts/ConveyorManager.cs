using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public List<GameObject> easyChunkList;
    public List<GameObject> mediumChunkList;
    public List<GameObject> hardChunkList;
    public GameObject startingChunk;
    public int chunkCountMax = 10;
    public int chunksToLevelUp = 10;
    public int easyDifficulty = 10;
    public int mediumDifficulty = 20;
    public int hardDifficulty = 30;
    public float chunkMovementSpeed = 10f;

    [SerializeField]
    private int level = 0;

    [SerializeField]
    private List<GameObject> activeChunkList;

    private int chunksDestroyed;
    private bool hasLoadedChunks = false;
    private bool hasPositionedChunks = false;
    private GameObject lastPositionedChunk = null;
    private Vector3 movement;

    // Start is called before the first frame update
    void Start()
    {
        LoadChunks();
        PositionChunks();

        movement = new Vector3(0, 0, Time.deltaTime * chunkMovementSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveConveyor();
    }

    // When a chunk hits this trigger, destroy the chunk and handle level incrementation.
    // This is the code that decides when the player reaches a new level.
    // Default is 10 chunks destroyed = level up. Also increases the chunk movement speed by 5%.
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Chunk")
        {
            activeChunkList.Remove(other.gameObject);
            Destroy(other.gameObject);
            chunksDestroyed++;

            GameObject chunk = null;

            if(level <= easyDifficulty)
            {
                chunk = Instantiate(easyChunkList[Random.Range(0, easyChunkList.Count)], new Vector3(-1000, -500, -1000), startingChunk.transform.rotation);
                Debug.Log("Spawned easy chunk.");
            }
            else if(level > easyDifficulty && level <= mediumDifficulty)
            {
                chunk = Instantiate(mediumChunkList[Random.Range(0, mediumChunkList.Count)], new Vector3(-1000, -500, -1000), startingChunk.transform.rotation);
                Debug.Log("Spawned medium chunk.");
            }
            else if(level > mediumDifficulty)
            {
                chunk = Instantiate(hardChunkList[Random.Range(0, hardChunkList.Count)], new Vector3(-1000, -500, -1000), startingChunk.transform.rotation);
                Debug.Log("Spawned hard chunk.");
            }

            GameObject lpcEndPoint = null;
            GameObject chunkStartPoint = null;

            foreach (Transform t in lastPositionedChunk.transform)
            {
                if (t.tag == "End Point")
                {
                    lpcEndPoint = t.gameObject;
                }
            }

            foreach (Transform tr in chunk.transform)
            {
                if (tr.tag == "Start Point")
                {
                    chunkStartPoint = tr.gameObject;
                }
            }

            if (lpcEndPoint != null && chunkStartPoint != null)
            {
                lpcEndPoint.transform.parent = null;
                chunkStartPoint.transform.parent = null;

                lastPositionedChunk.transform.parent = lpcEndPoint.transform;
                chunk.transform.parent = chunkStartPoint.transform;

                chunkStartPoint.transform.position = lpcEndPoint.transform.position;

                lastPositionedChunk.transform.parent = null;
                chunk.transform.parent = null;

                lpcEndPoint.transform.parent = lastPositionedChunk.transform;
                chunkStartPoint.transform.parent = chunk.transform;

                lastPositionedChunk = chunk;
            }

            lastPositionedChunk = chunk;
            activeChunkList.Add(chunk);
        }

        if (chunksDestroyed >= chunksToLevelUp)
        {
            {
                level++;
                chunksDestroyed = 0;
                chunkMovementSpeed *= 1.05f;
                Debug.Log("Level up!");
            }
        }
    }

    /// <summary>
    /// Handles the logic to spawn all of the starting chunks in before the game begins and before they start moving.
    /// </summary>
    public void LoadChunks()
    {
        for (int i = 0; i < chunkCountMax; i++)
        {
            GameObject chunk;

            chunk = Instantiate(easyChunkList[Random.Range(0, easyChunkList.Count)], startingChunk.transform.position, startingChunk.transform.rotation);

            activeChunkList.Add(chunk);
        }

        hasLoadedChunks = true;
    }

    /// <summary>
    /// Positions each chunk one after the other in a line using the start and end points on each chunk.
    /// </summary>
    public void PositionChunks()
    {
        //GameObject lastPositionedChunk = null;

        foreach (GameObject chunk in activeChunkList)
        {
            if(lastPositionedChunk != null)
            {
                GameObject lpcEndPoint = null;
                GameObject chunkStartPoint = null;

                foreach (Transform t in lastPositionedChunk.transform)
                {
                    if(t.tag == "End Point")
                    {
                        lpcEndPoint = t.gameObject;
                    }
                }

                foreach (Transform tr in chunk.transform)
                {
                    if(tr.tag == "Start Point")
                    {
                        chunkStartPoint = tr.gameObject;
                    }
                }

                if(lpcEndPoint != null && chunkStartPoint != null)
                {
                    lpcEndPoint.transform.parent = null;
                    chunkStartPoint.transform.parent = null;

                    lastPositionedChunk.transform.parent = lpcEndPoint.transform;
                    chunk.transform.parent = chunkStartPoint.transform;

                    chunkStartPoint.transform.position = lpcEndPoint.transform.position;

                    lastPositionedChunk.transform.parent = null;
                    chunk.transform.parent = null;

                    lpcEndPoint.transform.parent = lastPositionedChunk.transform;
                    chunkStartPoint.transform.parent = chunk.transform;

                    lastPositionedChunk = chunk;
                }
            }
            else if(lastPositionedChunk == null)
            {
                chunk.transform.position = startingChunk.transform.position;
                lastPositionedChunk = chunk;
            }
        }

        hasPositionedChunks = true;
    }


    public void MoveConveyor()
    {
        if(hasLoadedChunks && hasPositionedChunks)
        {
            foreach (GameObject chunk in activeChunkList)
            {
                chunk.transform.Translate(-movement, Space.World);
            }
        }
    }
}
