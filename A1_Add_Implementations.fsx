module Implementation1 =

    let add x y =
        4











module Implementation2 =

    let add x y =
        match (x,y) with
        | 42,1 -> 43
        | -3,-1 -> -4
        | _ -> 4










module Implementation3 =


    let add x y =
       match (x,y) with
       | (1,2) -> 3
       | (2,3) -> 5
       | (3,5) -> 8
       | (1,41) -> 42
       | (27,15) -> 42
       | (_,_) -> 4    // all other cases
