using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GridManager gridManager;
    public float rotSpeedX = 30f;
    public float rotSpeedY = 40f;
    public float lowerClamp = 10f;
    public float upperClamp = 40f;

    private Vector3 rotPoint;
    private float camInputX;
    private float camInputY;

    private float vertRot;

    public bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotPoint = gridManager.transform.position;
        rotPoint.x -= 0.5f;
        rotPoint.z -= 0.5f;
        vertRot = this.transform.rotation.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            this.transform.RotateAround(rotPoint, Vector3.up, camInputX * Time.deltaTime);

            if ((camInputY > 0) && (vertRot < upperClamp))
            {
                this.transform.RotateAround(rotPoint, this.transform.right, camInputY * Time.deltaTime);
                vertRot = this.transform.rotation.eulerAngles.x;
            }
            else if ((camInputY < 0) && (vertRot > lowerClamp))
            {
                this.transform.RotateAround(rotPoint, this.transform.right, camInputY * Time.deltaTime);
                vertRot = this.transform.rotation.eulerAngles.x;
            }
        }
        else
        {
            this.transform.RotateAround(rotPoint, Vector3.up, -10f * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        camInputX = input.x * rotSpeedX * -1;
        camInputY = input.y * rotSpeedY;
    }
}
