

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
        Name = "Creative Commons BY-3.0 license"
        Link = "https://creativecommons.org/licenses/by-nc-sa/2.5/it/deed.it"
        MinimalAttributionInfo = None
    }

let private areas = dict [
    Census,
        { 
            DistrictsCount = 2185; 
            Data = 
            {
                License = license
                OriginLocation = None
                DownloadedFrom = "https://opendata.comune.fi.it/"
                LocalPath = """../src/Cities/OpenData/firenze/sez_censimento2011Polygon.geojson"""
            }
        }
    Wards,
        { 
            DistrictsCount = 129; 
            Data = 
            {
                License = license
                OriginLocation = None
                DownloadedFrom = "https://opendata.comune.fi.it/"
                LocalPath = """../src/Cities/OpenData/firenze/rioniPolygon.geojson"""
            }
        }
    Districts,
        { 
            DistrictsCount = 14; 
            Data = 
            {
                License = license
                OriginLocation = None
                DownloadedFrom = "https://opendata.comune.fi.it/"
                LocalPath = """../src/Cities/OpenData/firenze/vecchi_quartieriPolygon.geojson"""
            }
        }
]
let private addresses= 
    {
        OriginLocation = None
        DownloadedFrom = "https://batch.openaddresses.io/"
        LocalPath = """../src/Cities/OpenData/firenze/addresses.geojson"""
        License = license
    } 

let data: Settings =
    {
        Areas = areas
        StreetMapping = mappings
        Addresses = addresses
    }