// Choosing properties -- some examples

open System

#r "lib/Expecto.dll"
#r "lib/Expecto.FsCheck.dll"
#r "lib/FsCheck.dll"
open FsCheck
open Expecto
let expectoConfig = {defaultConfig with colour = Expecto.Logging.ColourLevel.Colour0}

#load "utils.fsx"
open Utils


let fizzBuzz n =
    let (|DivBy|_|) divisor n =
        if n % divisor = 0 then Some DivBy else None
    match n with
    | DivBy 3 & DivBy 5 -> "FizzBuzz"
    | DivBy 3 -> "Fizz"
    | DivBy 5 -> "Buzz"
    | _ -> string n

// test!
// [1..50] |> List.map fizzBuzz


module BadFizzBuzzProperty =


    let prop n =
        let actual = fizzBuzz n

        // just reimplementing it :(
        let expected =
            if n % 15 = 0 then "FizzBuzz"
            elif n % 3 = 0 then "Fizz"
            elif n % 5 = 0 then "Buzz"
            else string n

        actual = expected

    Check.Quick prop

// =======================================
// Oracle: good for comparing two different implementations
// Not the same as example-based testing!
// =======================================

module ListOracle =

    let rec mySort list =
        match list with
        | [] -> []
        | [x] -> [x]
        | head::tail ->
            let smaller, larger = tail |> List.partition (fun e -> e < head)
            smaller @ [head] @ larger  // deliberate bug!

    let librarySort list =
        List.sort list

    let prop (list:int list) =
        let list1 = mySort list
        let list2 = librarySort list
        list1 = list2

    Check.Quick prop


module FizzBuzzOracle =

    type FizzResult =
        | Carbonated of string
        | Uncarbonated of int

    let carbonate label divisor n =
        if n % divisor = 0 then
            Carbonated label
        else
            Uncarbonated n

    let orElse f result =
        match result with
        | Carbonated label -> Carbonated label
        | Uncarbonated n ->  f n

    let monadicFizzBuzz n =
        n
        |> carbonate "FizzBuzz" 15
        |> orElse (carbonate "Fizz" 3)
        |> orElse (carbonate "Buzz" 5)
        |> function
            | Carbonated label -> label
            | Uncarbonated n ->  string n

    let prop n =
        let s1 = fizzBuzz n
        let s2 = monadicFizzBuzz n
        s1 = s2

    Check.Quick prop



// =======================================
// Commutativity: "Different paths, same destination"
// =======================================

module ListSortCommutativity =

    let sort list = List.sort list
    //let sort list = [] // evil sort

    let prop list =
        let negate list = list |> List.map (fun i -> -i)

        let l1 = list |> sort |> negate // what am I missing?
        let l2 = list |> negate |> sort
        l1 = l2

    Check.Quick prop

module ListRevCommutativity =

    let prop list anyValue =
        let appendThenReverse = (list @ [anyValue]) |> List.rev
        let reverseThenPrepend = anyValue :: (list |> List.rev)
        appendThenReverse = reverseThenPrepend

    Check.Quick prop

module FizzBuzzCommutativity =

    // multiplying by a coprime number
    // should have the same result
    let prop n =
        let s1 = n |> fizzBuzz
        let s2 = n * 7 |> fizzBuzz
        s1 = s2

    Check.Quick prop  // fails :(



// a better version
module FizzBuzzCommutativity2 =

    let fizzBuzzOpt n =
        let (|DivBy|_|) divisor n =
            if n % divisor = 0 then Some DivBy else None
        match n with
        | DivBy 3 & DivBy 5 -> Some "FizzBuzz"
        | DivBy 3 -> Some "Fizz"
        | DivBy 5 -> Some "Buzz"
        | _ -> None

    let fizzBuzz n =
        n |> fizzBuzzOpt |> Option.defaultValue (string n)

    let prop n =
        let s1 = n |> fizzBuzzOpt
        let s2 = n * 7 |> fizzBuzzOpt
        s1 = s2

    Check.Quick prop

(*
// I claim this version of FizzBuzz is better because it is more versatile
// You can do the original
[1..50]
|> List.map fizzBuzz

// but also you can do...
[1..50]
|> List.choose (fun i -> i |> fizzBuzzOpt |> Option.map (fun s -> i,s))

*)


// =======================================
// Inverses: "There and back again"
// =======================================

module ListRevInverse =

    let prop list =
        let twiceReversed = list |> List.rev |> List.rev
        twiceReversed = list

    Check.Quick prop

module AdditionInverse =

    let add x y = x + y

    let prop x y =
        let actual = y |> add x |> add -x
        actual = y

    Check.Quick prop

// =======================================
// Verification: "Hard to prove, easy to verify"
// =======================================

module FizzBuzzVerification =

    let prop n =
        let str = fizzBuzz n

        ["Fizz"; "Buzz"; "FizzBuzz"; string n]
        |> List.contains str

    Check.Quick prop

module CsvParser =

    // code to test
    let parseCsv (str:string) =
        str.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)

    // Construct a string so that we know what
    // the results should be in advance
    let prop (strings:string[]) =
        let inputStr = strings |> String.concat ","
        let actual = parseCsv inputStr

        let expected =
            strings
            |> Array.filter (String.IsNullOrEmpty >> not)
        actual = expected

    Check.Quick prop


    // ------------------------------
    // corrected version
    // ------------------------------

    type CustomArbs =
        // build a custom generator of string[] just for this test
        static member StringArray() =
            Arb.generate<string>
            |> Gen.filter (fun s -> not (s.Contains(",")))
            |> Gen.arrayOf
            |> Arb.fromGen

    let config = { FsCheck.Config.Default with Arbitrary = [typeof<CustomArbs>] }

    Check.One(config,prop)  // still fails! Why?


module ListSortVerification =

    let sort list = List.sort list
    //let sort list = [] // evil sort

    let adjacentPairsShouldBeOrdered (list:int list) =
        let pairs = list |> sort |> Seq.pairwise
        pairs |> Seq.forall (fun (x,y) -> x <= y )

    Check.Quick adjacentPairsShouldBeOrdered


    // but evilSort still passes

    let listShouldBeSameLength (list:int list) =
        let sortedList = list |> sort
        List.length sortedList = List.length list

    Check.Quick (adjacentPairsShouldBeOrdered .&. listShouldBeSameLength)

    // another evil sort
(*
    let sort list =
        match list with
        | [] -> []
        | head::_ -> List.replicate (List.length list) head
*)


    let listShouldHaveSameContents (list:int list) =
        let sortedList = list |> sort
        sortedList |> isPermutationOf list

    Check.Quick (adjacentPairsShouldBeOrdered .&. listShouldHaveSameContents)

