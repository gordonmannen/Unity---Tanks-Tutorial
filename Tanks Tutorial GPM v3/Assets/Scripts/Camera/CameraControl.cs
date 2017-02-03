using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                     // Helps to smooth the camera as it refocuses to a new point as the tanks move.
    public float m_ScreenEdgeBuffer = 4f;               // Buffer at the edge of the screen to keep the outermost tank from moving off screen.
    public float m_MinSize = 6.5f;                      // Sets the lower size limit for the camera view.
    [HideInInspector] public Transform[] m_Targets;     // Tells the camera what targets it needs to cover, but I believe this is hidden because we
                                                        // are telling the gamemanager to handle this.


    private Camera m_Camera;                            // A reference for the camera.
    private float m_ZoomSpeed;                          // A speed reference for the camera to assist in smoothing out the transitions.
    private Vector3 m_MoveVelocity;                     // Another reference related to assisting in smoothing out the camera transitions.
    private Vector3 m_DesiredPosition;                  // Target position for the camera to move to.


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
                                                        // This works in this scenario because we only have one camera, we would need to be more specific if there were multiple.
    }


    private void FixedUpdate()
    {
        Move();                                         // Move camera to desired new position.
        Zoom();                                         // Zoom camera in/out.
    }


    private void Move()
    {
        FindAveragePosition();                          // Find desired new position using avg position between player 1 & 2.

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
                                                        // Move to new position smoothly.
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)      // Iterate through targets and add positions (extensible in the case of increasing no of players)
        {
            if (!m_Targets[i].gameObject.activeSelf)    // If target inactive go to next.
                continue;

            averagePos += m_Targets[i].position;        // Add to average and increment up until all targets have been evaluated.
            numTargets++;
        }

        if (numTargets > 0)                             // As long as there is at least one valid target, divide avg position by number of active targets.
            averagePos /= numTargets;

        averagePos.y = transform.position.y;            // That avg position will now be transformed into the desired position.

        m_DesiredPosition = averagePos;                 // Desired position is avg position.
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
                                                         // Determine required size using desired position and smoothly transform to that size.
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
                                                        // This section involves finding the position the camera is moving towards in order to make sure the view area is the correct size, no tanks are pushing over the buffer, etc.

        float size = 0f;                                // Starting at zero before introducing the size calculated based on the relative position of the tank(s).

        for (int i = 0; i < m_Targets.Length; i++)      // Iterate through the active targets.
        {
            if (!m_Targets[i].gameObject.activeSelf)    // Ignore inactive targets.
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
                                                        // Find the positions of the active targets.

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
                                                        // Used to determine which active target is farther away from the desired position.

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
                                                        // Calculates the new required size based on the tank that is the farthest from the desired position.

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
                                                        // Calculate whether the current size is larger or smaller than calculated size.  Do we need to zoom camera in or out as it is repositioned?
        }

        size += m_ScreenEdgeBuffer;                     // Adds the buffer to the calculation.

        size = Mathf.Max(size, m_MinSize);              // Insure calculated size is not below minimum size of 6.5f.

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();                          // Find desired position.

        transform.position = m_DesiredPosition;         // Set camera to desired position, no damping needed as this is for starting new round/game.

        m_Camera.orthographicSize = FindRequiredSize(); // Determine required camera size to start round/game.
    }
}