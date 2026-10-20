using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float kecepatan = 5f;

    private Vector2 arahGerak;

    void OnMove(InputValue value)
    {
        arahGerak = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 arah = new Vector3(arahGerak.x, arahGerak.y, 0);
        transform.position += arah * kecepatan * Time.deltaTime;
    }

    // Pengambilan koin sekarang ditangani oleh Item.cs (masuk ke inventory),
    // jadi OnTriggerEnter2D untuk koin dihapus dari sini.
}
