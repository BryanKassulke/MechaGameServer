using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState: IDisposable
{
    protected Player player;
    public abstract PlayerState HandleTransitions();
    public abstract void StateUpdate();
    public abstract void Enter();
    public abstract void Exit();

    private bool disposed = false;

    protected virtual void Dispose(bool _disposing) {
        if (!disposed) {
            if (_disposing) {
                player = null;
            }
            disposed = true;
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
