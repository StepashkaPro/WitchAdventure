using UnityEngine;

public class DemonVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI _enemyAI;
    private Animator _animator;

    private const string IS_RUNNING = "IsRunning";
    private const string CHASING_ACCELIRATIONA = "ChasingAcceliration";

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        _animator.SetBool(IS_RUNNING, _enemyAI.IsRunnig);
        _animator.SetFloat(CHASING_ACCELIRATIONA, _enemyAI.GetRoamingAnimationSpeed);
    }
}
