using UnityEngine;

public class TransformFollow_UI : MonoBehaviour
{
    public Transform subject;

    public float distance = 3.5f;         
    public float heightOffset = -0.2f;  

    public float positionSmooth = 5f;
    public float rotationSmooth = 5f;

    public bool keepUpright = true;
    public bool invertYRotation = true; 

    Vector3 targetPos;
    Quaternion targetRot;

    void LateUpdate()
    {
        if (!subject) return;

        targetPos = subject.position + subject.forward * distance + Vector3.up * heightOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmooth);

        if (keepUpright)
        {
            Vector3 lookDir = transform.position - subject.position;
            lookDir.y = 0; 
            if (lookDir.sqrMagnitude > 0.001f)
                targetRot = Quaternion.LookRotation(lookDir);
        }
        else
        {
            targetRot = Quaternion.LookRotation(transform.position - subject.position);
        }

        if (invertYRotation)
            targetRot *= Quaternion.Euler(0, 180f, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmooth);
    }
}
