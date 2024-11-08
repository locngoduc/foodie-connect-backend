using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace foodie_connect_backend.Data;

public class Area
    {
        public Guid Id { get; set; }

        [Required]
        public string FormattedAddress { get; set; }
        
        [MaxLength(255)]
        public string? StreetAddress { get; set; }

        public string? Route { get; set; }

        public string? Intersection { get; set; }

        public string? PoliticalEntity { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        public string? AdministrativeAreaLevel1 { get; set; }

        public string? AdministrativeAreaLevel2 { get; set; }

        public string? AdministrativeAreaLevel3 { get; set; }

        public string? Locality { get; set; }

        public string? Sublocality { get; set; }

        public string? Neighborhood { get; set; }

        public string? Premise { get; set; }

        public string? Subpremise { get; set; }

        public string? PostalCode { get; set; }

        public string? PlusCode { get; set; }

        public string? NaturalFeature { get; set; }

        public string? Airport { get; set; }

        public string? Park { get; set; }

        public string? PointOfInterest { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Restaurant> Restaurants { get; set; }
    }
