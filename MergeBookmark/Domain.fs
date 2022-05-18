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
    
type FolderEntry = {id:int; parentId:int; folderInfo:FolderInfo}
type MarkEntry = {id:int; parentId:int; markInfo:MarkInfo}
    
/// intermediate data with primaryId and parentId
type Entry =
    | FolderEntry of FolderEntry
    | MarkEntry of MarkEntry
//and FolderEntry = int * int * FolderInfo
//and MarkEntry = int * int * MarkInfo