using System;
using UnityEngine;

namespace gameview
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler INSTANCE { get; private set; }

        public event Action<Collider2D> OnMouseLeftClick;
        public event Action OnEsc;

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
            if (Input.GetMouseButtonDown(0))
                OnMouseLeftClick?.Invoke(Physics2D.OverlapPoint(GetMousePosition()));
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEsc?.Invoke();
        }

        private Vector3 GetMousePosition()
        {
            var p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            return p;
        }
    }
}
