using System;
using System.Collections.Generic;

namespace ViaEventAssociation.Infrastructure.EfcQueries.Models;

public partial class EventAggregate
{
    public string Id { get; set; } = null!;

    public int MaxNumberOfGuests { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? StartDateTime { get; set; }

    public string? EndDateTime { get; set; }

    public string Status { get; set; } = null!;

    public string Visibility { get; set; } = null!;
}
