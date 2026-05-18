using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FairyTales.Layer;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class PlayerState_Move : PlayerStateBase
{
    public override PlayerState ThisEnum => PlayerState.Move;

    public PlayerState_Move()
    {
        OnUpdate = Update;
        OnFixedUpdate = FixedUpdate;

        OnDashButtonDown = DashButtonDown;
        OnDamaged = Damaged;
    }

    private void DashButtonDown()
    {
        if (_currentInteractableObject)
        {
            _currentInteractableObject.Interact();
            return;
        }

        if (Player.IsDashCooldown)
        {
            return;
        }

        Player.ChangeState(PlayerState.Dash);
    }

    private void Update()
    {
        CheckInteractableObject();
        Player.SetMoveAnimation(Player.LastInputDir);
    }


    private static void FixedUpdate()
    {
        Player.currentMovementSpeed = Player.CurrentInputDir == Vector2.zero
            ? Mathf.Lerp(Player.currentMovementSpeed, 0f, Player.DeaccelerationSpeed)
            : Mathf.Lerp(Player.currentMovementSpeed, Player.MaximumMovementSpeed, Player.AccelerationSpeed);
        Player.MainRigidbody2D.linearVelocity = Player.velocity = Player.LastInputDir * Player.currentMovementSpeed;
    }


    private static void Damaged(Vector2 knockbackDir)
    {
        Player.SetReflectionDirection(knockbackDir);
        Player.ChangeState(PlayerState.Damaged);
    }

    #region 상호작용
    
    private InteractableObject _currentInteractableObject;
    private Collider2D _currentInteractedCollider;
    private readonly List<Collider2D> _interactableColliders = new(); // 주변 상호작용 가능한 콜라이더 담는 용도
    private readonly ContactFilter2D _interactionFilter = new()
    {
        layerMask = LayerCache.GetLayerMask(LayerName.Object),
        useLayerMask = true,
    };
    private const float InteractionRange = 0.75f;


    
    /// 상호작용 가능한 오브젝트 체크
    private void CheckInteractableObject()
    {
        var numOfInteracted = Physics2D.OverlapCircle(Player.GetFlyingPosition(),
            InteractionRange, _interactionFilter, _interactableColliders);
        
        if (numOfInteracted != 0)
        {
            var closed = _interactableColliders
                .OrderBy(collider2D => (collider2D.transform.position - Player.GetFlyingPosition()).sqrMagnitude)
                .First();

            if (_currentInteractedCollider != closed)
            {
                if (_currentInteractedCollider)
                {
                    _currentInteractableObject.StopCheckInteractable();
                }
                _currentInteractedCollider = closed;
                _currentInteractableObject = _currentInteractedCollider.GetComponent<InteractableObject>();
                _currentInteractableObject.CheckInteractable().Forget();
            }
        }
        else
        {
            if (_currentInteractedCollider)
            {
                _currentInteractableObject.StopCheckInteractable();
                _currentInteractableObject = null;
                _currentInteractedCollider = null;
            }
        }
    }

    #endregion

    
}