using NUnit.Framework;
using Reaction;
using UnityEngine;

public class RxnValueTests
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
    public void OnChangedNewValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChanged(_gameObject, _ => handlerCalled = true);
        rxnValue.Set(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedInit()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedInit(_gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedWhenConditionMet()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedWhen(_gameObject, change => change.New > 0, _ => handlerCalled = true);
        rxnValue.Set(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedWhenConditionNotMet()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedWhen(_gameObject, change => change.New > 1, _ => handlerCalled = true);
        rxnValue.Set(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnChangedWhenInitConditionMet()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedWhenInit(_gameObject, change => change.New == 0, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedWhenInitConditionNotMet()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedWhenInit(_gameObject, change => change.New > 0, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnChangedToMatchingValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedTo(1, _gameObject, _ => handlerCalled = true);
        rxnValue.Set(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedToOtherValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedTo(2, _gameObject, _ => handlerCalled = true);
        rxnValue.Set(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnChangedToInitMatchingValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedToInit(0, _gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedToInitOtherValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        rxnValue.OnChangedToInit(1, _gameObject, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void WaitUntilValue()
    {
        var rxnValue = new RxnValue<int>(0);
        var task = rxnValue.WaitUntil(1);

        rxnValue.Set(1);

        Assert.IsTrue(task.IsCompleted);
    }

    [Test]
    public void RemoveHandler()
    {
        var rxnValue = new RxnValue<int>(0);
        var handlerCalled = false;

        var handlerId = rxnValue.OnChanged(_gameObject, _ => handlerCalled = true);
        rxnValue.RemoveHandler(handlerId);
        rxnValue.Set(1);

        Assert.IsFalse(handlerCalled);
    }
}