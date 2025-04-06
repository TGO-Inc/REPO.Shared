using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Game;

public sealed class MonoBehaviourProxy : MonoBehaviour
{
    private EMonoBehaviour? _router;
    
    internal void Set(EMonoBehaviour router, bool liveLong)
    {
        _router = router;
        _router.Set(this);

        if (!liveLong) return;
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnDestroy() => _router?.InvokeTarget(EInvokeTarget.OnDestroy);
    private void Awake() => _router?.InvokeTarget(EInvokeTarget.Awake);
    private void Start() => _router?.InvokeTarget(EInvokeTarget.Start);
    private void Update() => _router?.InvokeTarget(EInvokeTarget.Update);
    private void FixedUpdate() => _router?.InvokeTarget(EInvokeTarget.FixedUpdate);
    private void OnEnable() => _router?.InvokeTarget(EInvokeTarget.OnEnable);
    private void OnDisable() => _router?.InvokeTarget(EInvokeTarget.OnDisable);
    
    internal Coroutine InvokeStartCoroutine(IEnumerator routine) => StartCoroutine(routine);
    internal void InvokeStopCoroutine(IEnumerator routine) => StopCoroutine(routine);
    internal void InvokeStopAllCoroutines() => StopAllCoroutines();
    internal void InvokeDestroy(Object obj) => Destroy(obj);
    internal void InvokeDontDestroyOnLoad(Object obj) => DontDestroyOnLoad(obj);
}