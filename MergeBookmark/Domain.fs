module MergeBookmark.Domain

type Folder = {
    Parent : Option<Folder>
    AddDate : Option<int64>
    LastModified : Option<int64>
    Name : string
}

type Bookmark = {
    Parent : Folder
    Link : string
    AddDate : Option<int64>
    Icon : string
    Name : string
}

type Node =
    | Folder of Folder
    | Bookmark of Bookmark
    
module Node =    
    let GetName node =
        match node with
        | Folder f -> $"{f.Name}"
        | Bookmark b -> $"{b.Name}"
        
    let GetParent node =
        match node with
        | Folder f -> f.Parent
        | Bookmark b -> Some b.Parent
        
    let GetLevel node =
        let mutable _count = 0
        
        let rec countLevel (f:Folder) =
            if f.Parent |> Option.isSome then
                do _count <- _count + 1
                countLevel f.Parent.Value
        
        match node with
        | Bookmark b ->
            do _count <- _count + 1
            countLevel b.Parent
        | Folder f -> countLevel f
        
        _count
        
//    let GetIndentName node =
//        let level = GetLevel node
//        sprintf "%s%s" (String.replicate level "  ") (GetName node)

    let GetIndentName node =
        let level = GetLevel node
        sprintf "%d%s" level (GetName node)