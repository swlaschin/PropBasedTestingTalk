open System

#r "lib/Expecto.dll"
open Expecto
let expectoConfig = {defaultConfig with colour = Expecto.Logging.ColourLevel.Colour0}


let randInt =
    let r = System.Random()
    // avoid issues with overflow :)  Exercise for the reader!
    fun () -> r.Next(-10000,10000)

let propertyCheck property =
   // property has type: int -> int -> bool

    for _ in [1..100] do
        let x = randInt()
        let y = randInt()
        let result = property x y
        Expect.isTrue result (sprintf "failed for %i %i" x y)

// good implementation
//let add x y = x + y

// bad implementations
let add x y = x - y
//let add x y = x * y
//let add x y = 0

// ===================================
// commutative property
// ===================================

let commutativeProperty x y =
   let result1 = add x y
   let result2 = add y x
   result1 = result2


let commutivityTest =
    test "commutivity" {
        propertyCheck commutativeProperty
        }

runTests expectoConfig commutivityTest


// ===================================
// associative property
// ===================================

let adding1TwiceIsAdding2OnceProperty x y =
    let result1 = x |> add 1 |> add 1
    let result2 = x |> add 2
    result1 = result2

let associativityTest =
    test "associativity" {
        propertyCheck adding1TwiceIsAdding2OnceProperty
        }

runTests expectoConfig associativityTest


// ===================================
// identity property
// ===================================

let identityProperty x _ =
   let result1 = x |> add 0
   result1 = x

let identityTest =
    test "identity" {
        propertyCheck identityProperty
        }

runTests expectoConfig identityTest
