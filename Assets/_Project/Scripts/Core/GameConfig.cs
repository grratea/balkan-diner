using UnityEngine;

// ova klasa samo sluzi kao poveznik, tj gdje se drze reference na assete
public class GameConfig : MonoBehaviour
{
    public static GameConfig instance { get; private set; }

    [SerializeField] private BalanceConfig balance;
    [SerializeField] private MenuSO menu;

    public static BalanceConfig Balance { get { return instance.balance; } }
    public static MenuSO Menu { get { return instance.menu; } }

    private void Awake()
    { 
        if (instance != null && instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
}
