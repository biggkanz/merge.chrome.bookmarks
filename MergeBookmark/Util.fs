module MergeBookmark.Util

type Tree<'LeafData,'INodeData> =
    | LeafNode of 'LeafData
    | InternalNode of 'INodeData * Tree<'LeafData,'INodeData> seq
        
/// Generic tree collection functions   
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
            let localAccum = fNode acc nodeInfo
            let finalAccum = subtrees |> Seq.fold recurse localAccum
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
            
    let rec flattenLeaf (tree:Tree<'LeafData,'INodeData>) =
        seq {
            match tree with
            | LeafNode leafInfo -> yield leafInfo
            | InternalNode (_,subtrees) -> yield! subtrees |> Seq.collect flattenLeaf
         }
        
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
        
module Regex =
    
    open System
    open System.Text.RegularExpressions
    
    let (|Integer64|_|) (str: string) =
       let mutable int64value = 0L
       if Int64.TryParse(str, &int64value) then Some(int64value)
       else None
          
    let (|String|_|) (str: string) =
       if str.Length > 0 then Some(str)
       else None
       
    /// parses a regular expression and returns a list of the strings that match
    let (|ParseRegexGroups|_|) regex str =
       let m = Regex(regex, RegexOptions.IgnoreCase).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None
       
    /// parse and return one string match group
    let ParseString regx str =
        match str with
        | ParseRegexGroups regx [String s]
            -> Some s
        | _ -> None
        
    /// return a match group
    let ParseInteger64 regx str =
        match str with
        | ParseRegexGroups regx [Integer64 i]
            -> Some i
        | _ -> None