using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private const string IS_RUNNING = "IsRunnig";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool(IS_RUNNING, Player.Instance.IsRunnig());
        AdjustPlayerFacingDirection();
    }

    private void AdjustPlayerFacingDirection()
    {
        UnityEngine.Vector3 mousePos = GameInput.Instance.GetmousePosirion();
        UnityEngine.Vector3 playerPos = Player.Instance.GetPlayerScreenPosition();

        if (mousePos.x < playerPos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
