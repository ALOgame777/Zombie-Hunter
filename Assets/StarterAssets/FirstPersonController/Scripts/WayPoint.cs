using UnityEngine; // 유니티에서 필요한 것들을 가져와

public class WayPoint : MonoBehaviour // 큐브를 제어하는 클래스야
{
    public float rotationSpeed = 50f; // 큐브가 회전하는 속도야
    public float moveSpeed = 1f; // 큐브가 위아래로 움직이는 속도야
    public float moveAmount = 0.5f; // 큐브가 위아래로 얼마나 움직이는지를 정해
    private Vector3 startPosition; // 큐브의 초기 위치를 저장하는 변수야

    void Start() // 게임이 시작될 때 한 번 실행돼
    {
        startPosition = transform.position; // 큐브의 처음 위치를 저장해
    }

    void Update() // 매 프레임마다 실행돼
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // 큐브를 계속 회전시켜

        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount; // 큐브의 새로운 y 위치를 계산해
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // 큐브를 새로운 위치로 옮겨
    }
}
