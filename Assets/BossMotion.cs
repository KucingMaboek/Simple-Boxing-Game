using UnityEngine;

public class BossMotion : MonoBehaviour, ICharacterMotion
{
    private Animator _animator;
    private int _punchIteration;
    private float _punchDelay;
    private float _punchCooldown;
    private bool _isBlocking;
    private float _blockDelay;

    public GameObject opponent;
    private ICharacterController _opponentController;

    // List of punch motion
    private readonly string[,] _punchMotion = new string[,]
    {
        {"punch1", "left"},
        {"punch2", "right"},
        {"punch3", "left"},
        {"punch4", "right"}
    };

    // List of combo motion
    private readonly string[] _comboPunchMotion = new string[]
    {
        "comboPunch1",
        "comboPunch2"
    };

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _opponentController = opponent.GetComponent<ICharacterController>();
    }

    private void Update()
    {
        CoolingDown();
    }

    private void CoolingDown()
    {
        if (_punchIteration > 0)
        {
            _punchDelay += Time.deltaTime;
            if (_punchDelay > _punchCooldown)
            {
                _animator.SetTrigger("reset");
                _punchIteration = 0;
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
    // Trigger issue, need to be revised
    private void Punch()
    {
        _animator.SetTrigger(_punchMotion[_punchIteration, 0]);
        _opponentController.DealAttack(_punchMotion[_punchIteration, 1]);
        _punchDelay = 0f;
        _punchCooldown = _animator.GetCurrentAnimatorStateInfo(0).length;

        // If iteration is lesser than punch motion list length
        if (_punchIteration < _punchMotion.GetLength(0)-1)
        {
            _punchIteration++;
        }
        // If iteration is more than punch motion list length
        else
        {
            _punchIteration = 0;
        }
    }

    // Do Combo Punch
    private void ComboPunch()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            // Do random combo punch motion in list
            _animator.SetTrigger(_comboPunchMotion[Random.Range(0, _comboPunchMotion.Length)]);
            _opponentController.DealAttack("middle");
        }
    }

    // Do Elbow Punch
    private void ElbowPunch()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _animator.SetTrigger("elbowPunch");
            _opponentController.DealAttack();
        }
    }

    // Block an attack
    private void ReadyFightIdle()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _animator.SetTrigger("idleBlock");
            _blockDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
        }
    }

    public void SingleTapAction()
    {
        Punch();
    }

    public void DoubleTapAction()
    {
        ComboPunch();
    }

    public void SwipeUpAction()
    {
        ElbowPunch();
    }

    public void TwoTouchAction()
    {
        _isBlocking = true;
        if (gameObject.tag.Equals("Player"))
        {
            ReadyFightIdle();
        }
    }

    public void DealAttackAction(string side)
    {
        if (_isBlocking)
        {
            if (side.Equals("left"))
            {
                _animator.SetTrigger("leftBlock");
                _blockDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
            }
            else if (side.Equals("right"))
            {
                _animator.SetTrigger("rightBlock");
                _blockDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
            }
            else
            {
                _animator.SetTrigger("middleBlock");
                _blockDelay = _animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
        else
        {
            if (side.Equals("left"))
            {
                _animator.SetTrigger("leftHit");
            }
            else if (side.Equals("right"))
            {
                _animator.SetTrigger("rightHit");
            }
            else
            {
                _animator.SetTrigger("middleHit");
            }
        }
    }
}