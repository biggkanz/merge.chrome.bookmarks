namespace MergeBookmark

open MergeBookmark.Domain

module Operations =
    
    open System.Collections.Generic
    open Parse
    open Data
    open Domain
    
    let htmlToItem file =
        let mutable PK = 0
        let parentFolderList = List<int>()
        do parentFolderList.Add(0)
        
        [for line in file do
            match (ParseLine line) with
            | BookmarkLine.FolderInfo fi ->
                do PK <- PK + 1                   
                Some (FolderInfoItem (PK,
                      parentFolderList.Item(parentFolderList.Count-1),
                      fi))
                do parentFolderList.Add(PK) 
            | BookmarkLine.BookmarkInfo bi ->
                do PK <- PK + 1
                Some (MarkInfoItem (PK,
                      parentFolderList.Item(parentFolderList.Count-1),
                      bi))
            | BookmarkLine.ListCloseTag ->
                do parentFolderList.RemoveAt(parentFolderList.Count-1)
                None    
            | BookmarkLine.Ig ->
                None]
        |> List.choose id
        
    let itemToBookmark (lst:Item list) =
        lst
        |> List.map(fun x ->
            match x with
            | MarkInfoItem (_,_,mark) -> Some mark
            | _ -> None )
        |> List.choose id
        
    let duplicates xs =
        Seq.scan (fun xs x -> Set.add x xs) Set.empty xs
        |> Seq.zip xs
        |> Seq.choose (fun (x, xs) -> if Set.contains x xs then Some x else None)
        
    // tree
        
    let buildTree (list:Item list) =
        let rec getChildren parentId = list |> List.choose (fun itm ->
            match itm with
            | MarkInfoItem (i,p,info) when p = parentId ->
                Some (LeafNode info)
            | FolderInfoItem (i,p,info) when p = parentId ->
                Some (InternalNode (info, getChildren i))
            | _ -> None )
            
        let root =
            list
            |> List.pick (fun itm ->
                match itm with
                | FolderInfoItem (i,p,info) when p = 0 ->
                    Some (ItemTree.InternalNode (info, getChildren i))
                | _ -> failwith "no root node found")
        root
        
    let getPath (tree:ItemTree) =
        let fMark acc mark =
            acc + mark.name + "| "
        let fFolder acc (nodeInfo:FolderInfo) =
            acc + nodeInfo.name + " -> "
        Tree.fold fMark fFolder "" tree
