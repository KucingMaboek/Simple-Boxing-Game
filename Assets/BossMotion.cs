using UnityEngine;

public class BossMotion : MonoBehaviour, ICharacterMotion
{
    private Animator _animator;
    private int _punchIteration;
    private bool _isBlocking;
    private float _blockDelay;
    private float _punchDelay;

    public GameObject opponent;
    private ICharacterController _opponentController;

    // List of punch motion
    private readonly string[,] _punchMotion = new string[,]
    {
        {"Left Punch", "Left"},
        {"Right Punch", "Right"},
        {"Left Hook", "Left"},
        {"Right Hook", "Right"}
    };

    // List of combo motion
    private readonly string[] _comboPunchMotion = new string[]
    {
        "Combo Punch 1",
        "Combo Punch 2"
    };

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _opponentController = opponent.GetComponent<ICharacterController>();
    }

    private void Update()
    {
        if (_punchIteration > 0)
        {
            _punchDelay -= Time.deltaTime;
            if (_punchDelay < 0)
            {
                _punchIteration = 0;
                _animator.SetInteger("punch", _punchIteration);
            }
        }
        if (_blockDelay > 0)
        {
            _blockDelay -= Time.deltaTime;
        }
        else
        {
            _isBlocking = false;
        }
    }

    // Do Punch
    private void Punch()
    {
        if (_punchIteration == 0 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // Play first motion from motion list
            _animator.Play(_punchMotion[0, 0]);
            _opponentController.DealAttack(_punchMotion[_punchIteration, 1]);
            _punchDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
        }
        else if (_punchDelay > 0)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_punchMotion[_punchIteration, 0]))
            {
                if (_punchIteration < _punchMotion.GetLength(0) - 1)
                {
                    _punchIteration++;
                    _animator.SetInteger("punch", _punchIteration);
                    _opponentController.DealAttack(_punchMotion[_punchIteration, 1]);
                    _punchDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
                }
            }
        }
    }

    private void Attack(string motion, string side = "Middle")
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _animator.Play(motion);
            _opponentController.DealAttack(side);
        }
    }

    public void SingleTapAction()
    {
        Punch();
    }

    public void DoubleTapAction()
    {
        Attack(_comboPunchMotion[Random.Range(0, _comboPunchMotion.Length)]);
    }

    public void SwipeUpAction()
    {
        Attack("Elbow Punch");
    }

    public void TwoTouchAction()
    {
        _isBlocking = true;
        if (gameObject.tag.Equals("Player"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _animator.Play("Ready Block Idle");
                _blockDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
    }

    public void DealAttackAction(string side)
    {
        // If at blocking state, block attack
        if (_isBlocking)
        {
            _animator.Play(side + " Block");
        }
        // If not in blocking state, hit
        else
        {
            _animator.Play(side + " Hit");
        }

        // Break Chain Punch
        _punchIteration = 0;
        _animator.SetInteger("punch", _punchIteration);
    }
}