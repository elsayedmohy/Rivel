global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.OpenApi;
global using System.Text.Json.Serialization;
global using FluentValidation;
// ************************************************

global using System.Text;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;

// ************************************************

global using RiverLine.Api;
global using RiverLine.Api.Models.Enums;
global using RiverLine.Api.Models.Entities;
global using RiverLine.Api.Models.Dtos.Auth;
global using RiverLine.Api.Models.Dtos.ShipmentRequest;
global using RiverLine.Api.Models.Dtos.Offer;
global using RiverLine.Api.Models.Dtos.Shipment;
global using RiverLine.Api.Data;
global using RiverLine.Api.Services.Auth;
global using RiverLine.Api.Services.ShipmentRequests;
global using RiverLine.Api.Services.Shipments;
global using RiverLine.Api.Services.Offers;
global using RiverLine.Api.Mappers;
global using RiverLine.Api.Settings;
global using RiverLine.Api.Middleware;
global using RiverLine.Api.Extensions;
