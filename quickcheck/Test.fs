namespace quickcheck
open System
open System.Collections
open NUnit.Framework
open NUnit.Framework.Constraints
open NUnit.Framework.SyntaxHelpers
open UnityEngine
open KSPPP

module Tests =

    let (~~) (x:obj) = 
        match x with
        | :? 't as t -> t //'
        | _ -> null


    [<Test>]
    let IsNull() =
        let nada : obj = null
        Assert.IsNull(nada)
        Assert.That(nada, Is.Null)

    [<Test>]
    let IsNotNull() =
        Assert.IsNotNull(42)
        Assert.That(42, Is.Not.Null)

    [<Test>]
    let IsTrue() =
        Assert.IsTrue(2+2=4)
        Assert.That(2+2=4, Is.True)
        Assert.That(2+2=4)

    [<Test>]
    let IsFalse() =
        Assert.IsFalse(2+2=5)
        Assert.That(2+2=5, Is.False)

    [<Test>]
    let IsNaN() =
        let d : double = Double.NaN
        let f : float = Double.NaN
        Assert.IsNaN(d)
        Assert.IsNaN(f)
        Assert.That(d, Is.NaN)
        Assert.That(f, Is.NaN)