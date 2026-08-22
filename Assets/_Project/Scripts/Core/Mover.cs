using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mover : MonoBehaviour
{
    [Header("MOVEMENT")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float arriveThreshold = 0.05f;

    private Vector2 targetPosition;
    private bool isMoving;
    private Action onArrive;

    public bool IsMoving { get { return isMoving; } }

    // zove se mal mal
    void Update()
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

    private void Awake()
    {
        targetPosition = transform.position;
        // bez ovog targetPos bi bio (0, 0), pa bi igrac otisao tamo
        // ako je igrac npr. na donjem lijevom kutu, on ce i stajati tamo zbog ovog
    }

    public void MoveTo(Vector2 destination, Action onArrived)
    {
        targetPosition = destination;
        onArrive = onArrived;
        isMoving = true;
    }

    // ako gost postane ljut, pa ode
    public void Stop()
    {
        isMoving = false;
        onArrive = null;
    }

    // umjesto ispisivanja brojeva u konzoli
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (!isMoving) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetPosition, 0.2f);
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
