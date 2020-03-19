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

// ===================================
// Shrinking
// ===================================
open FsCheck

let smallerThan81Property x =
   x < 81

FsCheck.Check.Quick smallerThan81Property

Arb.shrink 100 |> Seq.toList
Arb.shrink 88 |> Seq.toList

// ===================================
// Low level generators
// See https://fscheck.github.io/FsCheck/TestData.html
// ===================================


// Gen.choose -- integer values between a minimum and maximum
// Also there is a "gen" computation expression
let genPair  =
    gen {
      let! i = Gen.choose (0, 100)
      let! j = Gen.choose (0, 100)
      return i,j
    }

// Gen.sample -- builds a sample list from a generator (for troubleshooting)
genPair |> Gen.sample 0 10
//                    ^size ^count

// Gen.elements -- drawn from a collection of possible values.
Gen.elements [42; 1337; 7; -100; 1453; -273] |> Gen.sample 0 10

// Gen.shuffle -- shuffle a list each time
Gen.shuffle ["a"; "b"; "c"; "d"] |> Gen.sample 0 6

// Gen.map -- transform item
Gen.choose (1,10)
|> Gen.map (fun i -> i.ToString() )
|> Gen.sample 0 10

// Gen.filter -- filter items
let evens =
    Gen.choose (1,10)
    |> Gen.filter (fun i -> i % 2 = 0)
evens |> Gen.sample 0 10

// Note: Throwing away 50% of the items is bad.
// Filters should only be used rarely -- better to *construct* than discard.
let evens2 =
    Gen.choose (1,5)
    |> Gen.map (fun i -> i * 2)

evens2 |> Gen.sample 0 10

// ===================================
// Arbitraries
// ===================================

let intArb = Arb.from<int>
intArb.Generator |> Gen.sample 0 10
intArb.Shrinker 100

// ===================================
// The importance of size
// ===================================

let intGen = Arb.generate<int>
intGen |> Gen.sample 0 10     // all 0
intGen |> Gen.sample 100 10   // change the size to 10

let stringGen = Arb.generate<string>
stringGen |> Gen.sample 0 10  // all empty or null
stringGen |> Gen.sample 10 10 // change the size to 10

let listGen = Arb.generate<int list>
listGen |> Gen.sample 0 10   // all empty
listGen |> Gen.sample 10 10  // change the size to 10

let optionGen = Arb.generate<int option>
optionGen |> Gen.sample 0 10  // All Some 0
optionGen |> Gen.sample 10 10 // change the size to 10

let tupleGen = Arb.generate<int * string>
tupleGen |> Gen.sample 0 10
tupleGen |> Gen.sample 10 10

// ===================================
// Combining Generators
// ===================================

// let intGen = Arb.generate<int>  // defined above

let twoInt = Gen.two intGen
twoInt |> Gen.sample 10 10

let threeInt = Gen.three intGen
threeInt  |> Gen.sample 10 10

// generating lists

let listOfInt = Gen.listOf intGen
listOfInt |> Gen.sample 0 10  // lots of empty lists
listOfInt |> Gen.sample 10 10 // up to size 10

Gen.listOfLength 4 intGen |> Gen.sample 0 10  // all zeros
Gen.listOfLength 4 intGen |> Gen.sample 10 10

Gen.nonEmptyListOf intGen |> Gen.sample 10 10

// ===================================
// Custom Generators
// ===================================

// Generate a CSV string with 10 fields
let csvGen =
    stringGen
    |> Gen.listOfLength 10
    |> Gen.map (fun strings -> String.concat "," strings)

csvGen |> Gen.sample 10 10

// Generate a custom temperature type
// uses Gen.oneof to choose between generators at random
type Temp = F of int | C of float
let fGen =
    Gen.choose(32,212)
    |> Gen.map (fun i -> F i)
let cGen =
    Gen.choose(0,100)
    |> Gen.map (fun i -> C (float i))
let tempGen =
    Gen.oneof [fGen; cGen]

tempGen |> Gen.sample 0 100

// Alternatively, choose between generators with specified frequency
let tempGen2 =
    Gen.frequency [90,fGen; 10,cGen]

tempGen2 |> Gen.sample 0 100

// ===================================
// Custom Generators #2 - Building an email address
// ===================================

type EmailAddress = EmailAddress of string

let badEmailGen =
    stringGen
    // only include strings with an @ sign
    |> Gen.filter (fun s -> s.Contains "@")  // this will rarely match :(
    |> Gen.map (fun s -> EmailAddress s)


let emailGen1 =
    // combine two generators
    Gen.two stringGen
    |> Gen.map (fun (s1,s2) -> EmailAddress (s1+"@"+s2) )

emailGen1 |> Gen.sample 10 100

// now exclude nulls
let emailGen2 =
    // set up the generator
    let nonNullGen =
        stringGen
        |> Gen.filter (System.String.IsNullOrEmpty >> not)

    // combine two generators
    Gen.two nonNullGen
    |> Gen.map (fun (s1,s2) -> EmailAddress (s1+"@"+s2) )

emailGen2 |> Gen.sample 10 10




// use the rules, Luke!
// https://en.wikipedia.org/wiki/Email_address#Syntax

let localPart =
    // set up the generators
    let char =
        ['A'..'Z'] @ ['a'..'z'] @ ['0'..'9'] @ (List.ofSeq "!#$%&'*+-/=?^_`{|}~")
        |> Gen.elements
    let dot = Gen.constant '.'

    // from the spec
    let firstChar = char
    let lastChar = char
    let innerChars = Gen.frequency [90,char; 10,dot] |> Gen.listOf

    // combine the generators
    (firstChar,innerChars,lastChar)
    |||> Gen.map3 (fun f i l -> [f] @ i @ [l] |> List.toArray |> System.String)

// use "gen" computation expression as an alternative to using map3
let localPart2 =
    // set up the generators
    let char =
        ['A'..'Z'] @ ['a'..'z'] @ ['0'..'9'] @ (List.ofSeq "!#$%&'*+-/=?^_`{|}~")
        |> Gen.elements
    let dot = Gen.constant '.'

    gen {
        let! firstChar = char
        let! lastChar = char
        let! innerChars = Gen.frequency [90,char; 10,dot] |> Gen.listOf
        return
            [firstChar] @ innerChars @ [lastChar]
            |> List.toArray
            |> System.String
    }


localPart |> Gen.sample 10 10
localPart2 |> Gen.sample 10 10

let domainPart =
    // set up the generators
    let char =
        ['A'..'Z'] @ ['a'..'z'] @ ['0'..'9']
        |> Gen.elements
    let hyphen = Gen.constant '-'

    // from the spec
    let firstChar = char
    let lastChar = char
    let innerChars = Gen.frequency [90,char; 10,hyphen] |> Gen.listOf
    let isNotAllNumeric s = s |> Seq.forall System.Char.IsNumber |> not

    // combine the generators
    (firstChar,innerChars,lastChar)
    |||> Gen.map3 (fun f i l -> [f] @ i @ [l] |> List.toArray |> System.String)
    |> Gen.filter isNotAllNumeric

domainPart |> Gen.sample 10 10


let emailGen3 =
    // combine the generators
    (localPart,domainPart)
    ||> Gen.map2 (fun l d -> EmailAddress (l+"@"+d) )

emailGen3 |> Gen.sample 10 10

// ================================
// Look in Arb.Default though :)
// ================================

let hostName = Arb.Default.HostName().Generator
hostName |> Gen.sample 10 10

let mailAddress = Arb.Default.MailAddress().Generator
mailAddress |> Gen.sample 10 10

let culture =
    Arb.Default.Culture().Generator
    |> Gen.map (fun ci -> ci.IetfLanguageTag)

culture |> Gen.sample 10 10


// ================================
// Replaying tests
// ================================

// a test that fails
let badProp x = [-50..50] |> List.contains x

FsCheck.Check.Quick badProp

// replay FsCheck with same seed
let seed = (959318490,296720405) // copy numbers from above to here
let stdGen = FsCheck.Random.StdGen seed
let replayConfig = {FsCheck.Config.Default with Replay = Some stdGen }
FsCheck.Check.One(replayConfig,badProp)

// Expecto version
let replayExpectoConfig = {FsCheckConfig.defaultConfig with replay = Some seed}
let badTest = testPropertyWithConfig replayExpectoConfig "badProp" badProp
runTests expectoConfig badTest


