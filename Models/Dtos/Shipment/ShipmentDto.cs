namespace RiverLine.Api.Models.Dtos.Shipment;

public record ShipmentDto(Guid Id,  Guid ShipmentRequestId, Guid OfferId, Guid VesselId,
    string ShipmentStatus );