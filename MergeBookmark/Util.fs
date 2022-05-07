namespace MergeBookmark.Util

module Log =
    
    open System
    open System.Threading
    
    /// Print a colored log message
    let message =
        let lockObj = obj()
        fun (color : ConsoleColor) (message : string ) ->
            lock lockObj (fun () ->
                Console.ForegroundColor <- color
                printfn $"%s{message} (thread ID: %i{Thread.CurrentThread.ManagedThreadId})"
                Console.ResetColor())
    
    /// Print a red log message.
    let red = message ConsoleColor.Red
    let green = message ConsoleColor.Green
    let yellow = message ConsoleColor.Yellow
    let cyan  = message ConsoleColor.Cyan

module Time =
    
    open System

    /// convert UnixTimeSeconds to System.DateTime object
    let ToDateTime (timestamp:int64) =
        let offset = DateTimeOffset.FromUnixTimeSeconds timestamp
        offset.DateTime
        
    /// convert System.DateTime object to UnixTimeSeconds
    let ToUnixTimeSeconds (datetime:DateTime) =
        let epoch = DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)
        datetime.Subtract(epoch).TotalSeconds
        |> int64
        
module IO =
    
    open System.IO

    let ReadAllLines file =
        Log.green (sprintf $"ReadAllLines: {file}")
        try
            File.ReadAllLines file
        with
        | ex -> failwith $"error in MergeBookmark.IO.ReadAllLines: {file}"
        
    let WriteAllLines file lines =
        Log.green (sprintf $"WriteAllLines: {file}")        
        File.WriteAllLines (file, lines)
        
type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
        
/// Tree collection functions   
module Tree =

    let rec cata fLeaf fNode (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = cata fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            fNode nodeInfo (subtrees |> Seq.map recurse)

    let rec fold fLeaf fNode acc (tree:Tree<'LeafData,'INodeData>) :'r =
        let recurse = fold fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf acc leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            // determine the local accumulator at this level
            let localAccum = fNode acc nodeInfo
            // thread the local accumulator through all the subitems using Seq.fold
            let finalAccum = subtrees |> Seq.fold recurse localAccum
            // ... and return it
            finalAccum
            
    let rec map fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = map fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            let newLeafInfo = fLeaf leafInfo
            LeafNode newLeafInfo
        | InternalNode (nodeInfo,subtrees) ->
            let newNodeInfo = fNode nodeInfo
            let newSubtrees = subtrees |> Seq.map recurse
            InternalNode (newNodeInfo, newSubtrees)
            
    let rec iter fLeaf fNode (tree:Tree<'LeafData,'INodeData>) =
        let recurse = iter fLeaf fNode
        match tree with
        | LeafNode leafInfo ->
            fLeaf leafInfo
        | InternalNode (nodeInfo,subtrees) ->
            subtrees |> Seq.iter recurse
            fNode nodeInfo