using UnityEngine; // ����Ƽ���� �ʿ��� �͵��� ������

public class WayPoint : MonoBehaviour // ť�긦 �����ϴ� Ŭ������
{
    public float rotationSpeed = 50f; // ť�갡 ȸ���ϴ� �ӵ���
    public float moveSpeed = 1f; // ť�갡 ���Ʒ��� �����̴� �ӵ���
    public float moveAmount = 0.5f; // ť�갡 ���Ʒ��� �󸶳� �����̴����� ����
    private Vector3 startPosition; // ť���� �ʱ� ��ġ�� �����ϴ� ������

    void Start() // ������ ���۵� �� �� �� �����
    {
        startPosition = transform.position; // ť���� ó�� ��ġ�� ������
    }

    void Update() // �� �����Ӹ��� �����
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // ť�긦 ��� ȸ������

        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveAmount; // ť���� ���ο� y ��ġ�� �����
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); // ť�긦 ���ο� ��ġ�� �Ű�
    }
}
