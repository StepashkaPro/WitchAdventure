using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR.Haptics;
using WitchAdventure.Utils;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State _startingState; // Starting state of the enemy
    [SerializeField] private float _roamingDistanceMax = 7f; // Max distanse that enemy can culculate his path while roaming
    [SerializeField] private float _roamingDistanceMin = 2f; // Min distanse that enemy can culculate his path wgile roaming
    [SerializeField] private float _roamingTimerMax = 2f; // Time between recalculations the path of the enemy

    [SerializeField] private bool _isChasingAnemy = false; // Returns thue / false depence is the enemy need to chase character to attack him based on enemy attack type
    [SerializeField] private float _chasingDistanse = 4f; // Distanse that character need approach to the enemy for start chasing enemy chasing animation
    [SerializeField] private float _chasingAcceliration = 2f; // Acceliration of the enemy speed while chasing the character

    [SerializeField] private bool _isAttackingEnemy = false; // Returns type of the enemy (Attaking / pisfull)
    [SerializeField] private float _AttackDistance = 2f; // Distance that enemy need approach to the character to start attack animation
    [SerializeField] private float _attackRate = 2f; // Reload of enemy attack
    private float _nexAttackTime = 0; // Time for the nex enemy attack
    

    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTime;
    private Vector3 _roamingPosition;
    private Vector3 _startingPosition;

    private float _roamingSpeed;
    private float _chasingSpeed;

    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public event EventHandler OnEnemyAttack;

    

    public bool IsRunnig {
        get {
            if (_navMeshAgent.velocity == Vector3.zero){
                return false;
            }
            return true;
        }
    }

    public float GetRoamingAnimationSpeed {
        get {
            return _navMeshAgent.speed / _roamingSpeed;
        }
    }



    private enum State // Добавить логику для анимации Idle и переключения между Idle -> Roaming !!!
    {
        Idle,
        Roaming, 
        Chasing,
        Attack,
        Death
    }

    private void Start() {
        _startingPosition = transform.position;
    }

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = _startingState;

        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingAcceliration;
    }

    private void Update() {
        StateHeadler();
        MovementDirectionHandler();
    }


    private void StateHeadler() { // Controls enemy actions base on his current state
        switch (_currentState) {
            case State.Roaming:
                _roamingTime -= Time.deltaTime;
                if (_roamingTime <= 0)
                {
                    Roaming();
                    _roamingTime = _roamingDistanceMax;
                }
                CheckCurrentState();
                break;
            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attack:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break; 
        }
    }

    private void ChasingTarget() {
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    private void MovementDirectionHandler() {

        if (IsRunnig) {
            ChageFacingDitection(_lastPosition, transform.position);
        }
        else if (_currentState == State.Attack) {
            ChageFacingDitection(transform.position, Player.Instance.transform.position);
        }
        
        _lastPosition = transform.position;
    }

    private void CheckCurrentState() {
        float distanseToPlaer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (_isChasingAnemy) {
            if (distanseToPlaer <= _chasingDistanse) {
                newState = State.Chasing;
            }
        }

        if (_isAttackingEnemy) {
            if (distanseToPlaer <= _AttackDistance) {
                newState = State.Attack;
            }
        }

        if (newState != _currentState) {
            if (newState == State.Chasing) {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;
            }   
            else if (newState == State.Roaming) {
                _roamingTime = _roamingTimerMax;
                _navMeshAgent.speed = _roamingSpeed;
            }
            else if (newState == State.Attack) {
                _navMeshAgent.ResetPath();
            }

            _currentState = newState;
        }
    }

    private void AttackingTarget() {
        if (Time.time > _nexAttackTime) {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);

            _nexAttackTime = Time.time + _attackRate;
        }
    }

    private void Roaming() {
        _startingPosition = transform.position;
        _roamingPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamingPosition);
    }

    private Vector3 GetRoamingPosition() {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }

    private void ChageFacingDitection(Vector3 sourcePosition, Vector3 targetPosition) {
        if (sourcePosition.x > targetPosition.x) {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        } else {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
