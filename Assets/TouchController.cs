using UnityEngine;

public class TouchController : MonoBehaviour
{

    private Vector2 _firstPressPos;
    private Vector2 _secondPressPos;
    private readonly float _minSwipeLength = 50f;
    private float _intervalDelay;
    private readonly float _delay = 0.2f;
    private ICharacterMotion _player;
    private int _tapCount;

    private void Start()
    {
        _player = gameObject.GetComponent<ICharacterMotion>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_intervalDelay > 0)
        {
            _intervalDelay -= Time.deltaTime;
        }
        else if (_intervalDelay <= 0 && _tapCount == 1)
        {
            _player.SingleTapAction();
            _tapCount = 0;
        }

        // If running on desktop
        if (Application.platform != RuntimePlatform.Android)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get began mouse position
                _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Get ended mouse position
                _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                OneTouchEvent(_firstPressPos, _secondPressPos);
            }
        }
        else
        {
            if (Input.touchCount == 1)
            {
                if (Input.touches.Length > 0)
                {
                    Touch t = Input.GetTouch(0);

                    // Get began touch position 
                    if (t.phase == TouchPhase.Began)
                    {
                        _firstPressPos = new Vector2(t.position.x, t.position.y);
                    }
                    
                    // Get ended touch position
                    if (t.phase == TouchPhase.Ended)
                    {
                        _secondPressPos = new Vector2(t.position.x, t.position.y);
                        OneTouchEvent(_firstPressPos, _secondPressPos);
                    }
                }
            }

            if (Input.touchCount == 2)
            {
                TwoTouchEvent();
            }
        }
    }

    private void OneTouchEvent(Vector2 firstPressPos, Vector2 secondPressPos)
    {
        Vector2 currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x,
            secondPressPos.y - firstPressPos.y);

        if (currentSwipe.magnitude < _minSwipeLength)
        {
            // Double Tap
            if (_intervalDelay > 0)
            {
                _player.DoubleTapAction();
                _tapCount = 0;
            }
            // Single Tap
            else
            {
                _intervalDelay = _delay;
                _tapCount += 1;
            }
            return;
        }

        currentSwipe.Normalize();

        // Swipe up
        if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
        {
            _player.SwipeUpAction();
        }
    }

    private void TwoTouchEvent()
    {
        _player.TwoTouchAction();
    }
}