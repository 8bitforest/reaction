using NUnit.Framework;
using Reaction;
using UnityEngine;

public class RxnDictionaryTests
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
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.OnChanged(_gameObject, () => handlerCalled = true);
        rxnDictionary.Add(1, "value");

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnChangedInit()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.OnChangedInit(_gameObject, () => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnKeyChanged()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChanged(_gameObject, _ => handlerCalled = true);
        rxnDictionary[1] = "newValue";

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnKeyChangedInit()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChangedInit(_gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyChanged()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChanged(1, _gameObject, _ => handlerCalled = true);
        rxnDictionary[1] = "newValue";

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyChangedOtherKey()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChanged(2, _gameObject, _ => handlerCalled = true);
        rxnDictionary[1] = "newValue";

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyChangedInit()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChangedInit(1, _gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyChangedInitOtherKey()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyChangedInit(2, _gameObject, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnKeyAdded()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.OnKeyAdded(_gameObject, _ => handlerCalled = true);
        rxnDictionary.Add(1, "value");

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnKeyAddedInit()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyAddedInit(_gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyAdded()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.OnKeyAdded(1, _gameObject, _ => handlerCalled = true);
        rxnDictionary.Add(1, "value");

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyAddedOtherKey()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.OnKeyAdded(2, _gameObject, _ => handlerCalled = true);
        rxnDictionary.Add(1, "value");

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyAddedInit()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyAddedInit(1, _gameObject, _ => handlerCalled = true);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyAddedInitOtherKey()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyAddedInit(2, _gameObject, _ => handlerCalled = true);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void OnKeyRemoved()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyRemoved(_gameObject, _ => handlerCalled = true);
        rxnDictionary.Remove(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyRemoved()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyRemoved(1, _gameObject, _ => handlerCalled = true);
        rxnDictionary.Remove(1);

        Assert.IsTrue(handlerCalled);
    }

    [Test]
    public void OnSpecificKeyRemovedOtherKey()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        rxnDictionary.Add(1, "value");
        rxnDictionary.OnKeyRemoved(2, _gameObject, _ => handlerCalled = true);
        rxnDictionary.Remove(1);

        Assert.IsFalse(handlerCalled);
    }

    [Test]
    public void RemoveHandler()
    {
        var rxnDictionary = new RxnDictionary<int, string>();
        var handlerCalled = false;

        var handlerId = rxnDictionary.OnChanged(_gameObject, () => handlerCalled = true);
        rxnDictionary.RemoveHandler(handlerId);
        rxnDictionary.Add(1, "value");

        Assert.IsFalse(handlerCalled);
    }
}