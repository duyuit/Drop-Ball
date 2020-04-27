using UnityEngine;

namespace Player
{

    public class Player : MonoBehaviour
    {
        public float speed = 10;
        public ParticleSystem dustPS;
        public Joystick joyStick;
        protected Animator _animator;

        protected Rigidbody _rb;
        protected Vector3 _originPos;
        protected BallCatcher _ballCatcher;
        private float _lastSendPositionTime = 0;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();

            UpdateOriginPos();
            _ballCatcher = GetComponentInChildren<BallCatcher>();

            joyStick = FindObjectOfType<Joystick>();
        }

        public void UpdateOriginPos()
        {
            _originPos = transform.position;
        }

        protected virtual void FixedUpdate()
        {
            if (GlobalVariable.isOnline)
            {
                if (!GameOnlineController.Instance.isWaiting)
                    HandleInput();

               

            }
            else
            {
                if (!GameController.Instance.isWaiting)
                    HandleInput();
            }
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
            if (GlobalVariable.myIndex != 1)
                value *= -1f;

            if (Input.GetMouseButton(0))
            {
                transform.position += new Vector3(value.x, 0, value.y) * Time.fixedDeltaTime * speed;
                Vector3 movement = new Vector3(value.x, 0.0f, value.y);
                dustPS.transform.rotation = Quaternion.LookRotation(movement);
                _animator.SetTrigger("Run");
            }
            else
            {
                _animator.SetTrigger("Idle");
            }

            if (GlobalVariable.isOnline)
            {
                NetworkController.Instance.SendUpdateVelocity(value);

                if (Time.time - _lastSendPositionTime > 0.05f)
                {
                    NetworkController.Instance.SendUpdatePosition(transform.position);
                    _lastSendPositionTime = Time.time;
                }
            }
              
        }
    }
}