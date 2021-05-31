// mind the third-party licenses at :
// https://github.com/NetTopologySuite/NetTopologySuite/blob/develop/License.md

//and

//https://numerics.mathdotnet.com/License.html

#r "nuget: MathNet.Numerics"
#r "nuget: NetTopologySuite.IO.GeoJSON"

open System.Collections.Generic
open System.IO
open System.Linq
open System.Text.RegularExpressions

open NetTopologySuite.IO
open NetTopologySuite.Features
open NetTopologySuite.Geometries
open NetTopologySuite.Algorithm.Locate

// entire code will be move to appropriate nuget libary
// for the time being it is more like PoC rather than actual library

let scan f c data = 
    let h = data |> Array.item 0
    let t = data |> Array.skip 0
    let init = f c h

    (init, t) ||> Array.scan f

// geospatial
let isPointWithinFeature (lon,lat) (f:IFeature) = 
    let cords = new Coordinate(lon, lat)
    let locator = new IndexedPointInAreaLocator(f.Geometry)

    let location = locator.Locate(cords)

    location = Location.Interior || location = Location.Boundary
   
// extract feature collection from raw geojson
type GeojsonAnalyzer(geojson:string) = 
    let reader = new GeoJsonReader()
  
    let features = reader.Read<FeatureCollection>(geojson).ToArray()
    member _.Features = features

// addresses downloaded from openaddresses.io miss parent element 
// plus rows are not delimited with commas
let fixOpenAddressesGeojson (raw:string) =
    let beginning = raw.Substring(0,15)
    if beginning.Contains "features" then 
        raw
    else
        Regex.Replace($"{{ \"features\" :[{raw}] }}", @"}\r\n{?|}\n{","},{")

// domain
type Limit =
    | Addresses of length : int
    | Streets of length : int
    | NoLimit

type License = 
    {
        Name : string
        Link : string
        MinimalAttributionInfo : string option
    }
    with 
        member x.info =
            let text = sprintf $"{x.Name}{System.Environment.NewLine}({x.Link})"
            match x.MinimalAttributionInfo with
            | Option.None -> text
            | Some info -> $"{text}{System.Environment.NewLine}Attribution: {info}"

type OpenDataInfo =
    {
        OriginLocation : string option
        DownloadedFrom : string
        LocalPath : string
        License : License
    }


type CityAreaMetadata =
    {
        DistrictsCount:int
        Data : OpenDataInfo
    }

type Area =
    | Communities 
    | Neighbourhoods
    | Wards 
    | Districts
    | Census 
    | Grid   
    | Parcels
    | Statistical
    | Block
    | Electoral

type Settings = 
    {
        Areas : IDictionary<Area, CityAreaMetadata>
        StreetMapping : IDictionary<string,string>
        Addresses : OpenDataInfo
    }

type Address = 
    {
        StreetName :string 
        Id  :string 
        Nr  :string 
        Pos :(float*float)
    }

type Street =
    {
        Name:string
        Addresses:Address[]
    }

    static member sortAddresses (streets:Street[]) =
        streets 
        |> Array.collect (fun s -> s.Addresses |> Array.sortBy (fun a -> a.Nr)) 

let getDistricts kind (data:IDictionary<Area, CityAreaMetadata>)=
    try
        let path = data.[kind]

        let districtsRaw = File.ReadAllText(path.Data.LocalPath)
        GeojsonAnalyzer(districtsRaw).Features
    with
    | ex -> failwith $"Configuration for {kind} area can't be find. Not all cities enable area split for every possible type or you haven't added valid data path. Check out your {{city}}.fsx file."

let parse (addresses: IFeature[]) (mappings:IDictionary<string,string>) = 
    let read (f:IFeature)  name =
        f.Attributes.Item(mappings.[name]).ToString()
    addresses 
    |> Array.map (fun f -> 
        {
            StreetName = read f "street_name"
            Id = read f "address_id"
            Nr = read f "number"
            Pos = (f.Geometry.Coordinate.X, f.Geometry.Coordinate.Y)
        }) 

let addressesToStreets (addresses: Address[]) =
    addresses
    |> Array.groupBy (fun x -> x.StreetName) 
    |> Array.map (fun (k,v) -> 
        { 
            Street.Name = k; 
            Addresses = v |> Array.sortBy ( fun x -> x.Nr) 
        }
    )

let addressesToPositionedStreets (addresses: Address[]) =
    addresses
    |> Array.groupBy (fun x -> x.StreetName) 
    |> Array.map (fun (k,v) -> 
        let data = v |> Array.sortBy ( fun x -> x.Nr)
        {| 
            Name = k; 
            Addresses = data
            Positions = data |> Array.map ( fun x -> x.Pos)
        |}
    )

let rand xs = 
    let r = (Random ())
    xs |> Seq.sortBy (fun _ -> r.Next())

let setup (settings: Settings) area (addressLimitCount : Limit) =

    let addressesRaw = 
        File.ReadAllText(settings.Addresses.LocalPath)
        |> fixOpenAddressesGeojson
    
    let addressesGeo = 
        GeojsonAnalyzer(addressesRaw).Features 
    
    let parsedAddresses = parse addressesGeo settings.StreetMapping
    let streets = addressesToStreets parsedAddresses
    let sortedAddresses = Street.sortAddresses streets

    // short dataset summary
    let addressDistribution =
        streets 
        |> Array.map (fun x -> float x.Addresses.Length) 
    let avg = addressDistribution  |> MathNet.Numerics.Statistics.Statistics.Median 
    let (mean, stdev) = addressDistribution  |>  MathNet.Numerics.Statistics.Statistics.MeanStandardDeviation
    let districts = getDistricts area settings.Areas
    let summary = sprintf "data summary (without limits) -> %s areas: %d, streets: %d, addressess: %d %s addresses per street (median/mean/std-dev): %A/%A/%A" System.Environment.NewLine districts.Length streets.Length addressesGeo.Length System.Environment.NewLine avg mean stdev 

    let addresses = 
        match addressLimitCount with
        | Addresses count -> sortedAddresses |> Array.take count 
        | Streets count -> streets |> Array.take count |> Street.sortAddresses  
        | NoLimit -> sortedAddresses 
    
    let positionedStreets = addressesToPositionedStreets addresses
    addresses , districts, positionedStreets, summary