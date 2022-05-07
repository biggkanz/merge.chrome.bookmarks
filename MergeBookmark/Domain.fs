module MergeBookmark.Domain

open System
open MergeBookmark.Util
    
type FolderInfo = {name:string; date:string; modified:string}
type MarkInfo = {name:string; href:string; icon:string}

type BookmarkTree = Tree<MarkInfo,FolderInfo>    

// intermediate data for converting bookmark file
type FolderEntry = int * int * FolderInfo
type MarkEntry = int * int * MarkInfo

/// intermediate data with primaryId and parentId
type Entry =
    | FolderEntry of FolderEntry
    | MarkEntry of MarkEntry

/// raw data parsed from a bookmark file line
type BookmarkLine =
    | Folder of FolderInfo
    | Mark of MarkInfo
    | ListOpen 
    | ListClose

let prettyPrint (tree:BookmarkTree) =
    let rec loop depth (tree:BookmarkTree) =
        let spacer = String(' ', depth * 4)
        match tree with
        | LeafNode m ->        
            printfn "%s |- %A" spacer m.name
        | InternalNode (f,subtrees) ->
            printfn "%s | %s" spacer f.name
            subtrees |> Seq.iter (loop (depth + 1)) 
    loop 0 tree