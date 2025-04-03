using UnityEngine;

namespace Shared.Game;

/// <summary>
/// A MonoBehaviour base class that allows for early initialization.
/// </summary>
public abstract class EarlyMonoBehaviour(bool isPersistent = false)
{
    protected virtual void Awake() { }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }
    protected virtual void OnDestroy() { }
    
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
        this._monoBehaviourProxyLifeTimeObject.Set(CheckOnDestroy, CheckAwake, CheckStart, Update, FixedUpdate);
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
    
    private void CheckOnDestroy()
    {
        if (isPersistent)
            this.InternalInitialize();
        else
            this.OnDestroy();
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