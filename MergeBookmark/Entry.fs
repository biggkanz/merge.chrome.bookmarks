module MergeBookmark.Entry

open Domain

let toFolderEntry lst =
    lst
    |> List.choose (fun x ->
        match x with
        | FolderEntry f -> Some f
        | _ -> None)
    
let toMarkEntry lst =
    lst
    |> List.choose(fun x ->
        match x with
        | MarkEntry m -> Some m
        | _ -> None)

/// Get unique marks and parents from list2
let DiffMarksAndParent (list1:Entry list) (list2:Entry list) =
    /// Return MarkEntry if it doesn't exist in list1
    let tryGetUnique (m:MarkEntry) =
        let _,_,mi = m            
        let dup =
            list1
            |> toMarkEntry
            |> List.tryFind (fun (_,_,x) -> x.href = mi.href)
            
        match dup with
        | Some _ -> None
        | _ -> Some m
      
    // Get the parent folder of the MarkEntry  
    let getParent ((_,p,_):MarkEntry) : FolderEntry =
        let parent =
            list2
            |> toFolderEntry
            |> List.tryFind (fun (i,_,_)-> i = p)
        
        match parent with
        | Some f -> f
        | _ -> failwith "no parent found"
        
    list2
    |> toMarkEntry
    |> List.choose tryGetUnique
    |> List.map (fun k -> (k,getParent k))