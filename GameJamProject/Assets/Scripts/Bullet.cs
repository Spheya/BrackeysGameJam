using UnityEngine;

namespace Assets.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private bool _isFriendly;
        public bool IsFriendly { get => _isFriendly; set => _isFriendly = value; }

        [SerializeField]
        private float _speed;
        public float Speed { get => _speed; set => _speed = value; }

        public void Update()
        {
            transform.position += transform.right * -_speed * Time.deltaTime;

            if (transform.position.sqrMagnitude > 100)
                Destroy(gameObject);
        }
    }
}
