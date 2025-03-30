using UnityEngine;

[System.Serializable]
public class VRMap
{
    public bool isHead = false;
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;
    public void Map()
    {

        if (!isHead)
        {
            ikTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        }
        ikTarget.rotation = vrTarget.rotation;

        //if head log the position and rotation of the variables
        if (isHead)
        {
            //Debug.Log("Head Position: " + vrTarget.position);
            //Debug.Log("Head Rotation: " + vrTarget.rotation);
            //Debug.Log("IK Position: " + ikTarget.position);
            //Debug.Log("IK Rotation: " + ikTarget.rotation);
        }
    }
}

public class IKTargetFollowVRRig : MonoBehaviour
{
    [Range(0,1)]
    public float turnSmoothness = 0.1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPositionOffset;
    public float headBodyYawOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = head.ikTarget.position + headBodyPositionOffset;

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }

}
