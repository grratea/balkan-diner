using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class StoveManager : MonoBehaviour
{
    public static StoveManager instance { get; private set; }

    [SerializeField] private List<Stove> stoves = new List<Stove>();

    public IReadOnlyList<Stove> Stoves { get { return stoves; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            return;
        }
        instance = this;

        if (stoves.Count == 0)
        {
            stoves = FindObjectsByType<Stove>(FindObjectsSortMode.None).ToList();
        }
    }

    public Stove GetEmptyStove()
    {
        return stoves.FirstOrDefault(s => s.IsEmpty);
    }

    public bool HasEmptyStove { get {  return GetEmptyStove() != null; } }

    public void ResetForNewDay()
    {
        foreach(Stove s in stoves)
        {
            s.ResetForNewDay();
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
