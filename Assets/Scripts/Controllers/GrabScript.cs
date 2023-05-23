using UnityEngine;

namespace Controllers
{
    public class GrabScript : MonoBehaviour
    {
        private Camera _camera;
        private Transform _grabbedObject = null;
        private bool _isGrabbing = false;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            bool grabbedObjectExist = _grabbedObject != null;
        
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (_isGrabbing)
                {
                    if (grabbedObjectExist)
                    {
                        _grabbedObject = null;
                        _isGrabbing = false;
                    }
                }
                else
                {
                    if (!grabbedObjectExist)
                    {
                        OnLeftClick();
                    }

                }
            }

            if (grabbedObjectExist)
            {
                Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = _grabbedObject.position.z;
                _grabbedObject.position = mousePosition;
            }
        }

        private void OnLeftClick()
        {
            // Raycast из позиции мышки в направлении камеры только для объектов на слое Grabable
            RaycastHit2D hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f,
                LayerMask.GetMask("Grabable"));
            if (hit.collider)
            {
                _isGrabbing = true;
                //Если что-то попалось, то запоминаем объект
                _grabbedObject = hit.transform;
            }
        }
    }
}