using UnityEngine;
using Cinemachine;

public class CineTouch : MonoBehaviour
{
    [SerializeField] private TouchField _touchInput;
    [SerializeField] private CinemachineFreeLook _freeLookCamera;
    [SerializeField] private GameObject _fpsCameraObject;

    private float _dragSpeed = 0.01f;

    public static bool _isFirstPersonView;

    private Vector2 _lookInput;
    private Vector2 _lastTouchPos;

    [SerializeField] private float _touchSpeedSensitivityX;
    [SerializeField] private float _touchSpeedSensitivityY;
    [SerializeField] private float _zoomSpeed = 40f;

    private string _touchXMapTo = "Mouse X";
    private string _touchYMapTo = "Mouse Y";

    private Vector3 StoredFpsObjectPosition;

    private float _targetXAxisValue;
    private float _targetYAxisValue;
    private float _targetZoomValue;
    [SerializeField] private float _lerpSpeed = 1f;

    private float _targetTopRigRadius;
    private float _targetTopRigHeight;
    private float _targetMiddleRigRadius;
    private float _targetMiddleRigHeight;
    private float _targetBottomRigRadius;
    private float _targetBottomRigHeight;
    private float _rigLerpSpeed = 5f;

    private void Start()
    {
        _isFirstPersonView = false;
        CinemachineCore.GetInputAxis = GetInputAxis;

        _touchSpeedSensitivityX *= -1;
        _touchSpeedSensitivityY *= -1;

        _targetXAxisValue = _freeLookCamera.m_XAxis.Value;
        _targetYAxisValue = _freeLookCamera.m_YAxis.Value;
        // Initialize rig values
        _targetTopRigRadius = _freeLookCamera.m_Orbits[0].m_Radius;
        _targetTopRigHeight = _freeLookCamera.m_Orbits[0].m_Height;
        _targetMiddleRigRadius = _freeLookCamera.m_Orbits[1].m_Radius;
        _targetMiddleRigHeight = _freeLookCamera.m_Orbits[1].m_Height;
        _targetBottomRigRadius = _freeLookCamera.m_Orbits[2].m_Radius;
        _targetBottomRigHeight = _freeLookCamera.m_Orbits[2].m_Height;
    }

    private float GetInputAxis(string axisName)
    {
        float input = Input.GetAxis(axisName);

        if (!_isFirstPersonView && Input.touchCount == 1)
        {
            Vector2 lookInput = _touchInput.PlayerJoystickOutputVector();
            if (axisName == _touchXMapTo)
                input = lookInput.x * _touchSpeedSensitivityX;
            if (axisName == _touchYMapTo)
                input = lookInput.y * _touchSpeedSensitivityY;
        }
        else
        {
            input = 0;
        }

        if (Input.touchCount == 2)
        {
            input = 0;
            // Calculate pinch-to-zoom input
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                // Store initial touch positions and zoom value
                _lastTouchPos = touch1.position;
                _targetZoomValue = _freeLookCamera.m_Orbits[0].m_Radius;
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                // Calculate the current touch positions and zoom value
                Vector2 currentTouch1Pos = touch1.position;
                Vector2 currentTouch2Pos = touch2.position;
                float currentTouchDistance = Vector2.Distance(currentTouch1Pos, currentTouch2Pos);
                float touchDelta = currentTouchDistance - Vector2.Distance(_lastTouchPos, _lastTouchPos);

                // Adjust the target zoom value based on the touch delta
                _targetZoomValue -= touchDelta * _zoomSpeed;
                _targetZoomValue = Mathf.Clamp(_targetZoomValue, _freeLookCamera.m_Orbits[0].m_Radius, _freeLookCamera.m_Orbits[2].m_Radius);

                _targetTopRigRadius -= _targetZoomValue;
                _targetTopRigHeight -= _targetZoomValue;
                _targetMiddleRigRadius -= _targetZoomValue;
                _targetMiddleRigHeight -= _targetZoomValue;
                _targetBottomRigRadius -= _targetZoomValue;
                _targetBottomRigHeight -= _targetZoomValue;
                
                _lastTouchPos = currentTouch1Pos;
            }
        }

        return input;
    }

    private void Update()
    {
        if (_isFirstPersonView)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _lastTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 deltaPos = touch.position - _lastTouchPos;
                    deltaPos.x *= -1; // Invert x-axis movement
                    deltaPos.y *= -1; // Invert y-axis movement

                    Vector3 cameraForward = Camera.main.transform.forward.normalized;
                    Vector3 cameraRight = Camera.main.transform.right.normalized;
                    Vector3 cameraUp = Camera.main.transform.up.normalized;

                    // Calculate movement direction relative to camera orientation
                    Vector3 moveDir = Vector3.zero;
                    if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                    {
                        moveDir += cameraRight * deltaPos.x;
                    }
                    else
                    {
                        moveDir += cameraUp * deltaPos.y;
                    }

                    if (Mathf.Abs(deltaPos.x) > 0.1f && Mathf.Abs(deltaPos.y) > 0.1f)
                    {
                        // Diagonal movement
                        moveDir += (cameraRight * deltaPos.x + cameraUp * deltaPos.y) * 0.5f;
                    }

                    moveDir *= _dragSpeed * Time.deltaTime;
                    _fpsCameraObject.transform.position += moveDir;
                    _lastTouchPos = touch.position;
                }
            }
        }
        else
        {
            StoredFpsObjectPosition = _fpsCameraObject.transform.position;
        }
    }

    void FixedUpdate(){
        // Lerp between the current camera values and the target values
        _freeLookCamera.m_XAxis.Value = Mathf.Lerp(_freeLookCamera.m_XAxis.Value, _targetXAxisValue, Time.deltaTime * _lerpSpeed);
        _freeLookCamera.m_YAxis.Value = Mathf.Lerp(_freeLookCamera.m_YAxis.Value, _targetYAxisValue, Time.deltaTime * _lerpSpeed);

        // Reset the target values after each update to prevent continuous movement
        _targetXAxisValue = _freeLookCamera.m_XAxis.Value;
        _targetYAxisValue = _freeLookCamera.m_YAxis.Value;

        // Lerp between the current rig values and the target rig values
        _freeLookCamera.m_Orbits[0].m_Radius = Mathf.Lerp(_freeLookCamera.m_Orbits[0].m_Radius, _targetTopRigRadius, Time.deltaTime * _rigLerpSpeed);
        _freeLookCamera.m_Orbits[0].m_Height = Mathf.Lerp(_freeLookCamera.m_Orbits[0].m_Height, _targetTopRigHeight, Time.deltaTime * _rigLerpSpeed);
        _freeLookCamera.m_Orbits[1].m_Radius = Mathf.Lerp(_freeLookCamera.m_Orbits[1].m_Radius, _targetMiddleRigRadius, Time.deltaTime * _rigLerpSpeed);
        _freeLookCamera.m_Orbits[1].m_Height = Mathf.Lerp(_freeLookCamera.m_Orbits[1].m_Height, _targetMiddleRigHeight, Time.deltaTime * _rigLerpSpeed);
        _freeLookCamera.m_Orbits[2].m_Radius = Mathf.Lerp(_freeLookCamera.m_Orbits[2].m_Radius, _targetBottomRigRadius, Time.deltaTime * _rigLerpSpeed);
        _freeLookCamera.m_Orbits[2].m_Height = Mathf.Lerp(_freeLookCamera.m_Orbits[2].m_Height, _targetBottomRigHeight, Time.deltaTime * _rigLerpSpeed);
    }

    private void OnDestroy()
    {
        // Reset the field of view of the camera when this script is destroyed

    }

    public void SwitchViewMode()
    {
        if (_isFirstPersonView)
        {
            // Switch from first person view to third person view
            _isFirstPersonView = false;
        }
        else
        {
            // Switch from third person view to first person view
            _isFirstPersonView = true;
        }
    }

    public void rotateClockWise()
    {
        // Rotate the camera clockwise
        _targetXAxisValue -= 60f;
    }

    public void rotateCounterClockWise()
    {
        // Rotate the camera counter-clockwise
        _targetXAxisValue += 60f;
    }

    public void ZoomIn()
    {
        // Decrease the radius and height of the top, middle, and bottom rigs
        _targetTopRigRadius -= _zoomSpeed;
        _targetTopRigHeight -= _zoomSpeed;
        _targetMiddleRigRadius -= _zoomSpeed;
        _targetMiddleRigHeight -= _zoomSpeed;
        _targetBottomRigRadius -= _zoomSpeed;
        _targetBottomRigHeight -= _zoomSpeed;
    }

    public void ZoomOut()
    {
        // Increase the radius and height of the top, middle, and bottom rigs
        _targetTopRigRadius += _zoomSpeed;
        _targetTopRigHeight += _zoomSpeed;
        _targetMiddleRigRadius += _zoomSpeed;
        _targetMiddleRigHeight += _zoomSpeed;
        _targetBottomRigRadius += _zoomSpeed;
        _targetBottomRigHeight += _zoomSpeed;
    }
}
