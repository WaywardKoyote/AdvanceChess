using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GridManager gridManager;
    public float rotSpeedX = 5f;
    public float rotSpeedY = 5f;

    private Vector3 rotPoint;
    private float camInputX;
    private float camInputY;

    private float vertRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotPoint = gridManager.transform.position;
        vertRot = this.transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(rotPoint, Vector3.up, camInputX * Time.deltaTime);

        if ((camInputY > 0) && (vertRot < 40))
        {
            this.transform.RotateAround(rotPoint, this.transform.right, camInputY * Time.deltaTime);
            vertRot = this.transform.rotation.eulerAngles.x;
        }
        else if ((camInputY < 0) && (vertRot > 5))
        {
            this.transform.RotateAround(rotPoint, this.transform.right, camInputY * Time.deltaTime);
            vertRot = this.transform.rotation.eulerAngles.x;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        camInputX = input.x * rotSpeedX * -1;
        camInputY = input.y * rotSpeedY;
    }
}
