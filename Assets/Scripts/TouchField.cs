using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class TouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Vector2 _playerTouchVectorOutput;
    private bool _isPlayerTouchingPanel;
    private Touch _myTouch;
    private int _touchID;

    private void FixedUpdate()
    {
        if (Input.touchCount > 0)
        {
            _myTouch = Input.GetTouch(0);
            if (_isPlayerTouchingPanel)
            {
                if (_myTouch.fingerId == _touchID)
                {
                    if (_myTouch.phase != TouchPhase.Moved || Input.touchCount == 2)
                        OutputVectorValue(Vector2.zero);
                }
            }
        }
    }

    private void OutputVectorValue(Vector2 outputValue)
    {
        _playerTouchVectorOutput = outputValue;
    }

    public Vector2 PlayerJoystickOutputVector()
    {
        if (Input.touchCount == 1)
            return _playerTouchVectorOutput;
        else
            return new Vector2(0, 0);
    }

    public void OnPointerUp(PointerEventData _onPointerUpData)
    {
        OutputVectorValue(Vector2.zero);
        _isPlayerTouchingPanel = false;
    }

    public void OnPointerDown(PointerEventData _onPointerDownData)
    {
        if ( Input.touchCount == 1){
            OnDrag(_onPointerDownData);
            _touchID = _myTouch.fingerId;
            _isPlayerTouchingPanel = true;
        } else {
            _isPlayerTouchingPanel = false;
        }
        
    }

    public void OnDrag(PointerEventData _onDragData)
    {
        OutputVectorValue(new Vector2(_onDragData.delta.normalized.x, _onDragData.delta.normalized.y));
    }

}