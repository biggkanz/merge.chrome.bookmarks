module MergeBookmark.Domain

type Folder = {
    Parent : Option<Folder>
    AddDate : string
    LastModified : string
    Name : string
}

type Bookmark = {
    Parent : Option<Folder>
    Ref : string
    AddDate : string
    Icon : Option<string>
    Name : string
}
