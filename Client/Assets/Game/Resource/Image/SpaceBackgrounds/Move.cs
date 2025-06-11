using UnityEngine;

public class Move : MonoBehaviour {

    public float Speed;
    private float x;
    public float Destination;
    public float StartPoint;

    // Update is called once per frame
    private void Update()
    {
        x = transform.position.x;
        x += Speed * Time.deltaTime;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        if (x <= Destination) {
            x = StartPoint;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
    }
}
