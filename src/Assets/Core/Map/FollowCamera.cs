using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform Target;
    public bool ScreenToWorldPoint = false;
    public float Speed = 1;
    Camera cam;
    float initZ;
    void Start()
    {
        this.cam = Camera.main;
        this.initZ = this.transform.position.z;
    }

    void Update()
    {
        if (this.Target != null && cam != null)
        {
            var p = this.ScreenToWorldPoint ? this.cam.ScreenToWorldPoint(this.Target.position) : this.Target.position;
            p.z = this.initZ;
            this.transform.position = Vector3.Lerp(this.transform.position, p, Time.deltaTime * this.Speed);
        }
    }
}
