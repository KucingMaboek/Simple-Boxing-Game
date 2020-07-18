using UnityEngine;

public class EnemyAI : MonoBehaviour, ICharacterController
{
    [SerializeField] private float blockChance = 0.2f;
    private ICharacterMotion _motion;

    private void Start()
    {
        _motion = gameObject.GetComponent<ICharacterMotion>();
    }
    // Start is called before the first frame update


    public void DealAttack(string side = "middle")
    {
        if (Random.value < blockChance)
        {
            _motion.TwoTouchAction();
            _motion.DealAttackAction(side);
        }
        else
        {
            _motion.DealAttackAction(side);
        }
    }
}