using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public bool yLock = false;

    // Update is called once per frame
    void Update()
    {
        LookAtCamera();
    }

    private void LookAtCamera()
    {
        Vector3 target = Camera.main.transform.position;

        if (yLock)
        {
            target.y = 0;
        }

        transform.LookAt(target);
    }
}
