open System

#r "lib/Expecto.dll"
#r "lib/Expecto.FsCheck.dll"
#r "lib/FsCheck.dll"
open Expecto
let expectoConfig = {defaultConfig with colour = Expecto.Logging.ColourLevel.Colour0}

(*
For FsCheck/Expecto integration,
see https://github.com/haf/expecto#property-based-tests
*)


// good implementation
let add x y = x + y

// bad implementations
//let add x y = x - y
//let add x y = x * y
//let add x y = 0

// ===================================
// commutative property
// ===================================

let commutativeProperty x y =
   let result1 = add x y
   let result2 = add y x
   result1 = result2

// Standalone FsCheck
FsCheck.Check.Quick commutativeProperty

// Expecto integration uses "testProperty"
let commutivityTest =
    testProperty "commutivity" commutativeProperty

runTests expectoConfig commutivityTest
(*
For FsCheck/Expecto integration,
see https://github.com/haf/expecto#property-based-tests
*)





// ===================================
// associative property
// ===================================

let adding1TwiceIsAdding2OnceProperty x y =
    let result1 = x |> add 1 |> add 1
    let result2 = x |> add 2
    result1 = result2

// Standalone FsCheck
FsCheck.Check.Quick adding1TwiceIsAdding2OnceProperty

// FsCheck within Expecto
let associativityTest =
    testProperty "associativity" adding1TwiceIsAdding2OnceProperty

runTests expectoConfig associativityTest

// We don't need the extra parameter any more!
let adding1TwiceIsAdding2OnceProperty2 x =
    let result1 = x |> add 1 |> add 1
    let result2 = x |> add 2
    result1 = result2

FsCheck.Check.Quick adding1TwiceIsAdding2OnceProperty2

// FsCheck within Expecto
let associativityTest2 =
    testProperty "associativity" adding1TwiceIsAdding2OnceProperty2

runTests expectoConfig associativityTest2


// ===================================
// identity property
// ===================================

/// Again, no need for the extra parameter
let identityProperty x =
   let result1 = x |> add 0
   result1 = x

// Standalone FsCheck
FsCheck.Check.Quick identityProperty

// FsCheck within Expecto
let identityTest =
    testProperty "identity" identityProperty

runTests expectoConfig identityTest

// ===================================
// Configuring FsCheck
// ===================================

let maxTest1000 = { FsCheckConfig.defaultConfig with maxTest = 1000 }

// Standalone FsCheck
FsCheck.Check.Quick identityProperty

// FsCheck within Expecto
let commutivityTest1000 =
    testPropertyWithConfig maxTest1000 "commutivity" commutativeProperty

runTests expectoConfig commutivityTest1000


// ===================================
// Combining properties
// ===================================

open FsCheck.PropOperators

let additionSpec =
    (commutativeProperty |@ "commutivity")
    //                   ^gives the function a label
    .&. (adding1TwiceIsAdding2OnceProperty2 |@ "associativity")
    .&. (identityProperty |@ "identity")


FsCheck.Check.Quick additionSpec

