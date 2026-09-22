namespace RiverLine.Api.Mappers;

public class NileBerthMapper
{
    public  NileBerthDto ToBerthDto(NileBerth b) => new(
        b.Id, b.Name, b.ArabicName, b.Governorate, b.Latitude, b.Longitude,
        b.Type.ToString(), b.Axis.ToString(), b.CoordinateAccuracy.ToString());

}