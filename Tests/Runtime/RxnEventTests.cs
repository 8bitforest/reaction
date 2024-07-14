using NUnit.Framework;
using Reaction;
using UnityEngine;

public class RxnEventTests
{
    private GameObject _gameObject;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameObject);
    }

    [Test]
    public void InvokeCallsHandler()
    {
        var rxnEvent = new RxnEvent();
        var handlerCalled = false;

        rxnEvent.OnInvoked(_gameObject, () => handlerCalled = true);
        rxnEvent.Invoke();

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void RemoveHandler()
    {
        var rxnEvent = new RxnEvent();
        var handlerCalled = false;

        var handlerId = rxnEvent.OnInvoked(_gameObject, () => handlerCalled = true);
        rxnEvent.RemoveHandler(handlerId);
        rxnEvent.Invoke();

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void RemoveHandlers()
    {
        var rxnEvent = new RxnEvent();
        var handler1Called = false;
        var handler2Called = false;

        rxnEvent.OnInvoked(_gameObject, () => handler1Called = true);
        rxnEvent.OnInvoked(_gameObject, () => handler2Called = true);
        rxnEvent.RemoveHandlers(_gameObject);
        rxnEvent.Invoke();

        Assert.IsFalse(handler1Called);
        Assert.IsFalse(handler2Called);
    }

    [Test]
    public void DestroyedGameObjectIsCleanedUp()
    {
        var rxnEvent = new RxnEvent();
        var handlerCalled = false;

        rxnEvent.OnInvoked(_gameObject, () => handlerCalled = true);
        Object.DestroyImmediate(_gameObject);
        rxnEvent.Invoke();

        Assert.IsFalse(handlerCalled);
    }
}