using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

// PRIVREMENA TEST SKRIPTA, obrisati kasnije
public class TableDebugClicker : MonoBehaviour
{
    private Camera cam;
    private PlayerController player;
    [SerializeField] private LayerMask interactableMask;

    private static readonly TableState[] cycle =
    {
        TableState.Free,
        TableState.Seated,
        TableState.Ordered,
        TableState.Served,
        TableState.Dirty,
    };

    private void Awake()
    {
        cam = Camera.main;
        player = FindFirstObjectByType<PlayerController>();
    }

    void Start()
    {
        foreach (var t in TableManager.instance.Tables)
            t.OnStateChanged += (table, st) => Debug.Log($"{table.name} -> {st}");
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Table t = GetTableUnderMouse();
            if (t != null)
            {
                player.Mover.MoveTo(t.ServePosition, () => Debug.Log($"Kod {t.name} sam"));
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Table t = GetTableUnderMouse();
            if (t != null)
            {
                t.SetState(NextInCycle(t.State));
            }
        }
    }

    private TableState NextInCycle(TableState current)
    {
        for (int i = 0; i  < cycle.Length; i++)
        {
            if (cycle[i] == current)
            {
                return cycle[(i + 1) % cycle.Length];
            }
        }

        return TableState.Free;
    }

    private Table GetTableUnderMouse()
    {
        Vector2 world = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // TU JE BIO PROBLEM
        // Collider2D hit = Physics2D.OverlapPoint(world);
        Collider2D hit = Physics2D.OverlapPoint(world, interactableMask);
        Debug.Log($"Klik na {world} | pogodak: {(hit != null ? hit.name : "NISTA")}");
        // ako postoji collision izmedu misa i stola vrati stol
        // TryGetComponent bolji od GetComponent jer ne alocira mem. kada nema komponente
        return hit != null && hit.TryGetComponent(out Table table) ? table : null; 
    }


    /*TRI TESTA KOJA MORA PROCI:
        - desni klik na stol - stanje i boja se mijenjaju - PASSED
        - lijevi klik na stol - player ode do kruga, log javi  - PASSED
        - lijevi klik na pod - player ode tamo - PASSED
     */
}
