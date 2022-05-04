namespace MergeBookmark

module Operations =
    
    open System.Collections.Generic
    open Parse
    open Domain
    
    let htmlToItem file =
        let mutable index = 0
        let parentFolders = List<int>()
        do parentFolders.Add(0)
        
        [for line in file do
            match (ParseLine line) with
            | BookmarkLine.Folder fi ->
                do index <- index + 1                   
                Some (FolderEntry (index,
                      parentFolders.Item(parentFolders.Count-1),
                      fi))
                do parentFolders.Add(index) 
            | BookmarkLine.Mark bi ->
                do index <- index + 1
                Some (MarkEntry (index,
                      parentFolders.Item(parentFolders.Count-1),
                      bi))
            | BookmarkLine.ListCloseTag ->
                do parentFolders.RemoveAt(parentFolders.Count-1)
                None    
            | BookmarkLine.Ig ->
                None]
        |> List.choose id
        
    let itemToBookmark (lst:Item list) =
        lst
        |> List.map(fun x ->
            match x with
            | MarkEntry (_,_,mark) -> Some mark
            | _ -> None )
        |> List.choose id
                
    // tree
        
    let buildTree (list:Item list) =
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
                    Some (ItemTree.InternalNode (info, getChildren i))
                | _ -> failwith "no root node found")
        root
        
    let getPath (tree:ItemTree) =
        let fMark acc mark =
            acc + mark.name + "| "
        let fFolder acc (nodeInfo:Folder) =
            acc + nodeInfo.name + " -> "
        Tree.fold fMark fFolder "" tree
