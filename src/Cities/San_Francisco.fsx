open Domain

let private mappings = 
    dict[
        "street_id", "street"
        "street_name", "street"
        "address_id", "hash"
        "number", "number" 
    ]

let license = 
    {
        Name = "The Open Data Commons – Public Domain Dedication and Licence"
        Link = "http://www.opendatacommons.org/licenses/pddl/1.0/"
        MinimalAttributionInfo = Option.None
    }

let addressingLicense = 
    {
        Name = "Open Data Commons Public Domain Dedication and License"
        Link = "https://www.opendatacommons.org/licenses/pddl/1-0/index.html"
        MinimalAttributionInfo = Some "City and County of San Francisco, Enterprise Addressing System"
    }

let private areas = dict [
    Neighbourhoods,
        { 
            DistrictsCount = 41; 
            Data = 
            {
                License = license
                OriginLocation = Option.None
                DownloadedFrom = "https://data.sfgov.org/Geographic-Locations-and-Boundaries/Analysis-Neighborhoods/p5b7-5n3h"
                LocalPath = """../src/Cities/OpenData/sanfrancisco/Analysis Neighborhoods.geojson"""
            }
        }
    Parcels,
        { 
            DistrictsCount = 47; 
            Data = 
            {
                License = license
                OriginLocation = Option.None
                DownloadedFrom = "https://data.sfgov.org/Geographic-Locations-and-Boundaries/Parcels-with-overlay-attributes/9grn-xjpx"
                LocalPath = """../src/Cities/OpenData/sanfrancisco/Parcels with overlay attributes.geojson"""
            }
        }
]
let private addresses= 
    {
        OriginLocation = Some "https://data.sfgov.org/api/geospatial/ramy-di5m?method=export&format=Shapefile"
        DownloadedFrom = "https://batch.openaddresses.io/"
        LocalPath = """../src/Cities/OpenData/sanfrancisco/us_ca_san_francisco-addresses-county.geojson"""
        License = addressingLicense
    } 

let data: Settings =
    {
        Areas = areas
        StreetMapping = mappings
        Addresses = addresses
    }