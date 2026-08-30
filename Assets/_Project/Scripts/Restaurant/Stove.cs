using TMPro;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Stove : MonoBehaviour
{
    [Header("SETTINGS")]
    [SerializeField] private Transform interactPoint;
    [SerializeField] private TextMeshPro debugLabel;
    [SerializeField] private Transform progressBar;

    [Header("ONLY FOR DEBUG")]
    [SerializeField] private StoveState state = StoveState.Empty;

    private Order currentOrder;
    // timeri
    private float cookTimer; // kolko je jos ostalo
    private float totalCookTime; 

    public StoveState State { get { return state; } }
    public Order CurrentOrder { get { return currentOrder; } }

    public bool IsEmpty { get { return state == StoveState.Empty; } }
    public bool IsReady { get { return state == StoveState.Ready;} }

    public Vector2 InteractPosition
    {
        get
        {
            if (interactPoint != null)
            {
                return (Vector2)interactPoint.position;
            }
            return (Vector2)transform.position;
        }
    }

    public event Action<Stove> OnCookingFinished;

    void Start()
    {
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        RefreshLabel();
        RefreshProgressBar();
    }

    private void RefreshLabel()
    {
        if (debugLabel == null)
        {
            return;
        }

        if (state == StoveState.Cooking && currentOrder != null)
        {
            debugLabel.text = $"{currentOrder.Dish.displayName}\n{cookTimer:F1}s";
            debugLabel.color = new Color(1f, 0.5f, 0f);
        }

        else if (state == StoveState.Ready && currentOrder != null)
        {
            debugLabel.text = $"{currentOrder.Dish.displayName}\nDONE";
            debugLabel.color = Color.green;
        }

        else
        {
            debugLabel.text = "EMPTY";
            debugLabel.color = Color.grey;
        }
    }

    private void RefreshProgressBar()
    {
        if (progressBar == null) 
        {
            return;
        }

        bool show = (state == StoveState.Cooking);
        progressBar.gameObject.SetActive(show);

        if (!show)
        {
            return;
        }

        float progress = 1f - (cookTimer / totalCookTime);
        Vector3 scale = progressBar.localScale;
        scale.x = Mathf.Clamp01(progress); // raspon je ogranicen na [0, 1]
        progressBar.localScale = scale;
    }

    void Update()
    {
        // timer se racuna samo kada se kuha
        if (state != StoveState.Cooking)
        {
            return;
        }

        cookTimer -= Time.deltaTime;

        if (cookTimer <= 0f)
        {
            FinishCooking();
        }

        RefreshVisuals(); // zbog progress bara koji se kontinuirano mijenja
    }

    public bool StartCooking(Order order)
    {
        if (state != StoveState.Empty || order == null)
        {
            return false;
        }

        currentOrder = order;
        totalCookTime = order.Dish.cookTime;
        cookTimer = totalCookTime;

        SetState(StoveState.Cooking);

        return true;
    }

    public void SetState(StoveState newState) 
    {
        if (state == newState) 
        {
            return;
        }

        state = newState;
        RefreshVisuals();
    }

    public void FinishCooking()
    {
        cookTimer = 0f;
        currentOrder.IsReady = true;
        SetState(StoveState.Ready);

        OrderManager.instance.NotifyOrdersChanged();

        OnCookingFinished?.Invoke(this);
    }

    public Order TakeDish()
    {
        if (state != StoveState.Ready)
        {
            return null;
        }

        Order order = currentOrder;

        currentOrder = null;
        SetState(StoveState.Empty);

        return order;
    }

    public void ResetForNewDay()
    {
        currentOrder = null;
        cookTimer = 0f;
        totalCookTime = 0f;
        SetState(StoveState.Empty);
        RefreshVisuals();
    }

    private void OnDrawGizmos()
    {
        if (interactPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(interactPoint.position, 0.2f);
        }
    }
}
