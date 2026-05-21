using UnityEngine;

public class CameraController
{
    private Vector2 _camera_position;
    private float _camera_zoom;

    private const float ZOOM_SPEED = 0.5f;
    private const float MOVE_SPEED = 2.0f;
    private const float MAX_ZOOM = 1000f;
    private const float MIN_ZOOM = 0.001f;

    public Camera mainCamera;

    public void Init()
    {
        mainCamera = Camera.main;
    }

    public void Free()
    {
        mainCamera = null;
    }

    public void SetNew()
    {
        _camera_position = new Vector2(0, 0);
        _camera_zoom = mainCamera.orthographicSize;

        mainCamera.transform.position = new Vector3(_camera_position.x, _camera_position.y, -10f);
        mainCamera.orthographicSize = _camera_zoom;
    }

    public void OnUpdate()
    {
        UpdateCamera();
    }

    public void UpdateCamera()
    {
        if (UIRoot.instance.settingWindow.loadFileWindow.isHoveringLoadWindow)
        {
            return;
        }

        // 处理缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _camera_zoom -= scroll * _camera_zoom * ZOOM_SPEED;
            _camera_zoom = Mathf.Clamp(_camera_zoom, MIN_ZOOM, MAX_ZOOM);

            mainCamera.orthographicSize = _camera_zoom;
        }

        // 处理平移
        Vector2 moveDir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.y -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x += 1;
        }

        if (moveDir != Vector2.zero)
        {
            float speed = _camera_zoom * MOVE_SPEED;
            _camera_position += moveDir * speed * Time.deltaTime;

            mainCamera.transform.position = new Vector3(_camera_position.x, _camera_position.y, -10f);
        }
    }
}
