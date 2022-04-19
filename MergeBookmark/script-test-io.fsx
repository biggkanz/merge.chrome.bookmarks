
    let cities = ["Munich"; "Rome"; "Florence"; "Berlin"; "Paris"; "Marseille"]

    type Tree<'A> =
    | Node of Tree<'A> * 'A * Tree<'A>
    | Leaf

    let rec insert tree element = 
        match element, tree with
        | x, Leaf                    -> Node(Leaf, x, Leaf)
        | x, Node(l,y,r) when x <= y -> Node((insert l x), y, r)
        | x, Node(l,y,r) when x >  y -> Node(l, y, (insert r x))

    let rec flatten = function
    | Leaf        -> []
    | Node(l,x,r) -> flatten l @ [x] @ flatten r

    let sort xs = xs |> List.fold insert Leaf
                     |> flatten


    let cityTree     = List.fold insert Leaf cities
    let sortedCities = sort cities