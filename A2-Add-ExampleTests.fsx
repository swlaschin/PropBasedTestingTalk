open System

#r "lib/Expecto.dll"
open Expecto
let expectoConfig = {defaultConfig with colour = Expecto.Logging.ColourLevel.Colour0}

#load "A1_Add_Implementations.fsx"
open A1_Add_Implementations


// ===============================
// Requirements:
// * Implement a function that adds two numbers together

(*
Me: Hey Garfield, we need a function that adds two numbers together,
    would you mind implementing it?

Garfield: Done!

Me: Thanks, let me write a quick test.
*)

// ===============================
// test of implementation 1

module Test1 =
    open Implementation1

    let tests = testList "implementation 1" [
        test "add 1 3 = 4" {
            let actual = add 1 3
            let expected = 4
            Expect.equal expected actual ""
            }

        test "add 2 2 = 4" {
            let actual = add 2 2
            let expected = 4
            Expect.equal expected actual ""
            }

        ]


runTests expectoConfig Test1.tests


(*
Me: That looks great. Let me do a quick code review as well.

Me: !!!!!!!!!!!!!!!!!!!
*)





(*
Me: Hey Garfield, *I'm* going write the tests first from now on.
    Can you make sure they pass?

Garfield: Sure!

*)



// ===============================
// test of implementation 2

module Test2 =
    open Implementation2

    let tests = testList "implementation 2" [
        test "add 1 3 = 4" {
            let actual = add 1 3
            let expected = 4
            Expect.equal expected actual ""
            }

        test "add 2 2 = 4" {
            let actual = add 2 2
            let expected = 4
            Expect.equal expected actual ""
            }

        test "add 42 1 = 43" {
            let actual = add 42 1
            let expected = 43
            Expect.equal expected actual ""
            }

        test "add -3 -1 = -4" {
            let actual = add -3 -1
            let expected = -4
            Expect.equal expected actual ""
            }

        ]

runTests expectoConfig Test2.tests



(*

Me: Thanks, Garfield. All tests pass!

Me (thinking): Let me just check the implementation again.

Me: !!!!!!!!!!!!!!!!!!!
*)







(*
Me: Garfield, what are you even doing?
    You haven't implemented anything at all,
    you're just making the tests pass.

Garfield: Chill out dude, I'm totally following TDD best
     practices.

     "Write only enough production code to make the
     failing unit test pass."

     http://www.javiersaldana.com/articles/tech/refactoring-the-three-laws-of-tdd

Me: Ok, let's try one more time.

*)


module Test3 =
    open Implementation3

    let tests = testList "implementation 3" [

        let testData = [
            (1,2,3)
            (2,2,4)
            (3,5,8)
            (27,15,42)
            ]

        test "using test data" {
            for (x,y,expected) in testData do
                let actual = add x y
                Expect.equal expected actual ""
            }

        ]


runTests expectoConfig Test3.tests


(*

Me (thinking): I don't trust Garfield at all now.

Me: !!!!!!!!!!!!!!!!!!!

*)


(*

That's when I know who I was dealing with...



... the legendary burned-out, always lazy and often malicious programmer called...




... The EDFH

*)