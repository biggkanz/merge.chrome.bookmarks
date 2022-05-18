module MergeBookmark.Domain

open Util
    
type FolderInfo = {name:string; date:string; modified:string}
type MarkInfo = {name:string; href:string; icon:string}

/// main type for manipulating bookmark collections
type BookmarkTree = Tree<MarkInfo,FolderInfo>

/// raw data parsed from a bookmark file line
type BookmarkLine =
    | Folder of FolderInfo
    | Mark of MarkInfo
    | ListOpen 
    | ListClose

type EntryInfo =
    | MarkInfo of MarkInfo
    | FolderInfo of FolderInfo

type Entry2 = 
    { id: int
      parentId: int
      info: EntryInfo }
