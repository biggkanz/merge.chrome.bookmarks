namespace MergeBookmark

open MergeBookmark.Domain

module Operations =
    
    open System.Collections.Generic
    open Parse
    open Domain
    
    let htmlToBookmarkLine file =
        file
            |> Seq.map ParseLine
            |> Seq.choose id
    
    let htmlToEntry file =
        let mutable index = 0
        let parentFolders = List<int>()
        do parentFolders.Add(0)
        
        [
        for line in file do
            match (ParseLine line) with
            | Some (BookmarkLine.Folder fi) ->
                do index <- index + 1                   
                Some (FolderEntry (index,
                    parentFolders.Item(parentFolders.Count-1),
                        fi))
                do parentFolders.Add(index) 
            | Some (BookmarkLine.Mark bi) ->
                do index <- index + 1
                Some (MarkEntry (index,
                      parentFolders.Item(parentFolders.Count-1),
                      bi))
            | Some (BookmarkLine.ListClose) ->
                do parentFolders.RemoveAt(parentFolders.Count-1)
                None    
            | _ -> None
        ]
        |> List.choose id
        
    let entryToBookmark (lst:Entry list) =
        lst
        |> List.map(fun x ->
            match x with
            | MarkEntry (_,_,mark) -> Some mark
            | _ -> None )
        |> List.choose id
        
    let HtmlToBookmark f =
        f
        |> htmlToEntry
        |> entryToBookmark
                
    // tree
        
    let buildTree (list:Entry list) =
        let rec getChildren parentId = list |> List.choose (fun itm ->
            match itm with
            | MarkEntry (i,p,info) when p = parentId ->
                Some (LeafNode info)
            | FolderEntry (i,p,info) when p = parentId ->
                Some (InternalNode (info, getChildren i))
            | _ -> None )
            
        let root =
            list
            |> List.pick (fun itm ->
                match itm with
                | FolderEntry (i,p,info) when p = 0 ->
                    Some (BookmarkTree.InternalNode (info, getChildren i))
                | _ -> failwith "no root node found")
        root
        
    let HtmlToTree f =
        f
        |> htmlToEntry
        |> buildTree
                
    
    let getPath (tree:BookmarkTree) =
        let fMark acc mark =
            acc + mark.name + "| "
        let fFolder acc (folder:FolderInfo) =
            acc + folder.name + " -> "
        Tree.fold fMark fFolder "" tree
