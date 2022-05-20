/// functions to convert bookmark collections to entry list or tree
module MergeBookmark.Convert

open Util
open Domain
open Parse

let htmlToBookmarkLine file =
    file
        |> Seq.map ParseHtmlLine
        |> Seq.choose id

// todo: cleanup/refactor
// should be a way to do this without mutating
let htmlToEntry file =
    let mutable index = -1
    let parents = ResizeArray<int>()
    do parents.Add(0)
    
    let folderToEntry (folder:FolderInfo) =        
        do index <- index + 1                   
        let result =
            Some
                { id = index
                  parentId = parents.Item(parents.Count-1)
                  info = FolderInfo(folder); }                
        do parents.Add(index)
        result
        
    let markToEntry mark =
        do index <- index + 1
        Some
            { id = index
              parentId = parents.Item(parents.Count-1)
              info = MarkInfo(mark) }
            
    [for line in file do
        match (ParseHtmlLine line) with
        | Some (Folder folder) -> folderToEntry folder
        | Some (Mark mark) -> markToEntry mark            
        | Some ListClose ->
            do parents.RemoveAt(parents.Count-1)
            None    
        | _ -> None]
    |> List.choose id
    //|> List.append  [{ id = 0; parentId = (-1); info = FolderInfo{name="Bookmarks";date="0";modified="0";}}]
  
let entryToMark (lst:Entry list) =
    lst
    |> List.map Entry.tryToMark
    |> List.choose id
    
let entryToMap (lst:Entry list) =    
    lst
    |> List.map (fun e -> (Entry.getHashCode e,e))
    |> Map.ofList
          
// tree    
let entryToTree (list:Entry list) =
    let rec getChildren parentId = list |> List.choose (fun itm ->
        match itm.info with
        | MarkInfo mi when itm.parentId = parentId ->
            Some (LeafNode mi)
        | FolderInfo fi when itm.parentId = parentId ->
            Some (InternalNode (fi, getChildren itm.id))
        | _ -> None )
        
    let root =
        list
        |> List.pick (fun itm ->
            match itm.info with
            | FolderInfo fi when itm.parentId = 0 ->
                Some (BookmarkTree.InternalNode (fi, getChildren itm.id))
            | _ -> failwith "no root node found")
    root
    
let markToTree (list:MarkInfo list) =
    let markInfoToEntry m = {id=1; parentId=1; info=MarkInfo m}
    
    list
    |> List.map markInfoToEntry
    |> List.append [{id=1;parentId=0;info=FolderInfo{name="import";date="";modified=""}}]
    |> entryToTree
    
/// Convert bookmark file to List of MarkInfos
let HtmlToBookmark file =
    file
    |> htmlToEntry
    |> entryToMark
    
/// Convert bookmark file to BookmarkTree
let HtmlToTree file =
    file
    |> htmlToEntry
    |> entryToTree