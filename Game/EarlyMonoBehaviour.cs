using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Game;

/// <summary>
/// A MonoBehaviour base class that allows for early initialization.
/// </summary>
public abstract class EarlyMonoBehaviour(bool isPersistent = false) : EMonoBehaviour
{
    /// <summary>
    /// Root game object.
    /// </summary>
    public GameObject gameObject { get; private set; } = null!;
    private Transform? _parent;
    
    private MonoBehaviourProxy _monoBehaviourProxyLifeTimeObject = null!;
    
    private bool _userInitialized = false;
    private bool _hasCalledAwake = false;
    private bool _hasCalledStart = false;
    
    /// <summary>
    /// Initialize the GameObject and MonoBehaviour proxy.
    /// </summary>
    public void Initialize(Transform? parent = null, bool cloneParent = false)
    {
        if (_userInitialized)
            return;
        
        if (parent is not null)
            _parent = parent;
        
        _userInitialized = true;
        this.InternalInitialize();
    }
    
    private void InternalInitialize()
    {
        this._monoBehaviourProxyLifeTimeObject = this.NewProxyObject.AddComponent<MonoBehaviourProxy>();
        this._monoBehaviourProxyLifeTimeObject.Set(this, true);
        // Call CheckAwake(); since it was called by Unity before we could call Set(...);
        this.CheckAwake();
    }

    private GameObject NewProxyObject
    {
        get 
        {
            this.gameObject = new GameObject { name = this.GetType().Name };
            if (this._parent is not null)
                this.gameObject.transform.SetParent(this._parent);
            return this.gameObject;
        }
    }
    
    protected override bool CheckOnDestroy()
    {
        if (isPersistent)
            this.InternalInitialize();
        return !isPersistent;
    }

    private void CheckAwake()
    {
        if (this._hasCalledAwake) return;
        this._hasCalledAwake = true;
        this.Awake();
    }
    
    private void CheckStart()
    {
        if (this._hasCalledStart) return;
        this._hasCalledStart = true;
        this.Start();
    }
}