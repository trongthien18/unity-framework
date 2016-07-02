using UnityEngine;
using System.Collections;

public class AutoReturnPool : MonoBehaviour
{
    [SerializeField]
    float timeDelayToReturnPool = 3;

    void Start()
    {

    }

    public void OnEnable()
    {
        CancelInvoke();
        Invoke("ReturnPool", timeDelayToReturnPool);
    }

    public void ReturnPool()
    {
        PoolManager.ReleaseObject(this.gameObject);
    }
}
