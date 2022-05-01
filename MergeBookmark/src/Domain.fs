module MergeBookmark.Domain

open Data

type FolderInfo = {name:string; date:string; modified:string}
type MarkInfo = {name:string; url:string; icon:string}

type ItemTree = Tree<MarkInfo,FolderInfo>    

// intermediate data for converting bookmark file
type FolderInfoItem = int * int * FolderInfo
type MarkInfoItem = int * int * MarkInfo

/// intermediate data with primaryId and parentId
type Item =
    | FolderInfoItem of FolderInfoItem
    | MarkInfoItem of MarkInfoItem

/// raw data parsed from a bookmark file line
type BookmarkLine =
    | FolderInfo of FolderInfo
    | BookmarkInfo of MarkInfo
    | ListCloseTag        
    | Ig
    
type DtoError =
    | FileReadException of exn
    | ValidationError of string