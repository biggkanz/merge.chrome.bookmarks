module MergeBookmark.Entry

open Domain

let getName (e:Entry) =
    match e.info with
    | MarkInfo markInfo -> markInfo.name
    | FolderInfo folderInfo  -> folderInfo.name

let getHashCode (e:Entry) =
    match e.info with
    | MarkInfo markInfo -> markInfo.href.GetHashCode()
    | FolderInfo folderInfo  -> folderInfo.name.GetHashCode()

let tryToMark e =
    match e.info with
    | MarkInfo markInfo -> Some markInfo
    | _ -> None
    
let tryToFolder e =
    match e.info with
    | FolderInfo folderInfo -> Some e
    | _ -> None

let toFolderEntry lst =
    lst
    |> List.map tryToFolder
    |> List.choose id
    
let toMarkInfo lst =
    lst
    |> List.map tryToMark
    |> List.choose id    
    
let getRootFolder lst =
    lst
    |> toFolderEntry
    |> List.minBy (fun x -> x.id)

/// Get unique marks and parents from list2
let DiffMarksAndParent (list1:Entry list) (list2:Entry list) =
    /// Return MarkInfo if it doesn't exist in list1
    let tryGetUniqueMark e =
        let mark = tryToMark e
        match mark with
        | Some m ->
            let dup =
                list1
                |> toMarkInfo
                |> List.tryFind (fun x -> x.href = m.href)
                
            match dup with
            | Some _ -> None
            | _ -> Some e
        | None -> None
      
    // Get the parent of the Entry  
    let getParent (me:Entry) : Entry =
        let parent =
            list2
            |> List.tryFind (fun e -> e.id = me.parentId)
        
        match parent with
        | Some f ->
                match f.info with
                | FolderInfo _ -> f
                | MarkInfo _ -> failwith "error: parent is a mark info"
        | _ -> failwith "no parent found"
        
    list2
    |> List.choose tryGetUniqueMark
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


