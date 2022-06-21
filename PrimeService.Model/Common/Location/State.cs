﻿namespace PrimeService.Model.Common.Location
{

    public class State
    {
        public int id { get; set; }
        public string Name { get; set; }
        public int country_id { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string state_code { get; set; }
        public object type { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}