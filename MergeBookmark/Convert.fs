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
    let mutable index = 0
    let parents = ResizeArray<int>()
    do parents.Add(0)
    
    let folderToEntry folder =
        do index <- index + 1                   
        let result =
            Some (FolderEntry {
                id=index;
                parentId=parents.Item(parents.Count-1);
                folderInfo=folder})
        do parents.Add(index)
        result
        
    let markToEntry mark =
        do index <- index + 1
        Some (MarkEntry {
            id=index;
            parentId=parents.Item(parents.Count-1);
            markInfo=mark})
    
    [for line in file do
        match (ParseHtmlLine line) with
        | Some (BookmarkLine.Folder folder) ->
            folderToEntry folder
        | Some (BookmarkLine.Mark mark) ->
            markToEntry mark            
        | Some BookmarkLine.ListClose ->
            do parents.RemoveAt(parents.Count-1)
            None    
        | _ -> None]
    |> List.choose id
    |> List.append  [FolderEntry{id=0;parentId=(-1);folderInfo={name="Bookmarks";date="0";modified="0"}}]
    
let entryToMark (lst:Entry list) =
    lst
    |> List.map(fun x ->
        match x with
        | MarkEntry me -> Some me.markInfo
        | _ -> None )
    |> List.choose id
          
// tree    
let entryToTree (list:Entry list) =
    let rec getChildren parentId = list |> List.choose (fun itm ->
        match itm with
        | MarkEntry me when me.parentId = parentId ->
            Some (LeafNode me.markInfo)
        | FolderEntry fe when fe.parentId = parentId ->
            Some (InternalNode (fe.folderInfo, getChildren fe.id))
        | _ -> None )
        
    let root =
        list
        |> List.pick (fun itm ->
            match itm with
            | FolderEntry fe when fe.parentId = 0 ->
                Some (BookmarkTree.InternalNode (fe.folderInfo, getChildren fe.id))
            | _ -> failwith "no root node found")
    root
    
let markToTree (list:MarkInfo list) =
    let markInfoToEntry m = MarkEntry {id=1; parentId=1; markInfo=m}
    
    list
    |> List.map markInfoToEntry
    |> List.append [FolderEntry{id=1;parentId=0;folderInfo={name="import";date="";modified=""}}]
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