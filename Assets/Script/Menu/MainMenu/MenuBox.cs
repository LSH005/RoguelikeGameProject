using UnityEngine;

public class MenuBox : MonoBehaviour
{
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -5f;
    public float maxY = 5f;
    public float minDestroyDelay = 0.1f;
    public float maxDestroyDelay = 0.5f;
    private float rotationSpeed;
    public float minRotationSpeed = 50f;
    public float maxRotationSpeed = 1000f;

    void Start()
    {
        MoveToRandomPosition();
        SetRandomInitialRotation();
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        float delayTime = Random.Range(minDestroyDelay, maxDestroyDelay);
        Destroy(gameObject, delayTime);
    }

    void Update()
    {
        transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }

    private void MoveToRandomPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector3(randomX, randomY, transform.position.z);
    }

    private void SetRandomInitialRotation()
    {
        float randomZRotation = Random.Range(0f, 90f);
        transform.rotation = Quaternion.Euler(0, 0, randomZRotation);
    }
}