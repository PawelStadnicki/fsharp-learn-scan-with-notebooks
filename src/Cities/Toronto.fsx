open Domain

let license = 
    {
        Name = "Open Government Licence – Toronto"
        Link = "https://www.toronto.ca/city-government/data-research-maps/open-data/open-data-licence/"
        MinimalAttributionInfo = Some "Contains information licensed under the Open Government Licence – Toronto"
    }

let private areas = dict [
    Neighbourhoods,
        { 
            DistrictsCount = 140; 
            Data = 
            {
                License = license
                OriginLocation = Option.None
                DownloadedFrom = "https://open.toronto.ca/dataset/neighbourhoods/"
                LocalPath = """OpenData/toronto/zones/Neighbourhoods.geojson"""
            }
        }
    Wards,
        { 
            DistrictsCount = 47; 
            Data = 
            {
                License = license
                OriginLocation = Option.None
                DownloadedFrom = "https://open.toronto.ca/dataset/city-wards/"
                LocalPath = """OpenData/toronto/zones/47-ward-model-wgs84-latitude-longitude/citygcs.ward_2018_wgs84.geojson"""
            }
        }
    // Toronto has couple of Wards profiles
    // Wards,
    //     { 
    //         DistrictsCount = 25; 
    //         Data = 
    //         {
    //             License = license
    //             OriginLocation = None
    //             DownloadedFrom = "https://open.toronto.ca/dataset/city-wards/"
    //             LocalPath = """OpenData/toronto/zones/25-ward-model-december-2018-wgs84-latitude-longitude/data.geojson"""
    //         }
    //     }
]
let private addresses= 
    {
        OriginLocation = Some "https://open.toronto.ca/dataset/address-points-municipal-toronto-one-address-repository/"
        DownloadedFrom = "https://batch.openaddresses.io/"
        LocalPath = """OpenData/toronto/addresses.geojson"""
        License = license
    } 

// mapping for above addresses path
// it is the same for all openaddresses.io data, for other data source
// you must adjsut it accordingly
let private mappings = 
    dict[
        "street_id", "street"
        "street_name", "street"
        "address_id", "hash"
        "number", "number" 
    ]


let data: Settings =
    {
        Areas = areas
        StreetMapping = mappings
        Addresses = addresses
    }