namespace quickcheck
open System
open System.Collections
open NUnit.Framework
open NUnit.Framework.Constraints
open NUnit.Framework.SyntaxHelpers
open UnityEngine
open KSPPP

[<TestFixture>]
type TestColor() = 

    [<Test>]
    let NewColorIsColor() =
        let ex1 = new Color (0.0f,0.0f,0.0f,1.0f)
        let ex2 = Color.black
        Assert.AreEqual(ex1,ex2)
    
    [<Test>]
    let NewColorManualAssignment() =
        let mutable ex1 = new Color()
        ex1.r <- 1.0f
        ex1.g <- 1.0f
        ex1.b <- 1.0f
        ex1.a <- 1.0f
        let ex2 = Color.white
        Assert.AreEqual(ex1,ex2)

    [<Test>]
    let Color32ToColorCast() =
        let ex1 = new Color32(byte 255,byte 255,byte 255,byte 255)
        let ex2 = new Color(1.0f,1.0f,1.0f,1.0f)
        let ex3 = Color32.op_Implicit(ex2)
        Assert.AreEqual(ex1,ex3)

    [<Test>]
    let ColorToColor32Cast() =
        let ex1 = new Color(0.0f,0.0f,0.0f,1.0f)
        let ex2 = new Color32(byte 0,byte 0,byte 0,byte 255)
        let ex3 = Color32.op_Implicit(ex2);
        Assert.AreEqual(ex1,ex3)


    [<Test>]
    let ColorToHSVToColorWhite() =
        let ex1 = Color.white
        let ex2 = ColorModels.HSVA(ex1)
        let ex3 = (ex2.toColor(ex2))
        Assert.AreEqual(ex1,ex3)

    [<Test>]
    let ColorToHSVToColorBlack() =
        let ex1 = Color.black
        let ex2 = ColorModels.HSVA(ex1)
        let ex3 = (ex2.toColor(ex2))
        Assert.AreEqual(ex1,ex3)

    [<Test>]
    let ColorToHSVToColorGrey() =
        let ex1 = Color.grey
        let ex2 = ColorModels.HSVA(ex1)
        let ex3 = (ex2.toColor(ex2))
        Assert.AreEqual(ex1,ex3)

    [<Test>]
    let LerpBlackToWhiteIsGrey() =
        let black = ColorModels.HSVA(Color.black)
        let white = ColorModels.HSVA(Color.white)
        let gray = ColorModels.HSVA(Color.gray)
        let mid = ColorModels.HSVA.Lerp(black,white,0.5f)
        Assert.AreEqual(mid,gray)

    [<Test>]
    let LerpColor() =
        let black = Color.black
        let white = Color.white
        let gray = Color.gray
        let mid = Color.Lerp(black,white,0.5f)
        Assert.AreEqual(mid,gray)

    let blackVblack() =
        let black = Color.black
        let hsv_black = ColorModels.HSVA(Color.black)
        Assert.AreEqual(black,hsv_black)
    [<Test>]
    let whiteVwhite() =
        let white = Color.white
        let hsv_white = ColorModels.HSVA(Color.white)
        Assert.AreEqual(white,hsv_white.toColor(hsv_white))
    [<Test>]
    let grayVgray() =
        let gray = Color.gray
        let hsv_gray = ColorModels.HSVA(Color.gray)
        Assert.AreEqual(gray ,hsv_gray.toColor(hsv_gray))
    [<Test>]
    let ColorIndexOOB() =
        let ex1 = Color.green
        try 
           do ex1.[4] = 1.0f |> ignore
        with
        | :? System.IndexOutOfRangeException -> Assert.IsTrue(true);
        | _ -> Assert.Fail()


