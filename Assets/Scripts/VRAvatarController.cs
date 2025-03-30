using UnityEngine;
using Photon.Pun;

public class VRAvatarController : MonoBehaviourPun
{
    // The bones of your character model
    public Transform headBone;
    public Transform leftHandBone;
    public Transform rightHandBone;

    // The VR references assigned by the Network Manager
    public Transform vrHead;
    public Transform vrLeftController;
    public Transform vrRightController;

    void Update()
    {
        // Only control this avatar if it's the local player's
        if (!photonView.IsMine) return;

        if (vrHead != null)
        {
            headBone.position = vrHead.position;
            headBone.rotation = vrHead.rotation;
        }
        if (vrLeftController != null)
        {
            leftHandBone.position = vrLeftController.position;
            leftHandBone.rotation = vrLeftController.rotation;
        }
        if (vrRightController != null)
        {
            rightHandBone.position = vrRightController.position;
            rightHandBone.rotation = vrRightController.rotation;
        }
    }
}
