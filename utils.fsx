// ============================
// utility functions for testing
// ============================

let rec insertElement anElement aList =
    [
    match aList with
    | [] -> 
        yield [anElement] 
    | first::rest ->
        yield anElement::aList
        for sublist in insertElement anElement rest do yield first::sublist
    ]

/// Given a list, return all permutations of it
let rec permutations aList =
    [
    match aList with
    | [] -> 
        yield []
    | first::rest ->
        for sublist in permutations rest do
            yield! insertElement first sublist
    ]


/// Given an element and a list, and other elements previously skipped,
/// return a new list without the specified element.
/// If not found, return None
let rec withoutElementRec anElement aList skipped = 
    match aList with
    | [] -> None
    | head::tail when anElement = head -> 
        // matched, so create a new list from the skipped and the remaining
        // and return it
        let skipped' = List.rev skipped
        Some (skipped' @ tail)
    | head::tail  -> 
        // no match, so prepend head to the skipped and recurse
        let skipped' = head :: skipped
        withoutElementRec anElement tail skipped' 

/// Given an element and a list
/// return a new list without the specified element.
/// If not found, return None
let withoutElement x aList = 
    withoutElementRec x aList [] 

/// Given two lists, return true if they have the same contents
/// regardless of order
let rec isPermutationOf list1 list2 = 
    match list1 with
    | [] -> 
        List.isEmpty list2 // if both empty, true
    | h1::t1 -> 
        match withoutElement h1 list2 with
        | None -> 
            false
        | Some t2 -> 
            isPermutationOf t1 t2

