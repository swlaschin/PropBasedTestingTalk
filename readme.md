# Code and slides for Property Based Testing talk

Contents:

* "A_xxx" is the dialog with the Enterprise Developer From Hell
* "B_xxx" where we build a custom property checker to test the properties of addition
* "C_xxx" uses FsCheck to test the same properties of addition
* "D_xxx" has examples of using FsCheck generators
* "E_xxx" has examples of choosing properties for FizzBuzz and more

The slides are PropBasedTesting.pdf

## Useful links

* [FsCheck home page and documentation](https://fscheck.github.io/FsCheck/)
* [Expecto/FsCheck integration](https://github.com/haf/expecto#property-based-tests)
* [My (now ancient) posts on FSFFAP](https://fsharpforfunandprofit.com/pbt/). The associated video was lost with the demise of SkillsMatter :(.
* [Nicolas Rinaudo: Much Ado About Testing (video)](https://www.youtube.com/watch?v=Jhzc7fxY5lw) -- has lots of examples, including face recognition.
* [Writing Better Tests Than Humans Can Part 2: Model-based Tests with FsCheck in C#](http://www.aaronstannard.com/fscheck-property-testing-csharp-part2/) --
"rather than come up with a contrived example, I’m going to use a real example of where we used FsCheck to prove the safety of Helios 2.1 - the socket library that powers the remoting and clustering systems inside Akka.NET 1.1. "
* [Property-Based Testing in a Screencast Editor](https://wickstrom.tech/programming/2019/03/02/property-based-testing-in-a-screencast-editor-introduction.html) -- great example of testing a real world problem. Also, updated and [available as a book](https://leanpub.com/property-based-testing-in-a-screencast-editor).
* [Migrating a C# test suite to property based tests in F#](https://viktorvan.github.io/fsharp/migrating-activelogin.identity-to-property-based-tests-4/) -- "I’ve been wanting to try property-based testing in a real-life situation for some time, and decided to try it out with the test suite for our open source library ActiveLogin.Identity."
