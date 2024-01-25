using System;
using Simulation.Runtime.Entities;
using UnityEngine;
using static Simulation.Runtime.PlayerInput.Input;

namespace Simulation.Runtime.Camera
{
    public class FreeCamera : MonoBehaviour
    {
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _fastSpeed = 40;
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _size = 10;
        [SerializeField] private int _z = -10;
        [SerializeField] private UnityEngine.Camera _camera;

        private Vector3 _currentPosition;

        private void Awake()
        {
            _camera.orthographicSize = _size;
            _currentPosition = new Vector3Int(0, 0, _z);
        }

        private void LateUpdate()
        {
            if (Controls.GamePlay.enabled)
            {
                if (Controls.GamePlay.Zoom.WasPerformedThisFrame())
                {
                    _size -= Controls.GamePlay.Zoom.ReadValue<float>() * Time.deltaTime * _zoomSpeed;
                    _size = Mathf.Clamp(_size, 1f, 10f);
                    _camera.orthographicSize = _size;
                }
                
                var direction = Controls.GamePlay.Movement.ReadValue<Vector2>();

                var speed = _speed;

                if (Controls.GamePlay.SpeedUpCamera.IsPressed())
                {
                    speed = _fastSpeed;
                }
                
                direction *= speed;

                _currentPosition += new Vector3(direction.x, direction.y, 0) * Time.deltaTime;

                var vertical = _size;
                var horizontal = vertical * _camera.aspect;

                var maxCameraBound = _currentPosition + new Vector3(horizontal, vertical);
                var minCameraBound = _currentPosition - new Vector3(horizontal, vertical);

                var maxBounds = GameWorld.Bounds.Max;
                var minBounds = GameWorld.Bounds.Min;

                if (maxCameraBound.x > maxBounds.x)
                {
                    _currentPosition.x -= Math.Abs(maxCameraBound.x - maxBounds.x);
                }

                if (maxCameraBound.y > maxBounds.y)
                {
                    _currentPosition.y -= Math.Abs(maxCameraBound.y - maxBounds.y);
                }

                if (minCameraBound.x < minBounds.x)
                {
                    _currentPosition.x += Math.Abs(minCameraBound.x - minBounds.x);
                }

                if (minCameraBound.y < minBounds.y)
                {
                    _currentPosition.y += Math.Abs(minCameraBound.y - minBounds.y);
                }

                _camera.transform.position = new Vector3(_currentPosition.x, _currentPosition.y, _z);
            }
        }
    }
}