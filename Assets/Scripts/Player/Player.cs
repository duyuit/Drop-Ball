using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public float speed = 10;
        public ParticleSystem dustPS;
        public Joystick joyStick;
        protected Animator _animator;

        private Rigidbody _rb;
        protected Vector3 _originPos;
        protected BallCatcher _ballCatcher;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            _originPos = transform.position;
            _ballCatcher = GetComponentInChildren<BallCatcher>();
        }

        protected virtual void FixedUpdate()
        {
            if (!GameController.Instance.isWaiting)
                HandleInput();
        }

        public void Reset()
        {
            _animator.Play("Idle");
            _rb.velocity = new Vector3(0, 0, 0);
            transform.position = _originPos;
            _ballCatcher.Reset();
        }

        public void Win()
        {
            _animator.Play("Dance" + Random.Range(1, 5));
        }

        public void Lose()
        {
            _animator.SetTrigger("Die");
        }

        void HandleInput()
        {
            var value = joyStick.Direction;

            if (Input.GetMouseButton(0))
            {
                transform.position =
                    transform.position + new Vector3(value.x, 0, value.y) * Time.fixedDeltaTime * speed;
                Vector3 movement = new Vector3(value.x, 0.0f, value.y);
                //_rb.velocity = movement *  speed;
                dustPS.transform.rotation = Quaternion.LookRotation(movement);
                _animator.SetTrigger("Run");

                //if (!grassPS.isPlaying)
                //    grassPS.Play();
            }
            else
            {
                _animator.SetTrigger("Idle");
                //grassPS.Stop();
            }
        }
    }
}