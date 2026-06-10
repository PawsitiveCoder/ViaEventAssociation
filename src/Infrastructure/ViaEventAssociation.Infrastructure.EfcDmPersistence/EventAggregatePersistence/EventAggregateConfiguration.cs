using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate;
using ViaEventAssociation.Core.Domain.Aggregates.EventAggregate.Values;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.EventAggregatePersistence;

public class EventAggregateConfiguration : IEntityTypeConfiguration<EventAggregate>
{
    public void Configure(EntityTypeBuilder<EventAggregate> builder)
    {
        builder.HasKey("Id");

        builder.Property<EventId>("Id")
            .HasConversion(
                id => id.Value,
                value => EventId.FromGuid(value).Value
            );

        builder.ComplexProperty<EventStatus>("Status")
            .Property<string>("Value")
            .HasColumnName("Status");

        builder.Property<MaxNumberOfGuests>("MaxNumberOfGuests")
            .HasConversion(
                guests => guests.Value,
                value => MaxNumberOfGuests.Create(value).Value
            );

        builder.Property<EventTitle>("Title")
            .HasConversion(
                title => title.Value,
                value => EventTitle.Create(value).Value
            );

        builder.Property<EventDescription>("Description")
            .HasConversion(
                desc => desc.Value,
                value => EventDescription.Create(value).Value
            );

        builder.OwnsOne<TimeInterval>("TimeInterval", intervalBuilder =>
        {
            intervalBuilder.Property<DateTime>("StartDateTime")
                .HasColumnName("StartDateTime");

            intervalBuilder.Property<DateTime>("EndDateTime")
                .HasColumnName("EndDateTime");
        });

        builder.ComplexProperty<EventVisibility>("Visibility")
            .Property<string>("Value")
            .HasColumnName("Visibility");
    }
}
