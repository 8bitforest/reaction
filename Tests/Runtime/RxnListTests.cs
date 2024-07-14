using NUnit.Framework;
using Reaction;
using UnityEngine;

public class RxnListTests
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
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnChanged(_gameObject, () => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedInit()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnChangedInit(_gameObject, () => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnAdded()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnAdded(_gameObject, _ => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnAddedInit()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnAddedInit(_gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnAddedWhenConditionMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnAddedWhen(_gameObject, item => item > 0, _ => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnAddedWhenConditionNotMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnAddedWhen(_gameObject, item => item > 1, _ => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnAddedWhenInitConditionMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnAddedWhenInit(_gameObject, item => item == 1, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnAddedWhenInitConditionNotMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnAddedWhenInit(_gameObject, item => item > 1, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnItemAdded()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnItemAdded(1, _gameObject, _ => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnItemAddedOtherItem()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.OnItemAdded(2, _gameObject, _ => handlerCalled = true);
        rxnList.Add(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnItemAddedInit()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnItemAddedInit(1, _gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnItemAddedInitOtherItem()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnItemAddedInit(2, _gameObject, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnRemoved()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnRemoved(_gameObject, _ => handlerCalled = true);
        rxnList.Remove(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnRemovedWhenConditionMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnRemovedWhen(_gameObject, item => item == 1, _ => handlerCalled = true);
        rxnList.Remove(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnRemovedWhenConditionNotMet()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnRemovedWhen(_gameObject, item => item == 2, _ => handlerCalled = true);
        rxnList.Remove(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnItemRemoved()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnItemRemoved(1, _gameObject, _ => handlerCalled = true);
        rxnList.Remove(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnItemRemovedOtherItem()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        rxnList.Add(1);
        rxnList.OnItemRemoved(2, _gameObject, _ => handlerCalled = true);
        rxnList.Remove(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void RemoveHandler()
    {
        var rxnList = new RxnList<int>();
        var handlerCalled = false;

        var handlerId = rxnList.OnChanged(_gameObject, () => handlerCalled = true);
        rxnList.RemoveHandler(handlerId);
        rxnList.Add(1);

        Assert.IsFalse(handlerCalled);
    }
}