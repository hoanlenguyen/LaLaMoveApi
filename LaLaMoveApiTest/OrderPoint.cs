using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LaLaMoveApiTest
{

    public class OrderPoint
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("addresses")]
        public Address Addresses { get; set; }

    }
    public class Location
    {
        [JsonProperty("lat")]
        public string Latitude { get; set; }

        [JsonProperty("lng")]
        public string Longitude { get; set; }
    }
    public class Address
    {
        [JsonProperty("vi_VN")]
        public LanguageString VNAddress { get; set; }
    }
    public class LanguageString
    {
        [JsonProperty("displayString")]
        public string DisplayString { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; } = "VN";
    }
    public class Contact
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
}
