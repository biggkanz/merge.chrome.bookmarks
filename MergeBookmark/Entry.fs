module MergeBookmark.Entry

open Domain
open MergeBookmark.Domain

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
    
let getRootFolder lst =
    lst
    |> toFolderEntry
    |> List.minBy (fun x -> x.id)

/// Get unique marks and parents from list2
let DiffMarksAndParent (list1:Entry list) (list2:Entry list) =
    /// Return MarkEntry if it doesn't exist in list1
    let tryGetUnique (me:MarkEntry) =
        let dup =
            list1
            |> toMarkEntry
            |> List.tryFind (fun x -> x.markInfo.href = me.markInfo.href)
            
        match dup with
        | Some _ -> None
        | _ -> Some me
      
    // Get the parent folder of the MarkEntry  
    let getParent (me:MarkEntry) : FolderEntry =
        let parent =
            list2
            |> toFolderEntry
            |> List.tryFind (fun e -> e.id = me.parentId)
        
        match parent with
        | Some f -> f
        | _ -> failwith "no parent found"
        
    list2
    |> toMarkEntry
    |> List.choose tryGetUnique
    |> List.map (fun k -> (k,getParent k))
    
//let InsertMark (mrk:MarkEntry,prnt:FolderEntry) (entries:Entry list) =
//    let tryFindParent p lst =
//        lst
//        |> toFolderEntry
//        |> List.tryFind (fun fe -> fe.folderInfo.name = p.folderInfo.name)
//        
//    let getUniqueId (lst:Entry list) : int =
//        lst
//        |> List.map (fun x -> x.id)
//        |> List.max
//
//    let parent =
//        tryFindParent prnt entries
//        |> Option.defaultValue (getRootFolder entries)
//    
//    (parent, getUniqueId entries)