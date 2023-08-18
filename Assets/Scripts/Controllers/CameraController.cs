using System.Linq;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        public bool editorMode;
        
        private Vector3 _offset;
        private Camera _camera;
        public float maxZoom;
        public float minZoom;
        public float zoomSpeed;
        private float _zoom;

        private Vector3 _startPos;
        private Vector3 _downLeftBound;
        private Vector3 _upRightBound;
        
        private void SetBounds()
        {
            var dlbGrid = GridManager.Instance.GetLeftDownBorder();
            var urbGrid = GridManager.Instance.GetRightUpBorder();
            var offset = GridManager.CellSize;
            _downLeftBound = new Vector3((dlbGrid.x - 1)* offset.x, (dlbGrid.y - 1 ) * offset.y, -10);
            _upRightBound = new Vector3((urbGrid.x + 1) * offset.x, (urbGrid.y + 1) * offset.y, -10);
        
            if (IsOutOfBounds(transform.position))
            {
                var X = (urbGrid.x - dlbGrid.x) / 2;
                var Y = (urbGrid.y - dlbGrid.y) / 2;
                transform.position = new Vector3(X * offset.x, Y * offset.y, -10);
            }
        }

        void Start()
        {
            _camera = Camera.main;
            _offset = Vector3.zero;
            _zoom = _camera.orthographicSize;
            _startPos = transform.position;
            GridManager.GridGenerated += SetBounds;
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _startPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetKey(KeyCode.Mouse2))
            {
                _offset = _startPos - _camera.ScreenToWorldPoint(Input.mousePosition);
            }

            //Zoom in and out with mouse wheel
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (_camera.orthographicSize > minZoom)
                {
                    _zoom -= zoomSpeed;
                    _offset = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                }
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (_camera.orthographicSize < maxZoom)
                {
                    _zoom += zoomSpeed;
                    _offset = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                }
            }

            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, Time.unscaledDeltaTime);

            if (_camera.orthographicSize > maxZoom)
            {
                _camera.orthographicSize = maxZoom;
                _zoom = maxZoom;
            }

            if (_camera.orthographicSize < minZoom)
            {
                _camera.orthographicSize = minZoom;
                _zoom = minZoom;
            }
        
            //=====================
        
        
        
            if (_offset.magnitude > 0.01f)
            {
                var deltaOffset = _offset.normalized * Mathf.Lerp(0, Vector3.Magnitude(_offset), Time.unscaledDeltaTime);
                transform.position += deltaOffset;
                if (IsOutOfBounds(transform.position))
                {
                    transform.position -= deltaOffset;
                }
                _offset -= deltaOffset;
            }
        }

        private bool IsOutOfBounds(Vector3 position)
        {
            return position.x < _downLeftBound.x || position.x > _upRightBound.x || position.y < _downLeftBound.y ||
                   position.y > _upRightBound.y;
        }
    }
}