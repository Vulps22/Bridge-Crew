using UnityEngine;
using System.Collections.Generic;

public class ChairManager : MonoBehaviour
{
    // List of spawn points (assign these via the Inspector)
    public List<Transform> chairSpawnPoints = new List<Transform>();

    // Track which chairs are occupied
    private bool[] isOccupied;

    void Start()
    {
        isOccupied = new bool[chairSpawnPoints.Count];
    }

    // Returns the transform of the first available chair spawn point
    public Transform GetAvailableSpawnPoint()
    {
        for (int i = 0; i < isOccupied.Length; i++)
        {
            if (!isOccupied[i])
            {
                isOccupied[i] = true;
                return chairSpawnPoints[i];
            }
        }
        // If all chairs are taken, just return the first one (or handle appropriately)
        return chairSpawnPoints[0];
    }

    // Release a chair when a player leaves
    public void ReleaseChair(int position)
    {
        if (position >= 0 && position < isOccupied.Length)
        {
            isOccupied[position] = false;
        }
    }

    public void ResetChairs()
    {
        isOccupied = null;
        isOccupied = new bool[chairSpawnPoints.Count];

        foreach (Transform chair in chairSpawnPoints)
        {
            Transform character = chair.GetChild(0);
            if(character.childCount > 0)
            {
                Destroy(character.GetChild(0).gameObject);
            }
        }

    }
}
