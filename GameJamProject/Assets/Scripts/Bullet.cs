using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private bool _isFriendly;
    public bool IsFriendly { get => _isFriendly; set => _isFriendly = value; }

    [SerializeField]
    private float _speed;
    public float Speed { get => _speed; set => _speed = value; }

    [SerializeField]
    private GameObject _splash;
    public GameObject Splash { get => _splash; set => _splash = value; }

    public void Update()
    {
        float distance = _speed * Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, -transform.right);
        if(hit.collider != null && hit.distance < distance)
        {
            if(_isFriendly)
            {
                var enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    var splash = Instantiate(Splash);
                    splash.transform.position = new Vector3(hit.point.x, hit.point.y, splash.transform.position.z);
                    enemy.GetComponent<SpriteFlasher>().Flash();
                    enemy.TakeDamage(55.0f);
                    Destroy(gameObject);
                }
            }
        }

        transform.position += -transform.right * distance;

        if (Mathf.Abs(transform.position.x) > 7.5f || Mathf.Abs(transform.position.y) > 5.5f)
            Destroy(gameObject);
    }
}
