using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Game;

public abstract class EMonoBehaviour : Object
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    protected virtual void OnDestroy() { }
    protected virtual bool CheckOnDestroy() => true;
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected Coroutine StartCoroutine(IEnumerator routine) => _proxy.InvokeStartCoroutine(routine);
    protected void StopCoroutine(IEnumerator routine) => _proxy.InvokeStopCoroutine(routine);
    protected void StopAllCoroutines() => _proxy.InvokeStopAllCoroutines();

    private MonoBehaviourProxy _proxy = null!;
    internal void Set(MonoBehaviourProxy proxy)
    {
        _proxy = proxy;
    }

    private void DestroyRunner()
    {
        if (this.CheckOnDestroy())
            this.OnDestroy();
    }
    internal virtual void InvokeTarget(EInvokeTarget target, object? param = null)
    {
        var action = target switch
        {
            EInvokeTarget.Awake => this.Awake,
            EInvokeTarget.Start => this.Start,
            EInvokeTarget.Update => this.Update,
            EInvokeTarget.FixedUpdate => this.FixedUpdate,
            EInvokeTarget.OnDestroy => this.DestroyRunner,
            EInvokeTarget.OnEnable => this.OnEnable,
            EInvokeTarget.OnDisable => (Action)this.OnDisable,
            _ => null
        };

        action?.Invoke();
    }
}