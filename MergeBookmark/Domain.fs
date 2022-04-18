module MergeBookmark.Domain

type Folder = {
    Parent : Option<Folder>
    AddDate : Option<int64>
    LastModified : Option<int64>
    Name : string
}

type Bookmark = {
    Parent : Option<Folder>
    Ref : string
    AddDate : Option<int64>
    Icon : Option<string>
    Name : string
}

type Node =
    | Folder of Folder
    | Bookmark of Bookmark
    
module Node =    
    let GetName node =
        match node with
        | Folder f -> f.Name
        | Bookmark b -> b.Name