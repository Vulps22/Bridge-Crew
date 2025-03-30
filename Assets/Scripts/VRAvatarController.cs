using UnityEngine;
using Photon.Pun;

public class VRAvatarController : MonoBehaviourPun
{
    [Header("Head")]
    public Transform AvatarHead;
    public Transform VRHead;
    public Vector3 headOffset;
    public Vector3 headRotation; // Euler angles offset for the head

    [Header("Left Hand")]
    public Transform AvatarLeftHand;
    public Transform VRLeftHand;
    public Vector3 leftHandOffset;
    public Vector3 leftHandRotation; // Euler angles offset for the left hand

    [Header("Right Hand")]
    public Transform AvatarRightHand;
    public Transform VRRightHand;
    public Vector3 rightHandOffset;
    public Vector3 rightHandRotation; // Euler angles offset for the right hand

    [Header("Body")]
    public Transform AvatarBody;
    public Vector3 bodyOffset;

    void Update()
    {
        // Only control this avatar if it's the local player's
        if (!photonView.IsMine) return;

        if (VRHead != null)
        {
            AvatarHead.position = VRHead.TransformPoint(headOffset);
            AvatarHead.rotation = VRHead.rotation * Quaternion.Euler(headRotation);
        }
        if (VRLeftHand != null)
        {
            AvatarLeftHand.position = VRLeftHand.TransformPoint(leftHandOffset);
            AvatarLeftHand.rotation = VRLeftHand.rotation * Quaternion.Euler(leftHandRotation);
        }
        if (VRRightHand != null)
        {
            AvatarRightHand.position = VRRightHand.TransformPoint(rightHandOffset);
            AvatarRightHand.rotation = VRRightHand.rotation * Quaternion.Euler(rightHandRotation);
        }
        if (AvatarBody != null)
        {
            // The body is fixed relative to the parent's (this transform's) position plus a constant offset.
            AvatarBody.position = transform.position + bodyOffset;
            // Optionally, maintain a fixed body rotation (e.g., keep the body upright and aligned with the parent's Y rotation).
            AvatarBody.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }
}
