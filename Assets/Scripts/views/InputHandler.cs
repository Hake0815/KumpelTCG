using System;
using UnityEngine;

namespace gameview
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler INSTANCE { get; private set; }
        public event Action<Collider2D> OnMouseLeftClick;
        public event Action<Collider2D> OnMouseRightClick;
        public event Action OnEsc;
        public event Action OnSpace;
        public event Action<float> OnMouseWheel;
        private bool _skipFrame = false;

        protected virtual void Awake()
        {
            if (INSTANCE != null)
            {
                Destroy(gameObject);
                return;
            }
            INSTANCE = this;
        }

        protected virtual void OnApplicationQuit()
        {
            INSTANCE = null;
            Destroy(gameObject);
        }

        public void Update()
        {
            if (_skipFrame)
            {
                _skipFrame = false;
                return;
            }
            if (Input.GetMouseButtonDown(0))
                OnMouseLeftClick?.Invoke(Physics2D.OverlapPoint(GetMousePosition()));
            if (Input.GetMouseButtonDown(1))
                OnMouseRightClick?.Invoke(Physics2D.OverlapPoint(GetMousePosition()));
            if (Input.GetKeyDown(KeyCode.Space))
                OnSpace?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEsc?.Invoke();
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                OnMouseWheel?.Invoke(Input.GetAxis("Mouse ScrollWheel"));
        }

        private static Vector3 GetMousePosition()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }

        public void SkipOneFrame()
        {
            _skipFrame = true;
        }
    }
}
