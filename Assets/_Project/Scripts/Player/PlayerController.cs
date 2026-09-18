using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Mover))] // automatski povezuje u Unity-u
public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask interactableMask;

    private Camera cam;
    private Mover mover;

    public Mover Mover { get { return mover; } }

    private void Awake()
    {
        cam = Camera.main;
        mover = GetComponent<Mover>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (!(Mouse.current.leftButton.wasPressedThisFrame)) {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // mis daje piksele
        Vector2 screenPos = Mouse.current.position.ReadValue();
        // cam prevede piksele u unit
        Vector2 worldPos = cam.ScreenToWorldPoint(screenPos);

        // vraca prvi collider na toj tocki, ali gleda SAMO layere iz maske
        // zato klik na zid ne radi nista, puno bolje od raycasta (tutoriala)

        Collider2D interactable = Physics2D.OverlapPoint(worldPos, interactableMask);
        if (interactable != null)
        {
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(worldPos, floorMask);

        if (hit != null)
        {
            // zove fju da spremi podatke koji ce biti ready for move
            // null zato sto ne poziva druge klase
            mover.MoveTo(worldPos, null);
            //MoveTo(worldPos, () => Debug.Log("TU SAM"));
        }
    }
}
