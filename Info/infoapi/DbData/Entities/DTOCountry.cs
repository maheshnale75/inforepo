﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PeoplesInfo.Models
{
    public partial class DTOCountry
    {
        

        public int CountryId { get; set; }
        public string CountryName { get; set; }
        [JsonIgnore]
        public virtual ICollection<DTOState> States { get; set; }
    }
}