using UnityEngine;

public class UIDirectionControl : MonoBehaviour     // This class is used to control world space UI
{
    public bool m_UseRelativeRotation = true;       // Use relative rotation


    private Quaternion m_RelativeRotation;          // The relative rotation at start of the scene


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
