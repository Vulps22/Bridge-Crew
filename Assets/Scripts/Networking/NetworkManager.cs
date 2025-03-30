using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public enum ConnectionStatus
{
    Offline,
    Connected,
    Connecting
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public ChairManager chairManager; // Will reference the ChairManager script
    public Transform XROrigin;         // Will reference the XROrigin
    public Transform vrHead;           // Will reference Main Camera
    public Transform vrLeftController; // Will reference Left Controller
    public Transform vrRightController; // Will reference Right Controller

    public TextMeshProUGUI RoomCodeText; // Will reference the RoomCodeText object in the Menu
    public TextMeshProUGUI OfflineText; // Will reference the OfflineText object in the Menu
    public TextMeshProUGUI ConnectedText;// Will reference the ConnectedText object in the Menu
    public TextMeshProUGUI ConnectingText; // Will reference the ConnectingText object in the Menu

    private string characterModelName = "Avatar_Simple"; // The name of the character model prefab
    private bool isGoingOnline = false; // Flag to indicate if the game is transitioning from offline to online
    private string pendingRoomCode = ""; // Room code to join after going online

    void Start()
    {  
        // Enable offline mode for testing without network connectivity
        PhotonNetwork.OfflineMode = true;
        setStatusMessage(ConnectionStatus.Offline);

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("11: OnConnectedToMaster");
        if (PhotonNetwork.OfflineMode && !isGoingOnline)
        {
            Debug.Log("12: Offline mode still enabled and not transitioning online. Creating mainMenu room...");
            setStatusMessage(ConnectionStatus.Offline);
            PhotonNetwork.JoinOrCreateRoom("mainMenu", new RoomOptions { MaxPlayers = 1 }, TypedLobby.Default);
            Debug.Log("13: Joined or created mainMenu room.");
        }
        else if (isGoingOnline)
        {
            Debug.Log("14: isGoingOnline is true. Transitioned to online mode.");
            isGoingOnline = false;
            Debug.Log("15: isGoingOnline reset to false.");
            if (!string.IsNullOrEmpty(pendingRoomCode))
            {
                Debug.Log("16: Pending room code found: " + pendingRoomCode + ". Joining room...");
                JoinRoom(pendingRoomCode);
            }
            else
            {
                Debug.Log("17: No pending room code. Hosting a new room...");
                HostRoom();
            }
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected: " + cause);

        //destroy the player avatar and remove them from the chair
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerManager manager = player.GetComponent<PlayerManager>();

            chairManager.ReleaseChair(manager.chairPosition);

            PhotonNetwork.Destroy(player);
        }

        setStatusMessage(ConnectionStatus.Offline);
        if(isGoingOnline)
        {
            Debug.Log("Attempting to reconnect...");
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // Call this method (e.g., from a UI button) to host a room.
    public void HostRoom()
    {

        Debug.Log("18: HostRoom called.");
        setStatusMessage(ConnectionStatus.Connecting);
        Debug.Log("19: Set status to Connecting.");

        // Generate a room name with a random 4-digit number
        string roomName = Random.Range(1000, 9999).ToString();
        Debug.Log("20: Generated room name: " + roomName);

        RoomOptions roomOptions = new RoomOptions();
        Debug.Log("21: Created RoomOptions.");
        roomOptions.MaxPlayers = 4;
        Debug.Log("22: Set MaxPlayers to 4.");

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        Debug.Log("23: Attempting to create room: " + roomName);
    }


    // Callback invoked when the room is successfully created.
    public override void OnCreatedRoom()
    {
        setStatusMessage(ConnectionStatus.Connected);
        Debug.Log("Room created successfully: " + PhotonNetwork.CurrentRoom.Name);
        // You can now load your game scene or set up the multiplayer environment.
    }

    // Callback invoked if room creation fails.
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        setStatusMessage(ConnectionStatus.Offline);
        Debug.LogError("Room creation failed: " + message);
        // Optionally handle failure (retry or show an error message to the user).
    }

    // Method to join a room. Typically, you can call this from a UI button,
    // passing in the room name (which could be entered by the user).
    public void JoinRoom(string roomCode)
    {
        string roomName = roomCode;
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log("Attempting to join room: " + roomName);
    }

    // Callback invoked when you successfully join a room.
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() was called! Room: " + PhotonNetwork.CurrentRoom.Name);

        if (!PhotonNetwork.OfflineMode)
        {
            setStatusMessage(ConnectionStatus.Connected);
        }
        else { 
            setStatusMessage(ConnectionStatus.Offline);
        }
       
        if (PhotonNetwork.IsMasterClient)
        {
            
            chairManager.ResetChairs();
        }
        
 
        // Assume chairManager is used to get a spawn point
        Transform spawnPoint = chairManager.GetAvailableSpawnPoint();
        XROrigin.position = spawnPoint.position;
        XROrigin.rotation = spawnPoint.rotation;


        GameObject avatar = PhotonNetwork.Instantiate(characterModelName, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("===================================");
        Debug.Log("Avatar instantiated at: " + spawnPoint.position + " with rotation: " + spawnPoint.rotation);
        Debug.Log(avatar);
        Debug.Log("===================================");

        // Optionally, parent the avatar to the chair to keep it in place
        avatar.transform.parent = spawnPoint;
        avatar.tag = "Player";
        
        avatar.GetComponent<PlayerManager>().chairPosition = chairManager.chairSpawnPoints.IndexOf(spawnPoint);

        // Now assign the VR rig references to the avatar's controller.
        VRAvatarController controller = avatar.GetComponent<VRAvatarController>();
        if (controller != null)
        {
            // Assuming your NetworkManager already has these public references assigned in the Inspector
            controller.VRHead = vrHead;
            controller.VRLeftHand = vrLeftController;
            controller.VRRightHand = vrRightController;
        }
        else { 
            Debug.LogError("No VRAvatarController component found on the avatar.");
        }
    }

    public void GoOnline(string roomCode = "")
    {

        if (isGoingOnline)
        {
            Debug.Log("Already transitioning online.");
            return;
        }

        Debug.Log("1: GoOnline called with roomCode: " + roomCode);
        if (PhotonNetwork.OfflineMode)
        {
            Debug.Log("2: Currently in offline mode.");
            setStatusMessage(ConnectionStatus.Connecting);
            Debug.Log("3: Set status to Connecting.");

            isGoingOnline = true;
            Debug.Log("4: isGoingOnline set to true.");
            pendingRoomCode = roomCode;
            Debug.Log("5: pendingRoomCode set to: " + pendingRoomCode);
            PhotonNetwork.OfflineMode = false;
            Debug.Log("6: Offline mode disabled.");

            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("7: Photon is connected in offline mode; disconnecting...");
                PhotonNetwork.Disconnect();
                
                Debug.Log("8: Disconecting.");

                // We will reconnect in OnDisconnected callback to make sure the host seat is free
                return;
            }

            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("9: Calling ConnectUsingSettings() - transitioning to online mode...");
        }
        else
        {
            // for some reason we are trying to create a new connection... so we disconnect and try again
            Debug.Log("10: Already online.");
            isGoingOnline = true;
            PhotonNetwork.Disconnect();

        }
    }


    // Callback invoked if joining the room fails.
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        setStatusMessage(ConnectionStatus.Offline);
        Debug.LogError("Failed to join room: " + message);
        // Optionally, show an error message to the user or allow them to try again.
    }

    public void setStatusMessage(ConnectionStatus status)
    {

        Debug.Log("Setting status message: " + status);
        switch (status)
        {
            case ConnectionStatus.Offline:
                OfflineText.gameObject.SetActive(true);
                RoomCodeText.gameObject.SetActive(false);
                RoomCodeText.text = "Room Code: ERR";
                ConnectedText.gameObject.SetActive(false);
                ConnectingText.gameObject.SetActive(false);
                break;
            case ConnectionStatus.Connected:
                OfflineText.gameObject.SetActive(false);
                RoomCodeText.gameObject.SetActive(true);
                RoomCodeText.text = PhotonNetwork.CurrentRoom.Name;
                ConnectedText.gameObject.SetActive(true);
                ConnectingText.gameObject.SetActive(false);
                break;
            case ConnectionStatus.Connecting:
                OfflineText.gameObject.SetActive(false);
                RoomCodeText.gameObject.SetActive(false);
                RoomCodeText.text = "Room Code: ERR";
                ConnectedText.gameObject.SetActive(false);
                ConnectingText.gameObject.SetActive(true);
                break;
        }
    }
}
