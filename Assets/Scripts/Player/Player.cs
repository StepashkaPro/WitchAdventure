using System.Numerics;
using UnityEditor.Callbacks;
using UnityEngine;


[SelectionBase]
public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }
    [SerializeField] private float movingSpeed = 10f;

    private Rigidbody2D rb;
    
    private float minMovementSpeed = 0.1f;

    private bool isRunnig = false;

    private void Awake() {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e) {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        UnityEngine.Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > minMovementSpeed || Mathf.Abs(inputVector.y) > minMovementSpeed) {
            isRunnig = true;
        } else {
            isRunnig = false;
        }
    }

    public bool IsRunnig() {
        return isRunnig;
    }

    public UnityEngine.Vector3 GetPlayerScreenPosition()
    {
        UnityEngine.Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}
