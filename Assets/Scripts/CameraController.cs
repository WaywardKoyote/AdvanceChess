using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GridManager gridManager;
    public float rotSpeed = 5f;

    private Vector3 rotPoint;
    private float camInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotPoint = gridManager.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(rotPoint, Vector3.up, rotSpeed * camInput * Time.deltaTime);
    }

    public void OnMove(Vector2 value)
    {
        camInput = value.x;
    }
}
