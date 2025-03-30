using UnityEngine;

/**
 * The player's Role in the game.
 */
public enum Role
{ 
    NONE,
    CAPTAIN,
    ENGINEER,
    PILOT,
    WEAPONS_OFFICER,
    OPS_OFFICER
}

public class PlayerManager: MonoBehaviour
{
    public int chairPosition;
    public Role role;
    public bool isHost;
}
