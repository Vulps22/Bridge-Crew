using UnityEngine;
using TMPro;

public class KeypadManager : MonoBehaviour
{

    [SerializeField] private NetworkManager networkManager;
    public TextMeshProUGUI roomCodeText;
    public string roomCode = "";

    public void AddDigit(int digit)
    {

        Debug.Log("Adding digit: " + digit);
        if (roomCode.Length < 4)
        {
            roomCode += digit;
            roomCodeText.text = roomCode;
        }
    }

    public void ClearCode()
    {
        roomCode = "";
        roomCodeText.text = roomCode;
    }

    // This method will be called by your Join button
    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(roomCode))
        {
            networkManager.GoOnline(roomCode);
        }
        else
        {
            Debug.LogWarning("No room code entered!");
        }
    }

    /**
     * Called by the Main Menu's Quit button to close the game 
     **/
    public void QuitGame()
    {
        Application.Quit();
    }
}
