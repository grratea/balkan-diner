using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float arriveThreshold = 0.05f;
    // dodan zbog float aritmetike, pozicija se pomice u malim koracima i gotovo nikad nece biti tocno jednaka cilju

    [Header("CLICK")]
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private LayerMask interactableMask;
    // to je var int koji svaki bit predstavlja jedan layer

    private Vector2 targetPosition;
    private bool isMoving;
    private Action onArrive;
    // ugradeni C# delegat, var koja drzi fju
    // sto napraviti kada stigne, omogucuje da klasa PC ne zna za stolove, kuhinju, goste, itd

    private Camera cam;

    public bool IsMoving => isMoving; // getter

    private void Awake()
    {
        cam = Camera.main;
        targetPosition = transform.position;
        // bez ovog targetPos bi bio (0, 0), pa bi igrac otisao tamo
        // ako je igrac npr. na donjem lijevom kutu, on ce i stajati tamo zbog ovog
    }

    void Start()
    {
        
    }

    void Update()
    {
        HandleClick();
        HandleMovement();
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
            MoveTo(worldPos, null);
            //MoveTo(worldPos, () => Debug.Log("TU SAM"));
        }

    }

    public void MoveTo(Vector2 destination, Action onArrived)
    {
        targetPosition = destination;
        onArrive = onArrived;
        isMoving = true;
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        // vrijeme proteklo od proslog framea, 4 UNITA po sekundi, a ne po frameu
        // MoveTowards NIKAD ne prebaci vrijednost (tj ne prode target), nije bitan smjer, ni normalizacija vektora
        // max distanca je po FRAMEU?? nisam 100%, ubiti nekad to moze se odraditi u vise manje frameova, npr ako je stol blizi
        // zato fja ima ugradeni stop, ne mora se provjeravati je li presao limit

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        // transform.position = Vector2.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) <= arriveThreshold)
        {
            isMoving = false;

            // ZASTITA 
            // kad stignes do stola, idi u kuhinju
            Action callback = onArrive;
            onArrive = null;
            callback?.Invoke(); // ? omogucava da ako je varijabla null DA SE NE zove
        }

        // MoveTowards je bolji od Lerpa jer je konst i stvarno stigne,
        // a Lerp usporava kako dolazi blizu cilju i nikad ne stigne
    }

    // umjesto ispisivanja brojeva u konzoli
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
    }
}
